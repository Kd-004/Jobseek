using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using JobPortal.Models;
using mainProject.Data;
using mainProject.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Controllers
{
    public class JobSeekController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public JobSeekController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var jobSeeker = _db.JobSeekers.FirstOrDefault(c => c.UserId == userId);

            if (jobSeeker == null)
            {
                return View();
            }

            return View(jobSeeker);
        }

        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var jobSeeker = _db.JobSeekers.FirstOrDefault(c => c.UserId == userId);

            if (jobSeeker == null)
            {
                // No profile exists for this user yet, create a new one
                jobSeeker = new JobSeeker
                {
                    UserId = userId
                };
            }

            return View(jobSeeker);
        }

        // POST: /JobSeek/Upsert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(JobSeeker jobSeeker)
        {
            if (!ModelState.IsValid)
            {
                return View(jobSeeker);
            }

            string wwwRootPath = _webHostEnvironment.WebRootPath;

            // Fetch the tracked entity FIRST (for edits) so file-preservation
            // logic and the final save both work against the same tracked instance.
            JobSeeker existing = null;
            if (jobSeeker.Id != 0)
            {
                existing = await _db.JobSeekers.FirstOrDefaultAsync(j => j.Id == jobSeeker.Id);
                if (existing == null)
                {
                    return NotFound();
                }
            }

            // Handle profile image upload
            if (jobSeeker.ProfileImageFile != null)
            {
                string imagesFolder = Path.Combine(wwwRootPath, "uploads", "profile-images");
                Directory.CreateDirectory(imagesFolder);

                string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(jobSeeker.ProfileImageFile.FileName);

                // Delete old image on edit
                if (existing != null && !string.IsNullOrEmpty(existing.ProfileImage))
                {
                    DeleteFileIfExists(wwwRootPath, existing.ProfileImage);
                }

                using (var stream = new FileStream(Path.Combine(imagesFolder, newFileName), FileMode.Create))
                {
                    await jobSeeker.ProfileImageFile.CopyToAsync(stream);
                }
                jobSeeker.ProfileImage = Path.Combine("uploads", "profile-images", newFileName).Replace("\\", "/");
            }
            else if (existing != null)
            {
                // No new file chosen — keep whatever was already stored
                jobSeeker.ProfileImage = existing.ProfileImage;
            }

            // Handle resume upload
            if (jobSeeker.ResumeUpload != null)
            {
                string resumeFolder = Path.Combine(wwwRootPath, "uploads", "resumes");
                Directory.CreateDirectory(resumeFolder);

                string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(jobSeeker.ResumeUpload.FileName);

                // Delete old resume on edit
                if (existing != null && !string.IsNullOrEmpty(existing.ResumeFile))
                {
                    DeleteFileIfExists(wwwRootPath, existing.ResumeFile);
                }

                using (var stream = new FileStream(Path.Combine(resumeFolder, newFileName), FileMode.Create))
                {
                    await jobSeeker.ResumeUpload.CopyToAsync(stream);
                }
                jobSeeker.ResumeFile = Path.Combine("uploads", "resumes", newFileName).Replace("\\", "/");
            }
            else if (existing != null)
            {
                // No new file chosen — keep whatever was already stored
                jobSeeker.ResumeFile = existing.ResumeFile;
            }

            if (existing == null)
            {
                // CREATE
                jobSeeker.CreatedDate = DateTime.Now;
                _db.JobSeekers.Add(jobSeeker);
                TempData["success"] = "Job seeker created successfully";
            }
            else
            {
                // UPDATE — copy values onto the ALREADY-TRACKED entity instead of
                // calling Update(jobSeeker), which would try to track a second
                // object with the same Id and throw the InvalidOperationException
                // you hit ("cannot be tracked because another instance ... is
                // already being tracked").
                jobSeeker.CreatedDate = existing.CreatedDate;
                _db.Entry(existing).CurrentValues.SetValues(jobSeeker);
                TempData["success"] = "Job seeker updated successfully";
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var jobSeeker = _db.JobSeekers
                                  .FirstOrDefault(c => c.Id == id && c.UserId == userId);

            if (jobSeeker == null)
            {
                return NotFound();
            }

            DeleteFileIfExists(_webHostEnvironment.WebRootPath, jobSeeker.ProfileImage);
            DeleteFileIfExists(_webHostEnvironment.WebRootPath, jobSeeker.ResumeFile);

            _db.JobSeekers.Remove(jobSeeker);
            _db.SaveChanges();

            TempData["success"] = "Job seeker deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        private void DeleteFileIfExists(string wwwRootPath, string? relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return;

            string fullPath = Path.Combine(wwwRootPath, relativePath);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }
    }
}