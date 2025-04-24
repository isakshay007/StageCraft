using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StageCraft.Data;
using StageCraft.Models;

namespace StageCraft.Controllers
{
    public class ProductionsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProductionsController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Productions
        public async Task<IActionResult> Index()
        {
            var productions = await _context.Productions
                .OrderByDescending(p => p.OpeningDay)
                .ToListAsync();

            return View(productions);
        }

        // GET: Productions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Productions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Production production, IFormFile? posterFile)
        {
            if (!ModelState.IsValid)
            {
                // Provide validation feedback in the view
                return View(production);
            }

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

        // GET: Productions/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var production = await _context.Productions.FindAsync(id);
            return production == null ? NotFound() : View(production);
        }

        // POST: Productions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Production updated, IFormFile? posterFile)
        {
            if (id != updated.Id)
                return NotFound();

            var existing = await _context.Productions.FindAsync(id);
            if (existing == null)
                return NotFound();

            if (!ModelState.IsValid)
                return View(updated);

            // Update fields
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

            // Optional poster update
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

        // GET: Productions/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var production = await _context.Productions.FindAsync(id);
            return production == null ? NotFound() : View(production);
        }

        // POST: Productions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
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
