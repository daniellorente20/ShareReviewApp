using ShareReviewApp.Models;

namespace ShareReviewApp.Repositories.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(Guid id);
    Task<User> CreateAsync(User user);
}
