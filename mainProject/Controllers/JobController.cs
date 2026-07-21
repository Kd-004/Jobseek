using System.Security.Claims;
using mainProject.Data;
using mainProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
namespace mainProject.Controllers
{
    public class JobController : Controller
    {
        private readonly ApplicationDbContext _context;
        public JobController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: Job
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.HasCompany = await _context.Companies.AnyAsync(c => c.UserId == userId);

            var jobs = await _context.Jobs.ToListAsync(); // adjust to your existing query
            return View(jobs);
        }
        // GET: Job/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var job = await _context.Jobs.FirstOrDefaultAsync(j => j.JobId == id);

            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        // GET: Job/Upsert
        // GET: Job/Upsert/5
        [HttpGet]
        public async Task<IActionResult> Upsert(int? id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var company = await _context.Companies.FirstOrDefaultAsync(c => c.UserId == userId);

            if (company == null)
            {
                TempData["error"] = "Please create your company profile before posting a job.";
                return RedirectToAction("Index", "Company");
            }

            ViewBag.CompanyList = new SelectList(new[] { company }, "Id", "CompanyName", company.Id);

            Job job;
            if (id == null || id == 0)
            {
                job = new Job
                {
                    CompanyId = company.Id.ToString(),
                    Status = "Open"
                };
            }
            else
            {
                job = await _context.Jobs.FirstOrDefaultAsync(j => j.JobId == id);
                if (job == null)
                {
                    return NotFound();
                }
            }

            return View(job);
        }
        // POST: Job/Upsert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Job job)
        {
            if (ModelState.IsValid)
            {
                if (job.JobId == 0)
                {
                    job.PostedDate = DateTime.Now;
                    _context.Jobs.Add(job);
                }
                else
                {
                    _context.Jobs.Update(job);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await PopulateCompanyList();
            return View(job);
        }

        // GET: Job/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();
            var job = await _context.Jobs.FirstOrDefaultAsync(x => x.JobId == id);
            if (job == null)
                return NotFound();
            return View(job);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job != null)
            {
                _context.Jobs.Remove(job);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        //userid
        private async Task PopulateCompanyList()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ViewBag.CompanyList = new SelectList(
                await _context.Companies
                    .Where(c => c.UserId == userId)
                    .ToListAsync(),
                "Id",
                "CompanyName"
            );
        }
    }
}