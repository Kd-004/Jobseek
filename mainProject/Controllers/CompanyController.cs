using System.Security.Claims;
using mainProject.Data;
using mainProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace mainProject.Controllers
{
    [Authorize(Roles = "Admin")] // default for this controller — most actions are employer-only
    public class CompanyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CompanyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // "My Company" profile — for the logged-in employer viewing their own company
        [HttpGet]
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var company = _context.Companies.FirstOrDefault(c => c.UserId == userId);

            if (company == null)
            {
                return View();
            }

            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var company = _context.Companies
                                  .FirstOrDefault(c => c.Id == id && c.UserId == userId);

            if (company == null)
            {
                return NotFound();
            }

            _context.Companies.Remove(company);
            _context.SaveChanges();

            TempData["success"] = "Company deleted successfully.";

            return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> Upsert(Company company, IFormFile? logoFile)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            company.UserId = userId;

            if (!ModelState.IsValid)
            {
                return View(company);
            }

            // Upload Logo
            if (logoFile != null && logoFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads",
                    "company");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(logoFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await logoFile.CopyToAsync(stream);
                }

                company.Logo = "/uploads/company/" + fileName;
            }

            var existingCompany = await _context.Companies
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (existingCompany == null)
            {
                company.CreatedDate = DateTime.Now;
                await _context.Companies.AddAsync(company);
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

                // Update Logo
                if (!string.IsNullOrEmpty(company.Logo))
                {
                    // Delete old logo
                    if (!string.IsNullOrEmpty(existingCompany.Logo))
                    {
                        var oldFile = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot",
                            existingCompany.Logo.TrimStart('/')
                                .Replace("/", Path.DirectorySeparatorChar.ToString()));

                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }
                    }

                    existingCompany.Logo = company.Logo;
                }

                _context.Companies.Update(existingCompany);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Company profile saved successfully.";

            return RedirectToAction(nameof(Index));
        }
        // GET: Company/Details/5 — PUBLIC company page (job seekers/guests browse this)
        [AllowAnonymous]
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

        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.Id == id);
        }
    }
}