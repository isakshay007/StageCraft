using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StageCraft.Models;
using StageCraft.ViewModels;

namespace StageCraft.Controllers
{
    [Authorize(Roles = "Admin")] // ✅ Only Admin can access this controller
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: Admin/CreateProductionManager
        [HttpGet]
        public IActionResult CreateProductionManager()
        {
            return View();
        }

        // POST: Admin/CreateProductionManager
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProductionManager(CreateProductionManagerViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var newManager = new ProductionManager
            {
                UserName = model.Email,
                Email = model.Email
                // Later you can add FullName, Department, etc. if you want
            };

            var result = await _userManager.CreateAsync(newManager, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newManager, "ProductionManager");
                return RedirectToAction("Success");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        // GET: Admin/Success
        public IActionResult Success()
        {
            return View();
        }
    }
}
