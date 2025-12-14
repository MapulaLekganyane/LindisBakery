using LindisBakery.Models;
using Microsoft.EntityFrameworkCore;

namespace LindisBakery.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed sample data
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Blueberry Muffins",
                    Description = "Plump, local blueberries burst in every bite of these moist, golden-topped muffins.",
                    Price = 8.99m,
                    ImageUrl = "/images/packet.jpeg",
                    Category = "Muffins",
                    IsAvailable = true
                },
                new Product
                {
                    Id = 2,
                    Name = "Dumplings",
                    Description = "Traditional, supremely tender scones made with pure Devon cream for the perfect crumb.",
                    Price = 12.99m,
                    ImageUrl = "/images/packet.jpeg",
                    Category = "Scones",
                    IsAvailable = true
                },
                new Product
                {
                    Id = 3,
                    Name = "Chocolate Muffins",
                    Description = "Rich, dark cocoa batter loaded with premium Belgian chocolate chunks.",
                    Price = 7.99m,
                    ImageUrl = "/images/blueberry.jpeg",
                    Category = "Dumplings",
                    IsAvailable = true
                },
                new Product
                {
                    Id = 4,
                    Name = "Vanilla Muffins",
                    Description = "Buttery, flaky scones studded with wild blueberries and a bright hint of lemon zest.",
                    Price = 5.99m,
                    ImageUrl = "/images/blueberry.jpeg",
                    Category = "Cupcakes",
                    IsAvailable = true
                },
                new Product
                {
                    Id = 5,
                    Name = "Chocolate Mint Muffins",
                    Description = "Cold brewed coffee with ice and milk",
                    Price = 3.99m,
                    ImageUrl = "/images/blueberry.jpeg",
                    Category = "Muffins",
                    IsAvailable = true
                }
            );
        }
    }
}