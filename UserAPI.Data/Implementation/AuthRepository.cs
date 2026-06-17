using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAPI.Data.Interface;

namespace UserAPI.Data.Implementation
{
    internal class AuthRepository : IAuthRepository
    {
        public Task<string> SignInAsync(string username, string password)
        {
            throw new NotImplementedException();
        }

        public Task SignOutAsync()
        {
            throw new NotImplementedException();
        }
    }
}
