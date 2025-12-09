using Microsoft.EntityFrameworkCore;

namespace LindisBakery.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // When you create models, you will add them here as DbSets.
        // Example:
        // public DbSet<Product> Products { get; set; }
    }
}
