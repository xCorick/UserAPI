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

namespace UserAPI.Data.Implementation
{
    public class ClienteRepository : IClientRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClienteRepository(IUnitOfWork unitOfWork) { 
            _unitOfWork = unitOfWork;
        }
        public async Task<Cliente> CreateAsync(ClienteDTO cliente)
        {
            const string sql = "insert_client";

            try
            {
                await _unitOfWork.EnsureConnectionAsync();

                await using var command = new NpgsqlCommand(sql, _unitOfWork.Connection, _unitOfWork.Transaction)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("p_clave", cliente.Clave!);
                command.Parameters.AddWithValue("p_nombre", cliente.Nombre!);
                command.Parameters.AddWithValue("p_edad", cliente.Edad!);
                command.Parameters.AddWithValue("p_fecha_nacimiento", cliente.Fecha_Nacimiento);
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

        public async Task<Cliente> DeleteAsync(string clave)
        {
            const string sql = "delete_client";

            try
            {
                await _unitOfWork.EnsureConnectionAsync();

                await using var command = new NpgsqlCommand(sql, _unitOfWork.Connection, _unitOfWork.Transaction)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("p_clave", clave);

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

        public async Task<Cliente> GetCliente(string clave)
        {
            const string sql = "get_client";

            try
            {
                await _unitOfWork.EnsureConnectionAsync();

                await using var command = new NpgsqlCommand(sql, _unitOfWork.Connection, _unitOfWork.Transaction)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("p_clave", clave);

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

        public Task<List<Cliente>> GetClientes()
        {
            throw new NotImplementedException();
        }

        public Task<List<Cliente>> GetClientesPage(PaginationFilter pagination)
        {
            throw new NotImplementedException();
        }

        public Task<Cliente> UpdateAsync(ClienteDTO cliente)
        {
            throw new NotImplementedException();
        }
    }
}
