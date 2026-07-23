using System.Diagnostics;
using System.Security.Claims;
using mainProject.Data;
using mainProject.Models;
using mainProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace mainProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly ILogger<AdminDashboardController> _logger;
        private readonly ApplicationDbContext _context;

        public AdminDashboardController(
            ILogger<AdminDashboardController> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Dashboard
        public async Task<IActionResult> Index()
        {
            // Logged-in Admin UserId
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Company belonging to the logged-in Admin
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (company == null)
            {
                TempData["Error"] = "Please create your company profile first.";
                return RedirectToAction("Upsert", "Company");
            }

            // Get all Job IDs belonging to this company
            var companyJobIds = await _context.Jobs
     .Where(j => j.CompanyId == company.Id.ToString())
     .Select(j => j.JobId)
     .ToListAsync();

            var model = new AdminDashboardViewModel
            {
                // Total Active Jobs
                ActiveJobPostings = await _context.Jobs
           .CountAsync(j =>
               j.CompanyId == company.Id.ToString() &&
               j.Status == "Open"),

                // Total Applications
                TotalApplications = await _context.JobApplications
           .CountAsync(a => companyJobIds.Contains(a.JobId)),

                // Accepted Candidates
                ShortlistedCandidates = await _context.JobApplications
           .CountAsync(a =>
               companyJobIds.Contains(a.JobId) &&
               a.Status == "Accepted"),

                // Rejected Candidates
                RejectedCandidates = await _context.JobApplications
           .CountAsync(a =>
               companyJobIds.Contains(a.JobId) &&
               a.Status == "Rejected"),

                // Recent Jobs
                RecentJobPostings = await _context.Jobs
           .Where(j => j.CompanyId == company.Id.ToString())
           .OrderByDescending(j => j.PostedDate)
           .Take(5)
           .ToListAsync(),

                // Recent Applicants
                RecentApplicants = await _context.JobApplications
           .Include(a => a.Job)
           .Include(a => a.JobSeeker)
           .Where(a => companyJobIds.Contains(a.JobId))
           .OrderByDescending(a => a.AppliedDate)
           .Take(5)
           .ToListAsync()
            };
            return View(model);
        }

        // Jobs Page
        public IActionResult Jobs()
        {
            return View();
        }

        // Companies Page
        public IActionResult Companies()
        {
            return View();
        }

        // About Page
        public IActionResult About()
        {
            return View();
        }

        // Contact Page
        public IActionResult Contact()
        {
            return View();
        }

        // Privacy Page
        public IActionResult Privacy()
        {
            return View();
        }

        // Error Page
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}