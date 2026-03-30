using ShareReviewApp.Models;

namespace ShareReviewApp.Repositories.Interfaces;

public interface IReviewRepository
{
    Task<IEnumerable<Review>> GetAllAsync();
    Task<Review?> GetByIdAsync(Guid id);
    Task<Review> CreateAsync(Review review);
    Task<IEnumerable<Review>> GetByProductIdAsync(Guid productId);
    Task<Review?> GetByUserAndProductAsync(Guid userId, Guid productId);
}
