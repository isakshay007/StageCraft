using Microsoft.EntityFrameworkCore;
using StageCraft.Models;

namespace StageCraft.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<Production> Productions { get; set; }
    }
}
