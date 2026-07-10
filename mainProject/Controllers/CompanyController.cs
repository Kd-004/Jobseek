using System.Security.Claims;
using mainProject.Data;
using mainProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace mainProject.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CompanyController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var company = _context.Companies.FirstOrDefault(c => c.UserId == userId);

            if (company == null)
            {
                // No company exists for this user, create a new one
                company = new Company
                {
                    UserId = userId
                };
            }

            return View(company);
        }

        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var company = _context.Companies.FirstOrDefault(c => c.UserId == userId);

            if (company == null)
            {
                // No company exists for this user, create a new one
                company = new Company
                {
                    UserId = userId
                };
            }

            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company, IFormFile? logoFile)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            company.UserId = userId;

            if (!ModelState.IsValid)
            {
                return View(company);
            }

            var existingCompany = _context.Companies.FirstOrDefault(c => c.UserId == userId);

            if (existingCompany == null)
            {
                company.CreatedDate = DateTime.Now;
                _context.Companies.Add(company);
            }
            else
            {
                existingCompany.CompanyName = company.CompanyName;
                existingCompany.Email = company.Email;
                existingCompany.Phone = company.Phone;
                existingCompany.Website = company.Website;
                existingCompany.Industry = company.Industry;
                existingCompany.Description = company.Description;
                existingCompany.Address = company.Address;
                existingCompany.City = company.City;
                existingCompany.State = company.State;
                existingCompany.Country = company.Country;

                // Update logo if uploaded
                if (!string.IsNullOrEmpty(company.Logo))
                {
                    existingCompany.Logo = company.Logo;
                }

                _context.Companies.Update(existingCompany);
            }

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
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