using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mainProject.Data;
using mainProject.Models;

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
        public async Task<IActionResult> Index()
        {
            return View(await _context.Jobs.ToListAsync());
        }

        // GET: Job/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var job = await _context.Jobs.FirstOrDefaultAsync(x => x.JobId == id);

            if (job == null)
                return NotFound();

            return View(job);
        }

        // GET: Job/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Job/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Job job)
        {
            if (ModelState.IsValid)
            {
                job.PostedDate = DateTime.Now;

                _context.Add(job);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(job);
        }

        // GET: Job/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var job = await _context.Jobs.FindAsync(id);

            if (job == null)
                return NotFound();

            return View(job);
        }

        // POST: Job/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Job job)
        {
            if (id != job.JobId)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(job);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

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

        // POST: Job/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var job = await _context.Jobs.FindAsync(id);

            if (job != null)
            {
                _context.Jobs.Remove(job);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}