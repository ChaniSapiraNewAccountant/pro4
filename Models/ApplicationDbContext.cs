using Microsoft.EntityFrameworkCore;

namespace TodoApi.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Item> Items { get; set; } // הגדרת ה-DbSet של טבלת Items
    }
}
