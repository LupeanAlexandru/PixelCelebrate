using Microsoft.EntityFrameworkCore;
using PixelCelebrate.Models;

namespace PixelCelebrate.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<Birthday> Birthdays { get; set; }
    }
}
  