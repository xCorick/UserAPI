using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAPI.Data.DTOs;
using UserAPI.Data.Models;

namespace UserAPI.Data.Interface
{
    public interface IClientRepository
    {
        Task<Cliente> CreateAsync(ClienteDTO cliente);
        Task<Cliente> UpdateAsync(ClienteDTO cliente);
        Task<Cliente> DeleteAsync(string clave);
        Task<List<Cliente>> GetClientes();
        Task<List<Cliente>> GetClientesPage(PaginationFilter pagination);
        Task<Cliente> GetCliente(string clave);
    }
}
