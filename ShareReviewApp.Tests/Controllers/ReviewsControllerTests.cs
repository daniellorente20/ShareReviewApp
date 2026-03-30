using Microsoft.AspNetCore.Mvc;
using Moq;
using ShareReviewApp.Controllers;
using ShareReviewApp.DTOs.Reviews;
using ShareReviewApp.Exceptions;
using ShareReviewApp.Services;

namespace ShareReviewApp.Tests.Controllers;

public class ReviewsControllerTests
{
    private readonly Mock<ReviewService> _mockService;
    private readonly ReviewsController _controller;

    public ReviewsControllerTests()
    {
        _mockService = new Mock<ReviewService>(MockBehavior.Loose, null!, null!, null!);
        _controller = new ReviewsController(_mockService.Object);
    }

    [Fact]
    public async Task GetAll_Returns200_WithReviewList()
    {
        var reviews = new List<ReviewResponse>
        {
            new() { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Comment = "Good", Rating = 4, CreatedAt = DateTime.UtcNow }
        };
        _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(reviews);

        var result = await _controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(200, ok.StatusCode);
    }

    [Fact]
    public async Task GetById_Returns200_WhenFound()
    {
        var id = Guid.NewGuid();
        var review = new ReviewResponse { Id = id, UserId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Comment = "Good", Rating = 4, CreatedAt = DateTime.UtcNow };
        _mockService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(review);

        var result = await _controller.GetById(id);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(200, ok.StatusCode);
    }

    [Fact]
    public async Task GetById_Returns404_WhenNotFound()
    {
        _mockService.Setup(s => s.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((ReviewResponse?)null);

        var result = await _controller.GetById(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_Returns201_WhenValid()
    {
        var request = new CreateReviewRequest { UserId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Comment = "Excellent!", Rating = 5 };
        var response = new ReviewResponse { Id = Guid.NewGuid(), UserId = request.UserId, ProductId = request.ProductId, Comment = "Excellent!", Rating = 5, CreatedAt = DateTime.UtcNow };
        _mockService.Setup(s => s.CreateAsync(request)).ReturnsAsync(response);

        var result = await _controller.Create(request);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, created.StatusCode);
    }

    [Fact]
    public async Task Create_Returns409_WhenDuplicateReview()
    {
        var request = new CreateReviewRequest { UserId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Comment = "Again!", Rating = 3 };
        _mockService.Setup(s => s.CreateAsync(request)).ThrowsAsync(new DuplicateReviewException("Already reviewed."));

        var result = await _controller.Create(request);

        var conflict = Assert.IsType<ConflictObjectResult>(result.Result);
        Assert.Equal(409, conflict.StatusCode);
    }

    [Fact]
    public async Task Create_Returns404_WhenUserOrProductNotFound()
    {
        var request = new CreateReviewRequest { UserId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Comment = "Test", Rating = 3 };
        _mockService.Setup(s => s.CreateAsync(request)).ThrowsAsync(new NotFoundException("User not found."));

        var result = await _controller.Create(request);

        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal(404, notFound.StatusCode);
    }

    [Fact]
    public async Task GetByProductId_Returns200_WithFilteredReviews()
    {
        var productId = Guid.NewGuid();
        var reviews = new List<ReviewResponse>
        {
            new() { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), ProductId = productId, Comment = "Nice", Rating = 4, CreatedAt = DateTime.UtcNow }
        };
        _mockService.Setup(s => s.GetByProductIdAsync(productId)).ReturnsAsync(reviews);

        var result = await _controller.GetByProductId(productId);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(200, ok.StatusCode);
    }
}
