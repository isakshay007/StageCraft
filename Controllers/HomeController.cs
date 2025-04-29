using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StageCraft.Data;

namespace StageCraft.Controllers
{
    [Authorize] //  Require login for all actions in HomeController
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
                .OrderByDescending(p => p.OpeningDay)
                .Take(9)
                .ToListAsync();

            return View(spotlightProductions);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
