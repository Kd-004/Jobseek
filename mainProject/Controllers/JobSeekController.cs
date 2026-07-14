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

            var JobSeeker = _db.JobSeekers.FirstOrDefault(c => c.UserId == userId);

            if (JobSeeker == null)
            {
                // No company exists for this user, create a new one
                JobSeeker = new JobSeeker
                {
                    UserId = userId
                };
            }

            return View(JobSeeker);
        }

      

        // POST: /JobSeeker/Upsert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(JobSeeker jobSeeker)
        {
            if (!ModelState.IsValid)
            {
                return View(jobSeeker);
            }

            string wwwRootPath = _webHostEnvironment.WebRootPath;

            // Handle profile image upload
            if (jobSeeker.ProfileImageFile != null)
            {
                string imagesFolder = Path.Combine(wwwRootPath, "uploads", "profile-images");
                Directory.CreateDirectory(imagesFolder);

                string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(jobSeeker.ProfileImageFile.FileName);

                // Delete old image on edit
                if (jobSeeker.Id != 0 && !string.IsNullOrEmpty(jobSeeker.ProfileImage))
                {
                    DeleteFileIfExists(wwwRootPath, jobSeeker.ProfileImage);
                }

                using (var stream = new FileStream(Path.Combine(imagesFolder, newFileName), FileMode.Create))
                {
                    await jobSeeker.ProfileImageFile.CopyToAsync(stream);
                }
                jobSeeker.ProfileImage = Path.Combine("uploads", "profile-images", newFileName).Replace("\\", "/");
            }

            // Handle resume upload
            if (jobSeeker.ResumeUpload != null)
            {
                string resumeFolder = Path.Combine(wwwRootPath, "uploads", "resumes");
                Directory.CreateDirectory(resumeFolder);

                string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(jobSeeker.ResumeUpload.FileName);

                // Delete old resume on edit
                if (jobSeeker.Id != 0 && !string.IsNullOrEmpty(jobSeeker.ResumeFile))
                {
                    DeleteFileIfExists(wwwRootPath, jobSeeker.ResumeFile);
                }

                using (var stream = new FileStream(Path.Combine(resumeFolder, newFileName), FileMode.Create))
                {
                    await jobSeeker.ResumeUpload.CopyToAsync(stream);
                }
                jobSeeker.ResumeFile = Path.Combine("uploads", "resumes", newFileName).Replace("\\", "/");
            }

            if (jobSeeker.Id == 0)
            {
                jobSeeker.CreatedDate = DateTime.Now;
                _db.JobSeekers.Add(jobSeeker);
                TempData["success"] = "Job seeker created successfully";
            }
            else
            {
                var existing = await _db.JobSeekers.FirstOrDefaultAsync(j => j.Id == jobSeeker.Id);
                if (existing == null)
                {
                    return NotFound();
                }

                // Preserve stored file paths if no new file was uploaded
                if (jobSeeker.ProfileImageFile == null)
                {
                    jobSeeker.ProfileImage = existing.ProfileImage;
                }
                if (jobSeeker.ResumeUpload == null)
                {
                    jobSeeker.ResumeFile = existing.ResumeFile;
                }
                jobSeeker.CreatedDate = existing.CreatedDate;

                _db.JobSeekers.Update(jobSeeker);
                TempData["success"] = "Job seeker updated successfully";
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /JobSeeker/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var jobSeeker = await _db.JobSeekers.FindAsync(id);
            if (jobSeeker == null)
            {
                return NotFound();
            }
            return View(jobSeeker);
        }

        // POST: /JobSeeker/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jobSeeker = await _db.JobSeekers.FindAsync(id);
            if (jobSeeker == null)
            {
                return NotFound();
            }

            string wwwRootPath = _webHostEnvironment.WebRootPath;
            DeleteFileIfExists(wwwRootPath, jobSeeker.ProfileImage);
            DeleteFileIfExists(wwwRootPath, jobSeeker.ResumeFile);

            _db.JobSeekers.Remove(jobSeeker);
            await _db.SaveChangesAsync();
            TempData["success"] = "Job seeker deleted successfully";
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