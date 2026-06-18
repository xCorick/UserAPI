using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAPI.Core.Models;

namespace UserAPI.Data.Interface
{
    public interface IAuthRepository
    {
        Task<User> SignInAsync(string username, string password);
        Task SignOutAsync();
        string GenerateToken(User user);
        Task<User> CreateUserAsync(User user);
    }
}
