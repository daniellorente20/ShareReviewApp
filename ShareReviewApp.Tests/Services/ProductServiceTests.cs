using Moq;
using ShareReviewApp.DTOs.Products;
using ShareReviewApp.Models;
using ShareReviewApp.Repositories.Interfaces;
using ShareReviewApp.Services;

namespace ShareReviewApp.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockRepo;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _mockRepo = new Mock<IProductRepository>();
        _service = new ProductService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllProducts()
    {
        var products = new List<Product>
        {
            new() { Id = Guid.NewGuid(), Name = "IDE Pro", Description = "A great IDE.", Category = "Software", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "DevBoard", Description = "A keyboard.", Category = "Hardware", CreatedAt = DateTime.UtcNow }
        };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(products);

        var result = await _service.GetAllAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsProduct_WhenFound()
    {
        var id = Guid.NewGuid();
        var product = new Product { Id = id, Name = "IDE Pro", Description = "A great IDE.", Category = "Software", CreatedAt = DateTime.UtcNow };
        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(product);

        var result = await _service.GetByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal("IDE Pro", result.Name);
        Assert.Equal("Software", result.Category);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Product?)null);

        var result = await _service.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ReturnsCreatedProduct()
    {
        var request = new CreateProductRequest { Name = "New Tool", Description = "Useful.", Category = "Software" };
        _mockRepo
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) => p);

        var result = await _service.CreateAsync(request);

        Assert.NotNull(result);
        Assert.Equal("New Tool", result.Name);
        Assert.Equal("Software", result.Category);
        Assert.NotEqual(Guid.Empty, result.Id);
        _mockRepo.Verify(r => r.CreateAsync(It.IsAny<Product>()), Times.Once);
    }
}
