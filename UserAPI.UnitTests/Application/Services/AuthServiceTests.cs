using Moq;
using UserAPI.Application.Common;
using UserAPI.Application.Common.Exceptions;
using UserAPI.Application.Interfaces.Repositories;
using UserAPI.Application.Interfaces.Security;
using UserAPI.Application.Services;
using UserAPI.Domain.Entities;
using UserAPI.Domain.Enums;

namespace UserAPI.UnitTests.Application.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _repository = new();
        private readonly Mock<IPasswordHasher> _passwordHasher = new();
        private readonly Mock<ITokenService> _tokenService = new();
        private readonly AuthService _service;

        public AuthServiceTests() =>
            _service = new AuthService(_repository.Object, _passwordHasher.Object, _tokenService.Object);

        [Fact]
        public async Task SignInAsync_WithValidCredentials_ReturnsUser()
        {
            var user = new User { Id = Guid.NewGuid(), UserName = "admin", Password = "hash", Rol = RoleEnum.ADMIN };
            _repository.Setup(r => r.FindUserByUserNameAsync("admin")).ReturnsAsync(user);
            _passwordHasher.Setup(h => h.Verify("secreto", "hash")).Returns(true);

            var result = await _service.SignInAsync("admin", "secreto");

            Assert.Same(user, result);
        }

        [Fact]
        public async Task SignInAsync_WithWrongPassword_ThrowsUnauthorized()
        {
            _repository.Setup(r => r.FindUserByUserNameAsync("admin"))
                .ReturnsAsync(new User { UserName = "admin", Password = "hash" });
            _passwordHasher.Setup(h => h.Verify("incorrecta", "hash")).Returns(false);

            var exception = await Assert.ThrowsAsync<UnauthorizedException>(() => _service.SignInAsync("admin", "incorrecta"));

            Assert.Equal("Las credenciales son incorrectas", exception.Message);
        }

        [Fact]
        public async Task SignInAsync_WhenUserIsNotFound_PropagatesPostgresError()
        {
            var postgresError = new BusinessRuleException(DatabaseErrorCodes.BusinessRule, "No existe el usuario fantasma");
            _repository.Setup(r => r.FindUserByUserNameAsync("fantasma")).ThrowsAsync(postgresError);

            var exception = await Assert.ThrowsAsync<BusinessRuleException>(() => _service.SignInAsync("fantasma", "x"));

            Assert.Same(postgresError, exception);
        }

        [Fact]
        public async Task SignInAsync_WhenPasswordCheckFails_WrapsError()
        {
            _repository.Setup(r => r.FindUserByUserNameAsync("admin"))
                .ReturnsAsync(new User { UserName = "admin", Password = "hash-corrupto" });
            _passwordHasher.Setup(h => h.Verify(It.IsAny<string>(), It.IsAny<string>())).Throws(new FormatException("Invalid salt"));

            var exception = await Assert.ThrowsAsync<UnexpectedException>(() => _service.SignInAsync("admin", "secreto"));

            Assert.Equal(ErrorMessages.Users.Login, exception.Message);
        }

        [Fact]
        public async Task CreateUserAsync_HashesPasswordBeforeSaving()
        {
            var user = new User { Name = "Ana", UserName = "ana", Password = "secreto" };
            _passwordHasher.Setup(h => h.Hash("secreto")).Returns("hash-de-secreto");

            await _service.CreateUserAsync(user);

            _repository.Verify(r => r.CreateUserAsync(It.Is<User>(u => u.Password == "hash-de-secreto")), Times.Once);
        }

        [Theory]
        [InlineData("ana", "")]
        [InlineData("ana", null)]
        [InlineData("  ", "secreto")]
        public async Task CreateUserAsync_WithBlankUserNameOrPassword_ThrowsWithoutSaving(string? userName, string? password)
        {
            var user = new User { Name = "Ana", UserName = userName, Password = password };

            var exception = await Assert.ThrowsAsync<ValidationException>(() => _service.CreateUserAsync(user));

            Assert.Equal("Formato inválido para la creación de usuario", exception.Message);
            _repository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task UpdateUserAsync_ResetsPasswordToUserNameFollowedBy123()
        {
            var user = new User { Id = Guid.NewGuid(), Name = "Ana", UserName = "ana" };
            _passwordHasher.Setup(h => h.Hash("ana123")).Returns("hash-de-ana123");

            await _service.UpdateUserAsync(user);

            _repository.Verify(r => r.UpdateUserAsync(It.Is<User>(u => u.Password == "hash-de-ana123")), Times.Once);
        }

        [Fact]
        public void GenerateToken_DelegatesToTokenService()
        {
            var user = new User { UserName = "admin" };
            _tokenService.Setup(t => t.GenerateToken(user)).Returns("jwt");

            Assert.Equal("jwt", _service.GenerateToken(user));
        }
    }
}
