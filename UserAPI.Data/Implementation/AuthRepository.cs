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
using UserAPI.Core;
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
                "p_name := @Name," +
                "p_username := @UserName," +
                "p_passwd := @Password," +
                "p_rol := @Rol"+
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

                command.Parameters.AddWithValue("@Name", user.Name!);
                command.Parameters.AddWithValue("@UserName", user.UserName!);
                command.Parameters.AddWithValue("@Password", hashedPassword);
                command.Parameters.AddWithValue("@Rol", (short)user.Rol);

                await using var reader = await command.ExecuteReaderAsync();

                if (await  reader.ReadAsync())
                {
                    return new User
                    {
                        Name = reader.GetString("name"),
                        UserName = reader.GetString(reader.GetOrdinal("username")),
                        Password = reader.GetString(reader.GetOrdinal("passwd")),
                        Rol = (RoleEnum)reader.GetInt16("rol")
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
                throw new Exception("An error occurs during register user", ex);
            }
        }

        public async Task<User> DeleteUserAsync(Guid Id)
        {
            const string sql = "select * from encriptacion.soft_delete_user(" +
                "p_id := @Id" +
                ");";
            try
            {
                await _unitOfWork.EnsureConnectionAsync();

                await using var command = new NpgsqlCommand(sql, _unitOfWork.Connection, _unitOfWork.Transaction)
                {
                    CommandType = CommandType.Text
                };

                command.Parameters.AddWithValue("@Id", Id);

                await using var reader = await command.ExecuteReaderAsync();

                if(await reader.ReadAsync())
                {
                    return new User
                    {
                        Id = reader.GetGuid("id"),
                        Name = reader.GetString("name"),
                        UserName = reader.GetString("username"),
                        Rol = (RoleEnum)reader.GetInt16("rol")
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
                throw new Exception("An error occurs during delete user", ex);
            }
        }

        public async Task<User> GetUserByIdAsync(Guid Id)
        {
            const string sql = "select * from encriptacion.get_user_by_id(" +
               "p_id := @Id" +
               ");";
            try
            {
                await _unitOfWork.EnsureConnectionAsync();

                await using var command = new NpgsqlCommand(sql, _unitOfWork.Connection, _unitOfWork.Transaction)
                {
                    CommandType = CommandType.Text
                };

                command.Parameters.AddWithValue("@Id", Id);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    if (reader.IsDBNull("id"))
                    {
                        return new User();
                    }
                    return new User
                    {
                        Id = reader.GetGuid("id"),
                        Name = reader.GetString("name"),
                        UserName = reader.GetString("username"),
                        Rol = (RoleEnum)reader.GetInt16("rol")
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
                throw new Exception("An error occurs during getting user", ex);
            }
        }

        public string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Role, user.Rol.ToString())
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

        public async Task<List<User>> GetUsersAsync()
        {
            const string sql = "select * from encriptacion.get_users();";
            var users = new List<User>();
            try
            {
                await _unitOfWork.EnsureConnectionAsync();

                await using var command = new NpgsqlCommand(sql, _unitOfWork.Connection, _unitOfWork.Transaction)
                {
                    CommandType = CommandType.Text
                };

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    users.Add(new User
                    {
                        Id = reader.GetGuid("id"),
                        Name = reader.GetString("name"),
                        UserName = reader.GetString("username"),
                        Rol = (RoleEnum)reader.GetInt16("rol")
                    });
                }
                return users;
            }
            catch (PostgresException ex)
            {
                throw new Exception(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurs during getting user", ex);
            }
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

        public async Task<User> UpdateUserAsync(User user)
        {
            const string sql = "select * from encriptacion.update_user(" +
                "p_id := @Id," +
                "p_name := @Name," +
                "p_username := @UserName," +
                "p_passwd := @Password," +
                "p_rol := @Rol" +
                ");";

            user.Password = GeneratePasswordByUserName(user.UserName!);
            try
            {
                await _unitOfWork.EnsureConnectionAsync();

                await using var command = new NpgsqlCommand(sql, _unitOfWork.Connection, _unitOfWork.Transaction)
                {
                    CommandType = CommandType.Text
                };

                command.Parameters.AddWithValue("@Id", user.Id);
                command.Parameters.AddWithValue("@Name", user.Name!);
                command.Parameters.AddWithValue("@UserName", user.UserName!);
                command.Parameters.AddWithValue("@Password", user.Password!);
                command.Parameters.AddWithValue("@Rol", (short)user.Rol);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new User
                    {
                        Id = reader.GetGuid("id"),
                        Name = reader.GetString("name"),
                        UserName = reader.GetString("username"),
                        Rol = (RoleEnum)reader.GetInt16("rol")
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
                throw new Exception("An error occurs during updating user", ex);
            }
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

        private string? GeneratePasswordByUserName(string username)
        {
            if (username.IsNullOrEmpty())
                return BC.EnhancedHashPassword("123456789");

            string password = $"{username}123";

            string hashedPassword = BC.EnhancedHashPassword(password);

            return hashedPassword;
        }

    }
}
