using Microsoft.EntityFrameworkCore;
using ShareReviewApp.Models;

namespace ShareReviewApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Unique constraint: one review per user per product
        // NOTE: InMemory provider does not enforce this at the DB level.
        // Enforcement is handled in ReviewService.CreateAsync via an explicit check.
        modelBuilder.Entity<Review>()
            .HasIndex(r => new { r.UserId, r.ProductId })
            .IsUnique();

        // Seed data
        var userId1 = new Guid("11111111-1111-1111-1111-111111111111");
        var userId2 = new Guid("22222222-2222-2222-2222-222222222222");
        var productId1 = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var productId2 = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<User>().HasData(
            new User { Id = userId1, Name = "Alice Smith", Email = "alice@example.com", CreatedAt = seedDate },
            new User { Id = userId2, Name = "Bob Jones", Email = "bob@example.com", CreatedAt = seedDate }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = productId1, Name = "CleanCode IDE", Description = "A powerful IDE for clean code lovers.", Category = "Software", CreatedAt = seedDate },
            new Product { Id = productId2, Name = "DevBoard Pro", Description = "Mechanical keyboard for developers.", Category = "Hardware", CreatedAt = seedDate }
        );

        modelBuilder.Entity<Review>().HasData(
            new Review
            {
                Id = new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                UserId = userId1,
                ProductId = productId1,
                Comment = "Excellent tool, highly recommend!",
                Rating = 5,
                CreatedAt = seedDate
            }
        );
    }
}
