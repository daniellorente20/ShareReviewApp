using Moq;
using ShareReviewApp.DTOs.Reviews;
using ShareReviewApp.Exceptions;
using ShareReviewApp.Models;
using ShareReviewApp.Repositories.Interfaces;
using ShareReviewApp.Services;

namespace ShareReviewApp.Tests.Services;

public class ReviewServiceTests
{
    private readonly Mock<IReviewRepository> _mockReviewRepo;
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IProductRepository> _mockProductRepo;
    private readonly ReviewService _service;

    public ReviewServiceTests()
    {
        _mockReviewRepo = new Mock<IReviewRepository>();
        _mockUserRepo = new Mock<IUserRepository>();
        _mockProductRepo = new Mock<IProductRepository>();

        // Default: GetAllAsync returns empty list so ToDictionary never receives null
        _mockProductRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Product>());

        _service = new ReviewService(_mockReviewRepo.Object, _mockUserRepo.Object, _mockProductRepo.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllReviews_WithProductInfo()
    {
        var productId = Guid.NewGuid();
        var product = new Product { Id = productId, Name = "GitHub", Description = "", Category = "Development", CreatedAt = DateTime.UtcNow };
        var reviews = new List<Review>
        {
            new() { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), ProductId = productId, Comment = "Great!", Rating = 9, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), ProductId = productId, Comment = "Solid.", Rating = 8, CreatedAt = DateTime.UtcNow }
        };

        _mockReviewRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(reviews);
        _mockProductRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Product> { product });

        var result = await _service.GetAllAsync();

        Assert.Equal(2, result.Count());
        Assert.All(result, r => Assert.Equal("GitHub", r.ProductName));
        Assert.All(result, r => Assert.Equal("Development", r.ProductCategory));
    }

    [Fact]
    public async Task CreateAsync_ReturnsReview_WhenValid()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var request = new CreateReviewRequest { UserId = userId, ProductId = productId, Comment = "Great!", Rating = 5 };

        _mockUserRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(new User { Id = userId, Name = "Alice", Email = "a@test.com", CreatedAt = DateTime.UtcNow });
        _mockProductRepo.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(new Product { Id = productId, Name = "IDE", Description = "", Category = "Software", CreatedAt = DateTime.UtcNow });
        _mockReviewRepo.Setup(r => r.GetByUserAndProductAsync(userId, productId)).ReturnsAsync((Review?)null);
        _mockReviewRepo.Setup(r => r.CreateAsync(It.IsAny<Review>())).ReturnsAsync((Review rv) => rv);

        var result = await _service.CreateAsync(request);

        Assert.NotNull(result);
        Assert.Equal("Great!", result.Comment);
        Assert.Equal(5, result.Rating);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(productId, result.ProductId);
        _mockReviewRepo.Verify(r => r.CreateAsync(It.IsAny<Review>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ThrowsDuplicateReviewException_WhenAlreadyReviewed()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var request = new CreateReviewRequest { UserId = userId, ProductId = productId, Comment = "Again!", Rating = 3 };

        _mockUserRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(new User { Id = userId, Name = "Alice", Email = "a@test.com", CreatedAt = DateTime.UtcNow });
        _mockProductRepo.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(new Product { Id = productId, Name = "IDE", Description = "", Category = "Software", CreatedAt = DateTime.UtcNow });
        _mockReviewRepo.Setup(r => r.GetByUserAndProductAsync(userId, productId)).ReturnsAsync(new Review { Id = Guid.NewGuid(), UserId = userId, ProductId = productId, Comment = "First", Rating = 4, CreatedAt = DateTime.UtcNow });

        await Assert.ThrowsAsync<DuplicateReviewException>(() => _service.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_ThrowsArgumentException_WhenUserNotFound()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var request = new CreateReviewRequest { UserId = userId, ProductId = productId, Comment = "Test", Rating = 3 };

        _mockUserRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_ThrowsArgumentException_WhenProductNotFound()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var request = new CreateReviewRequest { UserId = userId, ProductId = productId, Comment = "Test", Rating = 3 };

        _mockUserRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(new User { Id = userId, Name = "Alice", Email = "a@test.com", CreatedAt = DateTime.UtcNow });
        _mockProductRepo.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateAsync(request));
    }

    [Fact]
    public async Task GetByProductIdAsync_ReturnsFilteredReviews()
    {
        var productId = Guid.NewGuid();
        var reviews = new List<Review>
        {
            new() { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), ProductId = productId, Comment = "Good", Rating = 4, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), ProductId = productId, Comment = "Excellent", Rating = 5, CreatedAt = DateTime.UtcNow }
        };
        _mockReviewRepo.Setup(r => r.GetByProductIdAsync(productId)).ReturnsAsync(reviews);
        _mockProductRepo.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync((Product?)null);

        var result = await _service.GetByProductIdAsync(productId);

        Assert.Equal(2, result.Count());
        Assert.All(result, r => Assert.Equal(productId, r.ProductId));
    }

    [Fact]
    public async Task VoteHelpfulAsync_IncrementsHelpfulCount_WhenReviewExists()
    {
        var reviewId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var review = new Review { Id = reviewId, UserId = Guid.NewGuid(), ProductId = productId, Comment = "Great!", Rating = 8, HelpfulCount = 2, CreatedAt = DateTime.UtcNow };

        _mockReviewRepo.Setup(r => r.GetByIdAsync(reviewId)).ReturnsAsync(review);
        _mockReviewRepo.Setup(r => r.UpdateAsync(It.IsAny<Review>())).ReturnsAsync((Review r) => r);
        _mockProductRepo.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync((Product?)null);

        var result = await _service.VoteHelpfulAsync(reviewId);

        Assert.NotNull(result);
        Assert.Equal(3, result.HelpfulCount);
        _mockReviewRepo.Verify(r => r.UpdateAsync(It.Is<Review>(rv => rv.HelpfulCount == 3)), Times.Once);
    }

    [Fact]
    public async Task VoteHelpfulAsync_ReturnsNull_WhenReviewNotFound()
    {
        var reviewId = Guid.NewGuid();
        _mockReviewRepo.Setup(r => r.GetByIdAsync(reviewId)).ReturnsAsync((Review?)null);

        var result = await _service.VoteHelpfulAsync(reviewId);

        Assert.Null(result);
        _mockReviewRepo.Verify(r => r.UpdateAsync(It.IsAny<Review>()), Times.Never);
    }

    [Fact]
    public async Task VoteHelpfulAsync_IncludesProductInfo_InResponse()
    {
        var reviewId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var review = new Review { Id = reviewId, UserId = Guid.NewGuid(), ProductId = productId, Comment = "Solid", Rating = 7, HelpfulCount = 0, CreatedAt = DateTime.UtcNow };
        var product = new Product { Id = productId, Name = "GitHub", Description = "", Category = "Development", CreatedAt = DateTime.UtcNow };

        _mockReviewRepo.Setup(r => r.GetByIdAsync(reviewId)).ReturnsAsync(review);
        _mockReviewRepo.Setup(r => r.UpdateAsync(It.IsAny<Review>())).ReturnsAsync((Review r) => r);
        _mockProductRepo.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);

        var result = await _service.VoteHelpfulAsync(reviewId);

        Assert.NotNull(result);
        Assert.Equal("GitHub", result.ProductName);
        Assert.Equal("Development", result.ProductCategory);
    }
}
