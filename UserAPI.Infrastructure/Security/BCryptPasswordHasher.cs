using UserAPI.Application.Interfaces.Security;
using BC = BCrypt.Net.BCrypt;

namespace UserAPI.Infrastructure.Security
{
    public class BCryptPasswordHasher : IPasswordHasher
    {
        public string Hash(string password) => BC.EnhancedHashPassword(password);

        public bool Verify(string password, string hashedPassword) => BC.EnhancedVerify(password, hashedPassword);
    }
}
