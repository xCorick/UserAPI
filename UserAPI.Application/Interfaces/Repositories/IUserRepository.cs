using UserAPI.Domain.Entities;

namespace UserAPI.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(User user);
        Task<User> DeleteUserAsync(Guid Id);
        Task<User> GetUserByIdAsync(Guid Id);
        Task<List<User>> GetUsersAsync();
        Task<User> FindUserByUserNameAsync(string username);
    }
}
