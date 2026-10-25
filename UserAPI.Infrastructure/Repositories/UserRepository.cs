using System.Data;
using System.Data.Common;
using UserAPI.Application.Common;
using UserAPI.Application.Interfaces.Repositories;
using UserAPI.Domain.Entities;
using UserAPI.Domain.Enums;
using UserAPI.Infrastructure.Persistence.Sql;

namespace UserAPI.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ISqlExecutor _sqlExecutor;

        public UserRepository(ISqlExecutor sqlExecutor) => _sqlExecutor = sqlExecutor;

        public Task<User> CreateUserAsync(User user)
        {
            const string sql = "select * from encriptacion.fun_register_user_simple(" +
                "p_name := @Name," +
                "p_username := @UserName," +
                "p_passwd := @Password," +
                "p_rol := @Rol" +
                ");";

            return RepositoryGuard.ExecuteAsync(ErrorMessages.Users.Create, async () =>
            {
                var statement = new SqlStatement(sql,
                    new SqlParam("@Name", user.Name),
                    new SqlParam("@UserName", user.UserName),
                    new SqlParam("@Password", user.Password),
                    new SqlParam("@Rol", (short)user.Rol));

                return await _sqlExecutor.QueryFirstOrDefaultAsync(statement, reader => new User
                {
                    Name = reader.GetString("name"),
                    UserName = reader.GetString("username"),
                    Password = reader.GetString("passwd"),
                    Rol = (RoleEnum)reader.GetInt16("rol")
                }) ?? new User();
            });
        }

        public Task<User> DeleteUserAsync(Guid Id)
        {
            const string sql = "select * from encriptacion.soft_delete_user(" +
                "p_id := @Id" +
                ");";

            return RepositoryGuard.ExecuteAsync(ErrorMessages.Users.Delete, async () =>
            {
                var statement = new SqlStatement(sql, new SqlParam("@Id", Id));

                return await _sqlExecutor.QueryFirstOrDefaultAsync(statement, MapUser) ?? new User();
            });
        }

        public Task<User> GetUserByIdAsync(Guid Id)
        {
            const string sql = "select * from encriptacion.get_user_by_id(" +
               "p_id := @Id" +
               ");";

            return RepositoryGuard.ExecuteAsync(ErrorMessages.Users.GetById, async () =>
            {
                var statement = new SqlStatement(sql, new SqlParam("@Id", Id));

                // get_user_by_id devuelve una fila con columnas nulas cuando no encuentra el usuario
                return await _sqlExecutor.QueryFirstOrDefaultAsync(statement,
                    reader => reader.IsDBNull("id") ? new User() : MapUser(reader)) ?? new User();
            });
        }

        public Task<List<User>> GetUsersAsync()
        {
            const string sql = "select * from encriptacion.get_users();";

            return RepositoryGuard.ExecuteAsync(ErrorMessages.Users.GetAll,
                () => _sqlExecutor.QueryAsync(new SqlStatement(sql), MapUser));
        }

        public Task<User> UpdateUserAsync(User user)
        {
            const string sql = "select * from encriptacion.update_user(" +
                "p_id := @Id," +
                "p_name := @Name," +
                "p_username := @UserName," +
                "p_passwd := @Password," +
                "p_rol := @Rol" +
                ");";

            return RepositoryGuard.ExecuteAsync(ErrorMessages.Users.Update, async () =>
            {
                var statement = new SqlStatement(sql,
                    new SqlParam("@Id", user.Id),
                    new SqlParam("@Name", user.Name),
                    new SqlParam("@UserName", user.UserName),
                    new SqlParam("@Password", user.Password),
                    new SqlParam("@Rol", (short)user.Rol));

                return await _sqlExecutor.QueryFirstOrDefaultAsync(statement, MapUser) ?? new User();
            });
        }

        public Task<User> FindUserByUserNameAsync(string username)
        {
            const string sql = "select * from encriptacion.fun_find_user(" +
                "p_username := @UserName" +
                ");";

            return RepositoryGuard.ExecuteAsync(ErrorMessages.Users.Login, async () =>
            {
                var statement = new SqlStatement(sql, new SqlParam("@UserName", username));

                return await _sqlExecutor.QueryFirstOrDefaultAsync(statement, reader => new User
                {
                    Id = reader.GetGuid("id"),
                    UserName = reader.GetString("username"),
                    Password = reader.GetString("passwd"),
                    Rol = (RoleEnum)reader.GetInt16("rol")
                }) ?? new User();
            });
        }

        private static User MapUser(DbDataReader reader) => new()
        {
            Id = reader.GetGuid("id"),
            Name = reader.GetString("name"),
            UserName = reader.GetString("username"),
            Rol = (RoleEnum)reader.GetInt16("rol")
        };
    }
}
