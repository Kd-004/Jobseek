using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mainProject.Data;
using mainProject.Models;
using System.Linq;
using System.Threading.Tasks;

namespace mainProject.Controllers
{
    public class ApplyJobController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ApplyJobController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /ApplyJob or /ApplyJob/Apply
        // Shows all open jobs, with optional search/category/location filters
      
        [HttpGet]
        public async Task<IActionResult> Index(string search, string category, string location)
        {
            var jobsQuery = _context.Jobs
                .Where(j => j.Status == "Open")
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                jobsQuery = jobsQuery.Where(j =>
                    j.JobTitle.Contains(search) ||
                    j.JobDescription.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                jobsQuery = jobsQuery.Where(j => j.Category == category);
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                jobsQuery = jobsQuery.Where(j => j.Location.Contains(location));
            }
            var jobs = await jobsQuery
      .OrderByDescending(j => j.PostedDate)
      .GroupJoin(
          _context.Companies,
          job => job.CompanyId,
          company => company.Id.ToString(),
          (job, companies) => new { job, companies }
      )
      .SelectMany(
          x => x.companies.DefaultIfEmpty(),
          (x, company) => new JobWithCompany
          {
              Job = x.job,
              CompanyName = company != null
                  ? company.CompanyName
                  : "Company not listed"
          })
      .ToListAsync();
            return View(jobs);
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> MyApplications()
        {
            var userId = _userManager.GetUserId(User);

            var jobSeeker = await _context.JobSeekers
                .FirstOrDefaultAsync(j => j.UserId == userId);

            if (jobSeeker == null)
            {
                return NotFound();
            }

            var applications = await _context.JobApplications
                .Where(a => a.JobSeekerId == jobSeeker.Id)
                .Include(a => a.Job)
                .Include(a => a.Company)
                .OrderByDescending(a => a.AppliedDate)
                .ToListAsync();

            return View(applications);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Applications()
        {
            var applications = await _context.JobApplications
                .Include(a => a.Job)
                .Include(a => a.JobSeeker)
                .Include(a => a.Company)
                .OrderByDescending(a => a.AppliedDate)
                .ToListAsync();

            return View(applications);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Accept(int id)
        {
            var application = await _context.JobApplications.FindAsync(id);

            if (application == null)
                return NotFound();

            application.Status = "Accepted";

            await _context.SaveChangesAsync();

            TempData["success"] = "Application Accepted.";

            return RedirectToAction(nameof(Applications));
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int id)
        {
            var application = await _context.JobApplications.FindAsync(id);

            if (application == null)
                return NotFound();

            application.Status = "Rejected";

            await _context.SaveChangesAsync();

            TempData["success"] = "Application Rejected.";

            return RedirectToAction(nameof(Applications));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int jobId)
        {
            var job = await _context.Jobs
                .Where(j => j.JobId == jobId)
                .GroupJoin(
                    _context.Companies,
                    job => job.CompanyId,
                    company => company.Id.ToString(),
                    (job, companies) => new { job, companies }
                )
                .SelectMany(
                    x => x.companies.DefaultIfEmpty(),
                    (x, company) => new JobWithCompany
                    {
                        Job = x.job,
                        CompanyName = company != null
                            ? company.CompanyName
                            : "Company not listed",

                       
                    }
                )
                .FirstOrDefaultAsync();

            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        // [Authorize]
        public async Task<IActionResult> JobApply(int jobId)
        {
            var job = await _context.Jobs.FindAsync(jobId);
            if (job == null)
            {
                TempData["Error"] = "This job no longer exists.";
                return RedirectToAction(nameof(Details));
            }

            // Get logged-in Identity user id
            var userId = _userManager.GetUserId(User);

            // Get corresponding JobSeeker
            var jobSeeker = await _context.JobSeekers
                .FirstOrDefaultAsync(js => js.UserId == userId);

            if (jobSeeker == null)
            {
                TempData["Error"] = "Job seeker profile not found.";
                return RedirectToAction(nameof(Details));
            }

            bool alreadyApplied = await _context.JobApplications
                .AnyAsync(a => a.JobId == jobId && a.JobSeekerId == jobSeeker.Id);

            if (alreadyApplied)
            {
                TempData["Error"] = "You have already applied to this job.";
                return RedirectToAction(nameof(Details), new { jobId = jobId });
            }

            var application = new JobApplication
            {
                JobId = jobId,
                CompanyId = Convert.ToInt32(job.CompanyId),
                JobSeekerId = jobSeeker.Id,
                AppliedDate = DateTime.Now,
                Status = "Applied"
            };

            _context.JobApplications.Add(application);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"You have successfully applied for {job.JobTitle}.";
            return RedirectToAction(nameof(Details), new { jobId = jobId });
        }
        public IActionResult Test()
        {
            return Content("Test action works");
        }
    }
}