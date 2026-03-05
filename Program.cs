using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using StageCraft.Data;
using StageCraft.Models;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Load .env file if present (silently ignored in production)
Env.Load();

var port = Environment.GetEnvironmentVariable("PORT");
if (port != null)
{
    builder.WebHost.UseUrls($"http://+:{port}");
}

var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
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

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Seed roles & users
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var context = services.GetRequiredService<AppDbContext>();

    await context.Database.MigrateAsync();

    string[] roles = { "Admin", "ProductionManager", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Load from env
    string adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? "admin@fallback.com";
    string adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? "Fallback@123";

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

        context.ActivityLogs.AddRange(new[]
        {
            new ActivityLog { UserId = user.Id, Action = "Register", Timestamp = loginTime.AddMinutes(-10) },
            new ActivityLog { UserId = user.Id, Action = "Login", Timestamp = loginTime }
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

        context.ActivityLogs.AddRange(new[]
        {
            new ActivityLog { UserId = manager.Id, Action = "Register", Timestamp = mgrLoginTime.AddMinutes(-10) },
            new ActivityLog { UserId = manager.Id, Action = "Login", Timestamp = mgrLoginTime }
        });
    }

    await context.SaveChangesAsync();

    if (!context.Productions.Any())
    {
        var productions = new List<Production>
        {
            new Production
            {
                Title = "Hamlet",
                Description = "Shakespeare's timeless tragedy of a Danish prince seeking revenge for his father's murder. A haunting exploration of madness, mortality, and indecision.",
                Playwright = "William Shakespeare",
                Runtime = 180,
                OpeningDay = new DateTime(2025, 6, 1),
                ClosingDay = new DateTime(2025, 7, 15),
                ShowTimeEve = new DateTime(2025, 6, 1, 19, 30, 0),
                Season = 2025,
                IsWorldPremiere = false,
                TicketLink = "https://example.com/hamlet",
                PosterImagePath = "https://picsum.photos/seed/hamlet/400/600"
            },
            new Production
            {
                Title = "A Streetcar Named Desire",
                Description = "Blanche DuBois arrives at her sister's home in New Orleans, where her fragile world collides with the brutal reality of Stanley Kowalski.",
                Playwright = "Tennessee Williams",
                Runtime = 150,
                OpeningDay = new DateTime(2025, 5, 10),
                ClosingDay = new DateTime(2025, 6, 20),
                ShowTimeEve = new DateTime(2025, 5, 10, 20, 0, 0),
                Season = 2025,
                IsWorldPremiere = false,
                TicketLink = "https://example.com/streetcar",
                PosterImagePath = "https://picsum.photos/seed/streetcar/400/600"
            },
            new Production
            {
                Title = "Death of a Salesman",
                Description = "Willy Loman struggles with the collapse of his career and family life in this searing portrait of the American Dream unraveling.",
                Playwright = "Arthur Miller",
                Runtime = 165,
                OpeningDay = new DateTime(2025, 7, 5),
                ClosingDay = new DateTime(2025, 8, 10),
                ShowTimeEve = new DateTime(2025, 7, 5, 19, 0, 0),
                Season = 2025,
                IsWorldPremiere = false,
                TicketLink = "https://example.com/salesman",
                PosterImagePath = "https://picsum.photos/seed/salesman/400/600"
            },
            new Production
            {
                Title = "The Glass Menagerie",
                Description = "A memory play about the Wingfield family, trapped by their illusions and desperate longing for escape from a crumbling St. Louis apartment.",
                Playwright = "Tennessee Williams",
                Runtime = 130,
                OpeningDay = new DateTime(2025, 4, 15),
                ClosingDay = new DateTime(2025, 5, 25),
                ShowTimeEve = new DateTime(2025, 4, 15, 19, 30, 0),
                Season = 2025,
                IsWorldPremiere = false,
                TicketLink = "https://example.com/menagerie",
                PosterImagePath = "https://picsum.photos/seed/menagerie/400/600"
            },
            new Production
            {
                Title = "Waiting for Godot",
                Description = "Two men wait endlessly by a barren tree for someone named Godot who never arrives. A masterpiece of absurdist theatre.",
                Playwright = "Samuel Beckett",
                Runtime = 120,
                OpeningDay = new DateTime(2025, 8, 1),
                ClosingDay = new DateTime(2025, 9, 5),
                ShowTimeEve = new DateTime(2025, 8, 1, 20, 0, 0),
                Season = 2025,
                IsWorldPremiere = false,
                TicketLink = "https://example.com/godot",
                PosterImagePath = "https://picsum.photos/seed/godot/400/600"
            },
            new Production
            {
                Title = "The Crucible",
                Description = "The Salem witch trials become a chilling allegory for mass hysteria and the destruction wrought by fear and false accusations.",
                Playwright = "Arthur Miller",
                Runtime = 145,
                OpeningDay = new DateTime(2025, 9, 10),
                ClosingDay = new DateTime(2025, 10, 20),
                ShowTimeEve = new DateTime(2025, 9, 10, 19, 0, 0),
                Season = 2025,
                IsWorldPremiere = false,
                TicketLink = "https://example.com/crucible",
                PosterImagePath = "https://picsum.photos/seed/crucible/400/600"
            },
            new Production
            {
                Title = "A Raisin in the Sun",
                Description = "The Younger family dreams of a better life on the South Side of Chicago. A groundbreaking drama about race, family, and aspiration.",
                Playwright = "Lorraine Hansberry",
                Runtime = 140,
                OpeningDay = new DateTime(2025, 3, 20),
                ClosingDay = new DateTime(2025, 5, 1),
                ShowTimeEve = new DateTime(2025, 3, 20, 19, 30, 0),
                Season = 2025,
                IsWorldPremiere = false,
                TicketLink = "https://example.com/raisin",
                PosterImagePath = "https://picsum.photos/seed/raisin/400/600"
            },
            new Production
            {
                Title = "Midnight Echoes",
                Description = "A world premiere thriller set in an abandoned opera house where three strangers discover their fates are intertwined by a decades-old secret.",
                Playwright = "Elena Vasquez",
                Runtime = 110,
                OpeningDay = new DateTime(2025, 10, 5),
                ClosingDay = new DateTime(2025, 11, 15),
                ShowTimeEve = new DateTime(2025, 10, 5, 20, 0, 0),
                Season = 2025,
                IsWorldPremiere = true,
                TicketLink = "https://example.com/echoes",
                PosterImagePath = "https://picsum.photos/seed/echoes/400/600"
            },
            new Production
            {
                Title = "Romeo and Juliet",
                Description = "Two young lovers from feuding families risk everything for a forbidden love. Shakespeare's most iconic romantic tragedy.",
                Playwright = "William Shakespeare",
                Runtime = 155,
                OpeningDay = new DateTime(2025, 2, 14),
                ClosingDay = new DateTime(2025, 3, 30),
                ShowTimeEve = new DateTime(2025, 2, 14, 19, 30, 0),
                Season = 2025,
                IsWorldPremiere = false,
                TicketLink = "https://example.com/romeo",
                PosterImagePath = "https://picsum.photos/seed/romeo/400/600"
            },
            new Production
            {
                Title = "Fences",
                Description = "Troy Maxson, a former baseball player, builds fences both literal and metaphorical as he confronts race, regret, and responsibility in 1950s Pittsburgh.",
                Playwright = "August Wilson",
                Runtime = 140,
                OpeningDay = new DateTime(2025, 11, 1),
                ClosingDay = new DateTime(2025, 12, 15),
                ShowTimeEve = new DateTime(2025, 11, 1, 19, 0, 0),
                Season = 2025,
                IsWorldPremiere = false,
                TicketLink = "https://example.com/fences",
                PosterImagePath = "https://picsum.photos/seed/fences/400/600"
            },
            new Production
            {
                Title = "The Phantom Thread",
                Description = "A world premiere musical exploring the lives of seamstresses in 1920s London who uncover a conspiracy woven into the fabric of high society.",
                Playwright = "David Chen",
                Runtime = 135,
                OpeningDay = new DateTime(2025, 6, 20),
                ClosingDay = new DateTime(2025, 8, 5),
                ShowTimeEve = new DateTime(2025, 6, 20, 20, 0, 0),
                Season = 2025,
                IsWorldPremiere = true,
                TicketLink = "https://example.com/phantom-thread",
                PosterImagePath = "https://picsum.photos/seed/phantom/400/600"
            },
            new Production
            {
                Title = "Long Day's Journey into Night",
                Description = "Eugene O'Neill's autobiographical masterpiece follows the Tyrone family through one devastating day of addiction, blame, and buried love.",
                Playwright = "Eugene O'Neill",
                Runtime = 195,
                OpeningDay = new DateTime(2025, 1, 10),
                ClosingDay = new DateTime(2025, 2, 20),
                ShowTimeEve = new DateTime(2025, 1, 10, 19, 0, 0),
                Season = 2025,
                IsWorldPremiere = false,
                TicketLink = "https://example.com/longday",
                PosterImagePath = "https://picsum.photos/seed/longday/400/600"
            },
            new Production
            {
                Title = "Topdog/Underdog",
                Description = "Two brothers named Lincoln and Booth hustle to survive while grappling with identity, rivalry, and the weight of their names.",
                Playwright = "Suzan-Lori Parks",
                Runtime = 115,
                OpeningDay = new DateTime(2025, 4, 1),
                ClosingDay = new DateTime(2025, 5, 10),
                ShowTimeEve = new DateTime(2025, 4, 1, 20, 0, 0),
                Season = 2025,
                IsWorldPremiere = false,
                TicketLink = "https://example.com/topdog",
                PosterImagePath = "https://picsum.photos/seed/topdog/400/600"
            },
            new Production
            {
                Title = "Macbeth",
                Description = "Ambition and prophecy drive a Scottish general and his wife to murder and madness in Shakespeare's darkest tragedy.",
                Playwright = "William Shakespeare",
                Runtime = 160,
                OpeningDay = new DateTime(2025, 10, 20),
                ClosingDay = new DateTime(2025, 12, 1),
                ShowTimeEve = new DateTime(2025, 10, 20, 19, 30, 0),
                Season = 2025,
                IsWorldPremiere = false,
                TicketLink = "https://example.com/macbeth",
                PosterImagePath = "https://picsum.photos/seed/macbeth/400/600"
            },
            new Production
            {
                Title = "The Importance of Being Earnest",
                Description = "Oscar Wilde's sparkling comedy of manners where two gentlemen adopt fictitious identities to escape social obligations — with hilarious consequences.",
                Playwright = "Oscar Wilde",
                Runtime = 125,
                OpeningDay = new DateTime(2025, 7, 15),
                ClosingDay = new DateTime(2025, 8, 25),
                ShowTimeEve = new DateTime(2025, 7, 15, 19, 30, 0),
                Season = 2025,
                IsWorldPremiere = false,
                TicketLink = "https://example.com/earnest",
                PosterImagePath = "https://picsum.photos/seed/earnest/400/600"
            },
            new Production
            {
                Title = "Ember & Ash",
                Description = "A world premiere drama set after a wildfire destroys a small mountain town. Three families rebuild while confronting what they lost and what survived.",
                Playwright = "Maya Krishnan",
                Runtime = 130,
                OpeningDay = new DateTime(2025, 11, 10),
                ClosingDay = new DateTime(2025, 12, 20),
                ShowTimeEve = new DateTime(2025, 11, 10, 19, 0, 0),
                Season = 2025,
                IsWorldPremiere = true,
                TicketLink = "https://example.com/ember",
                PosterImagePath = "https://picsum.photos/seed/ember/400/600"
            },
            new Production
            {
                Title = "Cat on a Hot Tin Roof",
                Description = "Mendacity and desire collide in the Pollitt household as a wealthy Southern family confronts hidden truths during Big Daddy's birthday celebration.",
                Playwright = "Tennessee Williams",
                Runtime = 150,
                OpeningDay = new DateTime(2025, 8, 15),
                ClosingDay = new DateTime(2025, 9, 28),
                ShowTimeEve = new DateTime(2025, 8, 15, 20, 0, 0),
                Season = 2025,
                IsWorldPremiere = false,
                TicketLink = "https://example.com/cat",
                PosterImagePath = "https://picsum.photos/seed/hotcat/400/600"
            },
            new Production
            {
                Title = "Our Town",
                Description = "Life, love, and loss in the small fictional town of Grover's Corners. A meditation on the beauty of everyday existence.",
                Playwright = "Thornton Wilder",
                Runtime = 110,
                OpeningDay = new DateTime(2025, 5, 25),
                ClosingDay = new DateTime(2025, 7, 5),
                ShowTimeEve = new DateTime(2025, 5, 25, 19, 0, 0),
                Season = 2025,
                IsWorldPremiere = false,
                TicketLink = "https://example.com/ourtown",
                PosterImagePath = "https://picsum.photos/seed/ourtown/400/600"
            }
        };

        context.Productions.AddRange(productions);
        await context.SaveChangesAsync();
    }
}

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
