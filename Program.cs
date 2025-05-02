using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StageCraft.Data;
using StageCraft.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 32))
    ));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var context = services.GetRequiredService<AppDbContext>();

    string[] roles = { "Admin", "ProductionManager", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    string adminEmail = "admin@stagecraft.com";
    string adminPassword = "Admin@123";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        var newAdmin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail
        };
        var result = await userManager.CreateAsync(newAdmin, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newAdmin, "Admin");
        }
    }

    var rnd = new Random();
    var today = DateTime.UtcNow;

    for (int i = 1; i <= 30; i++)
    {
        var userEmail = $"user{i}@example.com";
        var user = await userManager.FindByEmailAsync(userEmail);
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = userEmail,
                Email = userEmail,
                FullName = $"User {i}"
            };
            await userManager.CreateAsync(user, "Password@123");
            await userManager.AddToRoleAsync(user, "User");
        }

        var daysAgo = rnd.Next(1, 30);
        var loginTime = today.AddDays(-daysAgo);

        context.ActivityLogs.Add(new ActivityLog
        {
            UserId = user.Id,
            Action = "Register",
            Timestamp = loginTime.AddMinutes(-10)
        });
        context.ActivityLogs.Add(new ActivityLog
        {
            UserId = user.Id,
            Action = "Login",
            Timestamp = loginTime
        });

        var managerEmail = $"manager{i}@example.com";
        var manager = await userManager.FindByEmailAsync(managerEmail);
        if (manager == null)
        {
            manager = new ProductionManager
            {
                UserName = managerEmail,
                Email = managerEmail,
                FullName = $"Manager {i}"
            };
            await userManager.CreateAsync(manager, "Password@123");
            await userManager.AddToRoleAsync(manager, "ProductionManager");
        }

        var mgrDaysAgo = rnd.Next(1, 30);
        var mgrLoginTime = today.AddDays(-mgrDaysAgo);

        context.ActivityLogs.Add(new ActivityLog
        {
            UserId = manager.Id,
            Action = "Register",
            Timestamp = mgrLoginTime.AddMinutes(-10)
        });
        context.ActivityLogs.Add(new ActivityLog
        {
            UserId = manager.Id,
            Action = "Login",
            Timestamp = mgrLoginTime
        });
    }

    await context.SaveChangesAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
