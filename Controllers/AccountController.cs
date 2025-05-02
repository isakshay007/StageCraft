using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StageCraft.Data;
using StageCraft.Models;
using StageCraft.ViewModels;

namespace StageCraft.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    // ✅ Log login action
                    _context.ActivityLogs.Add(new ActivityLog
                    {
                        UserId = user.Id,
                        Action = "Login",
                        Timestamp = DateTime.UtcNow
                    });

                    // ✅ Update daily login count
                    var today = DateTime.UtcNow.Date;
                    var stat = await _context.LoginStatistics.FirstOrDefaultAsync(s => s.Date == today);
                    if (stat == null)
                    {
                        stat = new LoginStatistic { Date = today, LoginCount = 1 };
                        _context.LoginStatistics.Add(stat);
                    }
                    else
                    {
                        stat.LoginCount++;
                        _context.LoginStatistics.Update(stat);
                    }

                    await _context.SaveChangesAsync();

                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Admin"))
                        return RedirectToAction("AdminPanel", "Admin");
                    else if (roles.Contains("ProductionManager"))
                        return RedirectToAction("Index", "Productions");
                    else
                        return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser
            {
                FullName = model.FullName,
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, isPersistent: false);

                // ✅ Log register action
                _context.ActivityLogs.Add(new ActivityLog
                {
                    UserId = user.Id,
                    Action = "Register",
                    Timestamp = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                // ✅ Log logout action
                _context.ActivityLogs.Add(new ActivityLog
                {
                    UserId = user.Id,
                    Action = "Logout",
                    Timestamp = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();
            }

            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        // GET: /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
