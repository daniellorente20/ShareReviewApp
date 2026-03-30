using Microsoft.AspNetCore.Mvc;
using Moq;
using ShareReviewApp.Controllers;
using ShareReviewApp.DTOs.Products;
using ShareReviewApp.Services;

namespace ShareReviewApp.Tests.Controllers;

public class ProductsControllerTests
{
    private readonly Mock<ProductService> _mockService;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _mockService = new Mock<ProductService>(MockBehavior.Loose, null!);
        _controller = new ProductsController(_mockService.Object);
    }

    [Fact]
    public async Task GetAll_Returns200_WithProductList()
    {
        var products = new List<ProductResponse>
        {
            new() { Id = Guid.NewGuid(), Name = "IDE Pro", Description = "A great IDE.", Category = "Software", CreatedAt = DateTime.UtcNow }
        };
        _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(products);

        var result = await _controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(200, ok.StatusCode);
    }

    [Fact]
    public async Task GetById_Returns200_WhenFound()
    {
        var id = Guid.NewGuid();
        var product = new ProductResponse { Id = id, Name = "IDE Pro", Description = "A great IDE.", Category = "Software", CreatedAt = DateTime.UtcNow };
        _mockService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(product);

        var result = await _controller.GetById(id);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(200, ok.StatusCode);
    }

    [Fact]
    public async Task GetById_Returns404_WhenNotFound()
    {
        _mockService.Setup(s => s.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((ProductResponse?)null);

        var result = await _controller.GetById(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_Returns201_WithCreatedProduct()
    {
        var request = new CreateProductRequest { Name = "New Tool", Description = "Useful.", Category = "Software" };
        var response = new ProductResponse { Id = Guid.NewGuid(), Name = "New Tool", Description = "Useful.", Category = "Software", CreatedAt = DateTime.UtcNow };
        _mockService.Setup(s => s.CreateAsync(request)).ReturnsAsync(response);

        var result = await _controller.Create(request);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, created.StatusCode);
    }
}
