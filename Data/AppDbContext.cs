using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StageCraft.Models;

namespace StageCraft.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options) 
        { 
        }

        public DbSet<Production> Productions { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
    }
}
