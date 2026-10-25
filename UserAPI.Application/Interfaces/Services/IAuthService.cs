using UserAPI.Domain.Entities;

namespace UserAPI.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<User> SignInAsync(string username, string password);
        string GenerateToken(User user);
        Task<User> CreateUserAsync(User user);
        Task<List<User>> GetUsersAsync();
        Task<User> UpdateUserAsync(User user);
        Task<User> DeleteUserAsync(Guid Id);
        Task<User> GetUserByIdAsync(Guid Id);
    }
}
