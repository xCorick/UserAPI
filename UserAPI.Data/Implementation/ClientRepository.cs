using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAPI.Data.DTOs;
using UserAPI.Data.Interface;
using UserAPI.Data.Models;
using UserAPI.DataAccess.Interface;
using System.Data;
using Microsoft.Extensions.Logging;

namespace UserAPI.Data.Implementation
{
    public class ClientRepository : IClientRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ClientRepository> _logger;

        public ClientRepository(IUnitOfWork unitOfWork, ILogger<ClientRepository> logger) { 
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        #region CreateAsync
        public async Task<Cliente> CreateAsync(ClienteDTO cliente)
        {
            const string sql = "select * from encriptacion.insert_client(" +
                "p_clave := @Clave," +
                "p_nombre := @Nombre," +
                "p_edad := @Edad," +
                "p_fecha_nacimiento := @FechaNacimiento" +
                ");";

            try
            {
                await _unitOfWork.EnsureConnectionAsync();

                await using var command = new NpgsqlCommand(sql, _unitOfWork.Connection, _unitOfWork.Transaction)
                {
                    CommandType = CommandType.Text
                };

                command.Parameters.AddWithValue("@Clave", cliente.Clave!);
                command.Parameters.AddWithValue("@Nombre", cliente.Nombre!);
                command.Parameters.AddWithValue("@Edad", cliente.Edad!);
                command.Parameters.AddWithValue("@FechaNacimiento", cliente.Fecha_Nacimiento);
                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new Cliente
                    {
                        Clave = reader.GetString(reader.GetOrdinal("clave")),
                        Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                        Edad = reader.GetInt32(reader.GetOrdinal("edad")),
                        Fecha_Nacimiento = DateOnly.FromDateTime(
                            reader.GetDateTime(reader.GetOrdinal("fecha_nacimiento")))
                    };
                }
            }
            catch (PostgresException ex)
            {
                throw new Exception(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error during insertion", ex);
            }
            return new Cliente();
        }
        #endregion
        #region DeleteAsync
        public async Task<Cliente> DeleteAsync(string clave)
        {
            const string sql = "select * from encriptacion.delete_client(" +
                "p_clave := @Clave" +
                ");";

            try
            {
                await _unitOfWork.EnsureConnectionAsync();

                await using var command = new NpgsqlCommand(sql, _unitOfWork.Connection, _unitOfWork.Transaction)
                {
                    CommandType = CommandType.Text
                };

                command.Parameters.AddWithValue("@Clave", clave);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new Cliente
                    {
                        Clave = reader.GetString(reader.GetOrdinal("clave")),
                        Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                        Edad = reader.GetInt32(reader.GetOrdinal("edad")),
                        Fecha_Nacimiento = DateOnly.FromDateTime(
                            reader.GetDateTime(reader.GetOrdinal("fecha_nacimiento")))
                    };
                }
            }
            catch (PostgresException ex)
            {
                throw new Exception(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error during delete", ex);
            }
            return new Cliente();
        }
        #endregion
        #region GetClienteAsync
        public async Task<Cliente> GetClienteAsync(string clave)
        {
            const string sql = "select * from encriptacion.get_client(" +
                "p_clave := @Clave" +
                ");";

            try
            {
                await _unitOfWork.EnsureConnectionAsync();

                await using var command = new NpgsqlCommand(sql, _unitOfWork.Connection, _unitOfWork.Transaction)
                {
                    CommandType = CommandType.Text
                };

                command.Parameters.AddWithValue("@Clave", clave);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new Cliente
                    {
                        Clave = reader.GetString(reader.GetOrdinal("clave")),
                        Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                        Edad = reader.GetInt32(reader.GetOrdinal("edad")),
                        Fecha_Nacimiento = DateOnly.FromDateTime(
                            reader.GetDateTime(reader.GetOrdinal("fecha_nacimiento")))
                    };
                }
            }
            catch (PostgresException ex)
            {
                throw new Exception(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error during get", ex);
            }
            return new Cliente();
        }
        #endregion
        #region GetClientesAsync
        public async Task<List<Cliente>> GetClientesAsync()
        {
            const string sql = "select * from encriptacion.get_clients()";
            var clientes = new List<Cliente>();
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
                    clientes.Add(new Cliente
                        {
                            Clave = reader.GetString(reader.GetOrdinal("clave")),
                            Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                            Edad = reader.GetInt32(reader.GetOrdinal("edad")),
                            Fecha_Nacimiento = DateOnly.FromDateTime(
                                reader.GetDateTime(reader.GetOrdinal("fecha_nacimiento")))
                        }
                    );
                }
            }
            catch (PostgresException ex)
            {
                throw new Exception(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error during get clients", ex);
            }
            return clientes;
        }
        #endregion
        #region GetClientesPageAsync
        public async Task<List<Cliente>> GetClientesPageAsync(PaginationFilter pagination)
        {
            const string sql = "select * from encriptacion.get_clients_page(" +
                "p_page := @Page," +
                "p_page_size := @PageSize" +
                ");";
            var clientes = new List<Cliente>();
            try
            {
                await _unitOfWork.EnsureConnectionAsync();

                await using var command = new NpgsqlCommand(sql, _unitOfWork.Connection, _unitOfWork.Transaction)
                {
                    CommandType = CommandType.Text
                };

                command.Parameters.AddWithValue("@Page", pagination.Page);
                command.Parameters.AddWithValue("@PageSize", pagination.PageSize);

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    clientes.Add(new Cliente
                        {
                            Clave = reader.GetString(reader.GetOrdinal("clave")),
                            Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                            Edad = reader.GetInt32(reader.GetOrdinal("edad")),
                            Fecha_Nacimiento = DateOnly.FromDateTime(
                                    reader.GetDateTime(reader.GetOrdinal("fecha_nacimiento")))
                        }
                    );
                }
                return clientes;
            }
            catch (PostgresException ex)
            {
                throw new Exception(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error during get clients", ex);
            }
        }
        #endregion
        #region UpdateAsync
        public async Task<Cliente> UpdateAsync(ClienteDTO cliente)
        {
            const string sql = "select * from encriptacion.update_client(" +
                "p_clave := @Clave," +
                "p_nombre := @Nombre," +
                "p_edad := @Edad," +
                "p_fecha_nacimiento := @FechaNacimiento" +
                ");";
            try
            {
                await _unitOfWork.EnsureConnectionAsync();

                await using var command = new NpgsqlCommand(sql, _unitOfWork.Connection, _unitOfWork.Transaction);

                command.Parameters.AddWithValue("@Clave", cliente.Clave!);
                command.Parameters.AddWithValue("@Nombre", cliente.Nombre!);
                command.Parameters.AddWithValue("@Edad", cliente.Edad!);
                command.Parameters.AddWithValue("@FechaNacimiento", cliente.Fecha_Nacimiento);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new Cliente
                    {
                        Clave = reader.GetString(reader.GetOrdinal("clave")),
                        Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                        Edad = reader.GetInt32(reader.GetOrdinal("edad")),
                        Fecha_Nacimiento = DateOnly.FromDateTime(
                                    reader.GetDateTime(reader.GetOrdinal("fecha_nacimiento")))
                    };
                }
                return new Cliente();
            }
            catch (PostgresException ex)
            {
                throw new Exception(sql, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error during update", ex);
            }
        }
        #endregion
    }
}
