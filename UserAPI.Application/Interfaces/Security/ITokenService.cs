using UserAPI.Domain.Entities;

namespace UserAPI.Application.Interfaces.Security
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
