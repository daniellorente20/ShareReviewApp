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

        var userId1 = new Guid("11111111-1111-1111-1111-111111111111");
        var userId2 = new Guid("22222222-2222-2222-2222-222222222222");
        var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // --- Users ---
        modelBuilder.Entity<User>().HasData(
            new User { Id = userId1, Name = "Alice Smith", Email = "alice@example.com", CreatedAt = seedDate },
            new User { Id = userId2, Name = "Bob Jones", Email = "bob@example.com", CreatedAt = seedDate }
        );

        // --- Products (8 real software tools) ---
        var pGitHub   = new Guid("00000001-0000-0000-0000-000000000000");
        var pSlack    = new Guid("00000002-0000-0000-0000-000000000000");
        var pNotion   = new Guid("00000003-0000-0000-0000-000000000000");
        var pFigma    = new Guid("00000004-0000-0000-0000-000000000000");
        var pVSCode   = new Guid("00000005-0000-0000-0000-000000000000");
        var pLinear   = new Guid("00000006-0000-0000-0000-000000000000");
        var pVercel   = new Guid("00000007-0000-0000-0000-000000000000");
        var pPostman  = new Guid("00000008-0000-0000-0000-000000000000");

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = pGitHub,  Name = "GitHub",   Category = "Development",        Description = "The world's leading platform for version control and collaboration. Host, review, and manage code at scale.",           CreatedAt = seedDate },
            new Product { Id = pSlack,   Name = "Slack",    Category = "Collaboration",       Description = "Messaging platform built for teams. Channels, threads, and integrations that keep everyone aligned.",                 CreatedAt = seedDate },
            new Product { Id = pNotion,  Name = "Notion",   Category = "Productivity",        Description = "All-in-one workspace for notes, wikis, databases, and project management. Highly customisable and flexible.",         CreatedAt = seedDate },
            new Product { Id = pFigma,   Name = "Figma",    Category = "Design",              Description = "Browser-based UI design and prototyping tool. Real-time collaboration makes it the standard for product teams.",      CreatedAt = seedDate },
            new Product { Id = pVSCode,  Name = "VS Code",  Category = "Development",        Description = "Free, open-source code editor from Microsoft. Huge extension ecosystem and excellent TypeScript/JS support.",         CreatedAt = seedDate },
            new Product { Id = pLinear,  Name = "Linear",   Category = "Project Management", Description = "Issue tracking and project management tool built for speed. Loved by engineering teams for its clean UX.",            CreatedAt = seedDate },
            new Product { Id = pVercel,  Name = "Vercel",   Category = "DevOps",             Description = "Frontend cloud platform. Deploy React, Next.js, and other frameworks with zero config and instant previews.",         CreatedAt = seedDate },
            new Product { Id = pPostman, Name = "Postman",  Category = "Development",        Description = "API development and testing platform. Build, test, and document APIs collaboratively with a visual interface.",       CreatedAt = seedDate }
        );

        // --- Reviews (distributed across users and products) ---
        modelBuilder.Entity<Review>().HasData(
            new Review { Id = new Guid("cc000001-0000-0000-0000-000000000000"), UserId = userId1, ProductId = pGitHub,  Rating = 10, Comment = "Absolutely essential for any dev team. Pull requests and code reviews are seamless.",    CreatedAt = seedDate },
            new Review { Id = new Guid("cc000002-0000-0000-0000-000000000000"), UserId = userId2, ProductId = pGitHub,  Rating = 9,  Comment = "Best version control platform out there. Actions for CI/CD is a game changer.",          CreatedAt = new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc) },
            new Review { Id = new Guid("cc000003-0000-0000-0000-000000000000"), UserId = userId1, ProductId = pFigma,   Rating = 9,  Comment = "Transformed how our design team works. Real-time collaboration is mind-blowing.",         CreatedAt = new DateTime(2024, 1, 3, 0, 0, 0, DateTimeKind.Utc) },
            new Review { Id = new Guid("cc000004-0000-0000-0000-000000000000"), UserId = userId2, ProductId = pVSCode,  Rating = 10, Comment = "The best free editor available. Extensions for everything you could possibly need.",      CreatedAt = new DateTime(2024, 1, 4, 0, 0, 0, DateTimeKind.Utc) },
            new Review { Id = new Guid("cc000005-0000-0000-0000-000000000000"), UserId = userId1, ProductId = pSlack,   Rating = 7,  Comment = "Great for team communication but can be distracting. Notification management is key.",   CreatedAt = new DateTime(2024, 1, 5, 0, 0, 0, DateTimeKind.Utc) },
            new Review { Id = new Guid("cc000006-0000-0000-0000-000000000000"), UserId = userId2, ProductId = pLinear,  Rating = 9,  Comment = "Finally a project management tool that doesn't feel like a chore. Fast and beautiful.",  CreatedAt = new DateTime(2024, 1, 6, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
