using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mainProject.Data;
using mainProject.Models;

namespace mainProject.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CompanyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Company
        public async Task<IActionResult> Index()
        {
            return View(await _context.Companies.ToListAsync());
        }

        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            Company company = new Company();

            if (id == null || id == 0)
            {
                return View(company);
            }

            company = _context.Companies.FirstOrDefault(x => x.Id == id);

            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company, IFormFile? logoFile)
        {
            if (ModelState.IsValid)
            {
                // Upload Logo
                if (logoFile != null)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/company");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(logoFile.FileName);

                    string filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        logoFile.CopyTo(stream);
                    }

                    company.Logo = fileName;
                }

                if (company.Id == 0)
                {
                    company.CreatedDate = DateTime.Now;

                    _context.Companies.Add(company);
                }
                else
                {
                    _context.Companies.Update(company);
                }

                _context.SaveChanges();

                TempData["success"] = "Company saved successfully.";

                return RedirectToAction(nameof(Index));
            }

            return View(company);
        }

       

        // GET: Company/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == id);

            if (company == null)
                return NotFound();

            return View(company);
        }

        // GET: Company/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Company/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Company company)
        {
            if (ModelState.IsValid)
            {
                company.CreatedDate = DateTime.Now;

                _context.Add(company);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(company);
        }

        // GET: Company/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var company = await _context.Companies.FindAsync(id);

            if (company == null)
                return NotFound();

            return View(company);
        }

        // POST: Company/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Company company)
        {
            if (id != company.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(company);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.Id))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(company);
        }

        // GET: Company/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == id);

            if (company == null)
                return NotFound();

            return View(company);
        }

        // POST: Company/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var company = await _context.Companies.FindAsync(id);

            if (company != null)
            {
                _context.Companies.Remove(company);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.Id == id);
        }
    }
}