using Microsoft.EntityFrameworkCore;
using StageCraft.Data; // ✅ your AppDbContext namespace

var builder = WebApplication.CreateBuilder(args);

// ✅ Register AppDbContext with MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 32)) // Adjust version if needed
    ));

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // ✅ This is needed to serve CSS/JS from wwwroot

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Productions}/{action=Index}/{id?}"); // 👈 Start with Productions by default

app.Run();
