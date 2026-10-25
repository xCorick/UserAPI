using System.Data;
using System.Data.Common;
using UserAPI.Application.Common;
using UserAPI.Application.DTOs;
using UserAPI.Application.Interfaces.Repositories;
using UserAPI.Domain.Entities;
using UserAPI.Infrastructure.Persistence.Sql;

namespace UserAPI.Infrastructure.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly ISqlExecutor _sqlExecutor;

        public ClientRepository(ISqlExecutor sqlExecutor) => _sqlExecutor = sqlExecutor;

        #region CreateAsync
        public Task<Cliente> CreateAsync(ClienteDTO cliente)
        {
            const string sql = "select * from encriptacion.insert_client(" +
                "p_clave := @Clave," +
                "p_nombre := @Nombre," +
                "p_edad := @Edad," +
                "p_fecha_nacimiento := @FechaNacimiento" +
                ");";

            return RepositoryGuard.ExecuteAsync(ErrorMessages.Clients.Create, async () =>
            {
                var statement = new SqlStatement(sql,
                    new SqlParam("@Clave", cliente.Clave),
                    new SqlParam("@Nombre", cliente.Nombre),
                    new SqlParam("@Edad", cliente.Edad),
                    new SqlParam("@FechaNacimiento", cliente.Fecha_Nacimiento));

                return await _sqlExecutor.QueryFirstOrDefaultAsync(statement, MapCliente) ?? new Cliente();
            });
        }
        #endregion
        #region DeleteAsync
        public Task<Cliente> DeleteAsync(string clave)
        {
            const string sql = "select * from encriptacion.delete_client(" +
                "p_clave := @Clave" +
                ");";

            return RepositoryGuard.ExecuteAsync(ErrorMessages.Clients.Delete, async () =>
            {
                var statement = new SqlStatement(sql, new SqlParam("@Clave", clave));

                return await _sqlExecutor.QueryFirstOrDefaultAsync(statement, MapCliente) ?? new Cliente();
            });
        }
        #endregion
        #region GetClienteAsync
        public Task<Cliente> GetClienteAsync(string clave)
        {
            const string sql = "select * from encriptacion.get_client(" +
                "p_clave := @Clave" +
                ");";

            return RepositoryGuard.ExecuteAsync(ErrorMessages.Clients.GetByClave, async () =>
            {
                var statement = new SqlStatement(sql, new SqlParam("@Clave", clave));

                // get_client devuelve una fila con columnas nulas cuando no encuentra el cliente
                return await _sqlExecutor.QueryFirstOrDefaultAsync(statement,
                    reader => reader.IsDBNull("clave") ? new Cliente() : MapCliente(reader)) ?? new Cliente();
            });
        }
        #endregion
        #region GetClientesAsync
        public Task<List<Cliente>> GetClientesAsync()
        {
            const string sql = "select * from encriptacion.get_clients()";

            return RepositoryGuard.ExecuteAsync(ErrorMessages.Clients.GetAll,
                () => _sqlExecutor.QueryAsync(new SqlStatement(sql), MapCliente));
        }
        #endregion
        #region GetClientesPageAsync
        public Task<List<Cliente>> GetClientesPageAsync(PaginationFilter pagination)
        {
            const string sql = "select * from encriptacion.get_clients_page(" +
                "p_page := @Page," +
                "p_page_size := @PageSize" +
                ");";

            return RepositoryGuard.ExecuteAsync(ErrorMessages.Clients.GetPage, () =>
            {
                var statement = new SqlStatement(sql,
                    new SqlParam("@Page", pagination.Page),
                    new SqlParam("@PageSize", pagination.PageSize));

                return _sqlExecutor.QueryAsync(statement, MapCliente);
            });
        }
        #endregion
        #region UpdateAsync
        public Task<Cliente> UpdateAsync(ClienteDTO cliente)
        {
            const string sql = "select * from encriptacion.update_client(" +
                "p_clave := @Clave," +
                "p_nombre := @Nombre," +
                "p_edad := @Edad," +
                "p_fecha_nacimiento := @FechaNacimiento" +
                ");";

            return RepositoryGuard.ExecuteAsync(ErrorMessages.Clients.Update, async () =>
            {
                var statement = new SqlStatement(sql,
                    new SqlParam("@Clave", cliente.Clave),
                    new SqlParam("@Nombre", cliente.Nombre),
                    new SqlParam("@Edad", cliente.Edad),
                    new SqlParam("@FechaNacimiento", cliente.Fecha_Nacimiento));

                return await _sqlExecutor.QueryFirstOrDefaultAsync(statement, MapCliente) ?? new Cliente();
            });
        }
        #endregion

        private static Cliente MapCliente(DbDataReader reader) => new()
        {
            Clave = reader.GetString("clave"),
            Nombre = reader.GetString("nombre"),
            Edad = reader.GetInt32("edad"),
            Fecha_Nacimiento = DateOnly.FromDateTime(reader.GetDateTime("fecha_nacimiento"))
        };
    }
}
