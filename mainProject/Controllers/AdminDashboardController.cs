using System.Diagnostics;
using AspNetCoreGeneratedDocument;
using mainProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace mainProject.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly ILogger<AdminDashboardController> _logger;

        public AdminDashboardController(ILogger<AdminDashboardController> logger)
        {
            _logger = logger;
        }

        // Landing Page
        public IActionResult Index()
        {
            return View();
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