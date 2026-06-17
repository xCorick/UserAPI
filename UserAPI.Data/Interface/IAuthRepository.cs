using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserAPI.Data.Interface
{
    public interface IAuthRepository
    {
        Task<string> SignInAsync(string username, string password);
        Task SignOutAsync();
    }
}
