using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UserAPI.Core.Models;
using UserAPI.Data.Interface;
using UserAPI.DataAccess.Interface;
using BC = BCrypt.Net.BCrypt;

namespace UserAPI.Data.Implementation
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuthRepository> _logger;
        private readonly IConfiguration _configuration;
        public AuthRepository(IUnitOfWork unitOfWork, ILogger<AuthRepository> logger, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            const string sql = "select * from encriptacion.fun_register_user_simple(" +
                "p_username := @UserName," +
                "p_passwd := @Password" +
                ");";
            try
            {
                await _unitOfWork.EnsureConnectionAsync();

                await using var command = new NpgsqlCommand(sql, _unitOfWork.Connection, _unitOfWork.Transaction)
                {
                    CommandType = CommandType.Text
                };

                if (user.Password!.Trim() == "" && user.UserName!.Trim() == "")
                    throw new Exception("Formato inválido para la creación de usuario");

                string hashedPassword = BC.EnhancedHashPassword(user.Password);

                command.Parameters.AddWithValue("@UserName", user.UserName!);
                command.Parameters.AddWithValue("@Password", hashedPassword);

                await using var reader = await command.ExecuteReaderAsync();

                if (await  reader.ReadAsync())
                {
                    return new User
                    {
                        UserName = reader.GetString(reader.GetOrdinal("username")),
                        Password = reader.GetString(reader.GetOrdinal("passwd")),
                    };
                }
                return new User();
            }
            catch (PostgresException ex)
            {
                throw new Exception(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurs during register", ex);
            }
        }

        public string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse(_configuration["Jwt:ExpirationMinutes"]!)
                ),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
        public async Task<User> SignInAsync(string username, string password)
        {
            try
            {
                await _unitOfWork.EnsureConnectionAsync();
                var user = await this.FindUserByUserNameAsync(username);

                if (string.IsNullOrEmpty(user?.UserName))
                    throw new UnauthorizedAccessException("Las credenciales son incorrectas");

                if (!BC.EnhancedVerify(password, user.Password))
                    throw new UnauthorizedAccessException("Las credenciales son incorrectas");

                return user;
            }
            catch(UnauthorizedAccessException)
            {
                throw;
            }
            catch(PostgresException)
            {
                throw;
            }
            catch(Exception ex)
            {
                throw new Exception("An error occurs during login", ex);
            }
        }

        public Task SignOutAsync()
        {
            throw new NotImplementedException();
        }

        private async Task<User> FindUserByUserNameAsync(string username)
        {
            const string sql = "select * from encriptacion.fun_find_user(" +
                "p_username := @UserName" +
                ");";

            await using var command = new NpgsqlCommand(sql, _unitOfWork.Connection, _unitOfWork.Transaction)
            {
                CommandType = CommandType.Text,
            };

            command.Parameters.AddWithValue("@UserName", username);

            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new User
                {
                    UserName = reader.GetString(reader.GetOrdinal("username")),
                    Password = reader.GetString(reader.GetOrdinal("passwd"))
                };
            }
            return new User();
        }

    }
}
