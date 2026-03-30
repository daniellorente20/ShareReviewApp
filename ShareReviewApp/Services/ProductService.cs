using ShareReviewApp.DTOs.Products;
using ShareReviewApp.Models;
using ShareReviewApp.Repositories.Interfaces;

namespace ShareReviewApp.Services;

public class ProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public virtual async Task<IEnumerable<ProductResponse>> GetAllAsync()
    {
        var products = await _repository.GetAllAsync();
        return products.Select(MapToResponse);
    }

    public virtual async Task<ProductResponse?> GetByIdAsync(Guid id)
    {
        var product = await _repository.GetByIdAsync(id);
        return product is null ? null : MapToResponse(product);
    }

    public virtual async Task<ProductResponse> CreateAsync(CreateProductRequest request)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Category = request.Category,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(product);
        return MapToResponse(created);
    }

    private static ProductResponse MapToResponse(Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Description = product.Description,
        Category = product.Category,
        CreatedAt = product.CreatedAt
    };
}
