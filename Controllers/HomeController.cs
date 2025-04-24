using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StageCraft.Data;

namespace StageCraft.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var spotlightProductions = await _context.Productions
                .OrderByDescending(p => p.OpeningDay) // You can keep .Id if preferred
                .Take(9) // ✅ Show up to 9 productions
                .ToListAsync();

            return View(spotlightProductions);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
