using ShareReviewApp.DTOs.Reviews;
using ShareReviewApp.Exceptions;
using ShareReviewApp.Models;
using ShareReviewApp.Repositories.Interfaces;

namespace ShareReviewApp.Services;

public class ReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProductRepository _productRepository;

    public ReviewService(
        IReviewRepository reviewRepository,
        IUserRepository userRepository,
        IProductRepository productRepository)
    {
        _reviewRepository = reviewRepository;
        _userRepository = userRepository;
        _productRepository = productRepository;
    }

    public virtual async Task<IEnumerable<ReviewResponse>> GetAllAsync()
    {
        var reviews = await _reviewRepository.GetAllAsync();
        var products = (await _productRepository.GetAllAsync())
            .ToDictionary(p => p.Id);

        return reviews.Select(r => MapToResponse(r, products.GetValueOrDefault(r.ProductId)));
    }

    public virtual async Task<ReviewResponse?> GetByIdAsync(Guid id)
    {
        var review = await _reviewRepository.GetByIdAsync(id);
        if (review is null) return null;

        var product = await _productRepository.GetByIdAsync(review.ProductId);
        return MapToResponse(review, product);
    }

    public virtual async Task<ReviewResponse> CreateAsync(CreateReviewRequest request)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user is null)
            throw new NotFoundException($"User with id '{request.UserId}' was not found.");

        var product = await _productRepository.GetByIdAsync(request.ProductId);
        if (product is null)
            throw new NotFoundException($"Product with id '{request.ProductId}' was not found.");

        var existing = await _reviewRepository.GetByUserAndProductAsync(request.UserId, request.ProductId);
        if (existing is not null)
            throw new DuplicateReviewException($"User '{request.UserId}' has already reviewed product '{request.ProductId}'.");

        var review = new Review
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            ProductId = request.ProductId,
            Comment = request.Comment,
            Rating = request.Rating,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _reviewRepository.CreateAsync(review);
        return MapToResponse(created, product);
    }

    public virtual async Task<IEnumerable<ReviewResponse>> GetByProductIdAsync(Guid productId)
    {
        var reviews = await _reviewRepository.GetByProductIdAsync(productId);
        var product = await _productRepository.GetByIdAsync(productId);
        return reviews.Select(r => MapToResponse(r, product));
    }

    private static ReviewResponse MapToResponse(Review review, Product? product) => new()
    {
        Id = review.Id,
        UserId = review.UserId,
        ProductId = review.ProductId,
        ProductName = product?.Name ?? string.Empty,
        ProductCategory = product?.Category ?? string.Empty,
        Comment = review.Comment,
        Rating = review.Rating,
        CreatedAt = review.CreatedAt
    };
}
