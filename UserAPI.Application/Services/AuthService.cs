using UserAPI.Application.Common;
using UserAPI.Application.Common.Exceptions;
using UserAPI.Application.Interfaces.Repositories;
using UserAPI.Application.Interfaces.Security;
using UserAPI.Application.Interfaces.Services;
using UserAPI.Domain.Entities;

namespace UserAPI.Application.Services
{
    public class AuthService : IAuthService
    {
        private const string DefaultPassword = "123456789";

        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;

        public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public async Task<User> SignInAsync(string username, string password)
        {
            try
            {
                var user = await _userRepository.FindUserByUserNameAsync(username);

                if (string.IsNullOrEmpty(user?.UserName) || !_passwordHasher.Verify(password, user.Password!))
                    throw new UnauthorizedException(ErrorMessages.Users.InvalidCredentials);

                return user;
            }
            catch (AppException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new UnexpectedException(ErrorMessages.Users.Login, ex);
            }
        }

        public string GenerateToken(User user) => _tokenService.GenerateToken(user);

        public async Task<User> CreateUserAsync(User user)
        {
            if (string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.Password))
                throw new ValidationException(ErrorMessages.Users.InvalidFormat);

            user.Password = _passwordHasher.Hash(user.Password);
            return await _userRepository.CreateUserAsync(user);
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            // Al actualizar, la contraseña se restablece a "{username}123"
            var password = string.IsNullOrEmpty(user.UserName) ? DefaultPassword : $"{user.UserName}123";
            user.Password = _passwordHasher.Hash(password);
            return await _userRepository.UpdateUserAsync(user);
        }

        public Task<User> DeleteUserAsync(Guid Id) => _userRepository.DeleteUserAsync(Id);

        public Task<User> GetUserByIdAsync(Guid Id) => _userRepository.GetUserByIdAsync(Id);

        public Task<List<User>> GetUsersAsync() => _userRepository.GetUsersAsync();
    }
}
