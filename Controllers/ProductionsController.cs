using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StageCraft.Data;
using StageCraft.Models;
using X.PagedList;

namespace StageCraft.Controllers
{
    public class ProductionsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private const int PageSize = 9;

        public ProductionsController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string searchString, int? page)
        {
            ViewBag.CurrentFilter = searchString;

            var productions = _context.Productions.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                productions = productions.Where(p => p.Title.Contains(searchString));
            }

            productions = productions.OrderByDescending(p => p.OpeningDay);

            int pageNumber = page ?? 1;
            var pagedProductions = await productions.ToPagedListAsync(pageNumber, PageSize);

            return View(pagedProductions);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id, string? from)
        {
            var production = await _context.Productions.FindAsync(id);
            if (production == null)
            {
                return NotFound();
            }

            // Load related comments
            var comments = await _context.Comments
                .Include(c => c.User)
                .Where(c => c.ProductionId == id)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            ViewBag.Comments = comments;
            ViewBag.From = from ?? "productions"; // Default to productions if not passed

            return View(production);
        }

        [Authorize(Roles = "Admin,ProductionManager")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,ProductionManager")]
        public async Task<IActionResult> Create(Production production, IFormFile? posterFile)
        {
            if (!ModelState.IsValid)
                return View(production);

            if (posterFile != null && posterFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(posterFile.FileName);
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await posterFile.CopyToAsync(stream);
                }

                production.PosterImagePath = "/images/" + fileName;
            }

            _context.Add(production);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,ProductionManager")]
        public async Task<IActionResult> Edit(int id)
        {
            var production = await _context.Productions.FindAsync(id);
            if (production == null)
                return NotFound();

            return View(production);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,ProductionManager")]
        public async Task<IActionResult> Edit(int id, Production updated, IFormFile? posterFile)
        {
            if (id != updated.Id)
                return NotFound();

            var existing = await _context.Productions.FindAsync(id);
            if (existing == null)
                return NotFound();

            if (!ModelState.IsValid)
                return View(updated);

            existing.Title = updated.Title;
            existing.Description = updated.Description;
            existing.Playwright = updated.Playwright;
            existing.Runtime = updated.Runtime;
            existing.OpeningDay = updated.OpeningDay;
            existing.ClosingDay = updated.ClosingDay;
            existing.ShowTimeEve = updated.ShowTimeEve;
            existing.Season = updated.Season;
            existing.IsWorldPremiere = updated.IsWorldPremiere;
            existing.TicketLink = updated.TicketLink;

            if (posterFile != null && posterFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(posterFile.FileName);
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await posterFile.CopyToAsync(stream);
                }

                existing.PosterImagePath = "/images/" + fileName;
            }

            _context.Update(existing);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,ProductionManager")]
        public async Task<IActionResult> Delete(int id)
        {
            var production = await _context.Productions.FindAsync(id);
            if (production == null)
                return NotFound();

            return View(production);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,ProductionManager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var production = await _context.Productions.FindAsync(id);
            if (production != null)
            {
                _context.Productions.Remove(production);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
