using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StageCraft.Data;
using StageCraft.Models;
using StageCraft.ViewModels;

namespace StageCraft.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(UserManager<ApplicationUser> userManager, AppDbContext context, ILogger<AdminController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult AdminPanel()
        {
            ViewBag.SuccessMessage = TempData["Success"];
            ViewBag.ErrorMessage = TempData["Error"];
            return View();
        }

        [HttpGet]
        public IActionResult CreateProductionManager()
        {
            return View(new CreateProductionManagerViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProductionManager(CreateProductionManagerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please correct the form errors.";
                return View(model);
            }

            var newManager = new ProductionManager
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(newManager, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newManager, "ProductionManager");
                _context.ActivityLogs.Add(new ActivityLog
                {
                    UserId = newManager.Id,
                    Action = "Created Production Manager",
                    Timestamp = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Production Manager {model.Email} created.");
                return RedirectToAction(nameof(CreateProductionManagerSuccess));
            }
            else
            {
                var errorMessages = string.Join("; ", result.Errors.Select(e => e.Description));
                TempData["Error"] = $"Failed to create Production Manager: {errorMessages}";
                _logger.LogWarning($"Failed to create Production Manager {model.Email}: {errorMessages}");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult CreateProductionManagerSuccess()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> UsersList()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    _context.ActivityLogs.Add(new ActivityLog
                    {
                        UserId = user.Id,
                        Action = "Deleted User",
                        Timestamp = DateTime.UtcNow
                    });
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "✅ User deleted successfully.";
                    _logger.LogInformation($"User {user.Email} deleted.");
                }
                else
                {
                    var errorMessages = string.Join("; ", result.Errors.Select(e => e.Description));
                    TempData["Error"] = $"Failed to delete user: {errorMessages}";
                    _logger.LogWarning($"Failed to delete user {user.Email}: {errorMessages}");
                }
            }
            else
            {
                TempData["Error"] = "User not found.";
                _logger.LogWarning($"DeleteUser: User with ID {id} not found.");
            }

            return RedirectToAction(nameof(UsersList));
        }

        [HttpGet]
        public async Task<IActionResult> ProductionManagersList()
        {
            var managers = await _userManager.GetUsersInRoleAsync("ProductionManager");
            return View(managers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProductionManager(string id)
        {
            var manager = await _userManager.FindByIdAsync(id);
            if (manager != null)
            {
                var result = await _userManager.DeleteAsync(manager);
                if (result.Succeeded)
                {
                    _context.ActivityLogs.Add(new ActivityLog
                    {
                        UserId = manager.Id,
                        Action = "Deleted Production Manager",
                        Timestamp = DateTime.UtcNow
                    });
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "✅ Production Manager deleted successfully.";
                    _logger.LogInformation($"Production Manager {manager.Email} deleted.");
                }
                else
                {
                    var errorMessages = string.Join("; ", result.Errors.Select(e => e.Description));
                    TempData["Error"] = $"Failed to delete Production Manager: {errorMessages}";
                    _logger.LogWarning($"Failed to delete Production Manager {manager.Email}: {errorMessages}");
                }
            }
            else
            {
                TempData["Error"] = "Production Manager not found.";
                _logger.LogWarning($"DeleteProductionManager: Manager with ID {id} not found.");
            }

            return RedirectToAction(nameof(ProductionManagersList));
        }

        [HttpGet]
        public async Task<IActionResult> RecentActivityLogs()
        {
            var recentLogs = await _context.ActivityLogs
                .OrderByDescending(l => l.Timestamp)
                .Take(50)
                .Join(_userManager.Users,
                    log => log.UserId,
                    user => user.Id,
                    (log, user) => new ActivityLogViewModel
                    {
                        UserId = log.UserId,
                        UserName = user.UserName,
                        UserEmail = user.Email,
                        Action = log.Action,
                        Timestamp = log.Timestamp
                    })
                .ToListAsync();

            return View(recentLogs);
        }

        [HttpGet]
        public async Task<IActionResult> LoginStatistics()
        {
            var totalAdmins = await _userManager.GetUsersInRoleAsync("Admin");
            var totalManagers = await _userManager.GetUsersInRoleAsync("ProductionManager");
            var totalUsers = await _userManager.GetUsersInRoleAsync("User");

            var model = new LoginStatisticsViewModel
            {
                TotalAdmins = totalAdmins.Count,
                TotalManagers = totalManagers.Count,
                TotalUsers = totalUsers.Count,
                RoleNames = new List<string> { "Admins", "Production Managers", "Users" },
                RoleCounts = new List<int> { totalAdmins.Count, totalManagers.Count, totalUsers.Count }
            };

            return View(model);
        }
    }
}
