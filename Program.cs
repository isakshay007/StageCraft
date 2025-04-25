using Microsoft.EntityFrameworkCore;
using StageCraft.Data;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 32))
    ));

// ✅ Important: Use AddControllersWithViews without AddMvc (which can cause duplicates)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// ✅ Simplified endpoint routing (only one mapping)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Productions}/{action=Index}/{id?}");

app.Run();