using System.Security.Claims;
using JobPortal.Models;
using mainProject.Data;
using mainProject.Models;
using mainProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace mainProject.Controllers
{
    [Authorize(Roles = "User")]
    public class UserDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var model = new UserDashboard();

            // Get logged-in JobSeeker
            var jobSeeker = await _context.JobSeekers
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (jobSeeker == null)
            {
                return View(model);
            }

            // Total Applied Jobs
            model.AppliedJobs = await _context.JobApplications
                .CountAsync(x => x.JobSeekerId == jobSeeker.Id);

            // Placeholder values
            model.AcceptedJobs = await _context.JobApplications
      .CountAsync(x =>
          x.JobSeekerId == jobSeeker.Id &&
          x.Status == "Accepted");

            model.RejectedJobs = await _context.JobApplications
                .CountAsync(x =>
                    x.JobSeekerId == jobSeeker.Id &&
                    x.Status == "Rejected");
            model.ProfileCompletion = 80;

            // Recent Applications
            model.RecentApplications = await _context.JobApplications
                .Include(x => x.Job)
                .Where(x => x.JobSeekerId == jobSeeker.Id)
                .OrderByDescending(x => x.AppliedDate)
                .Take(5)
                .ToListAsync();

            // Recommended Jobs
            model.RecommendedJobs = await _context.Jobs
                .Where(j => j.Status == "Open")
                .OrderByDescending(j => j.PostedDate)
                .Take(5)
                .ToListAsync();

            return View(model);
        }
    }
}