using UserAPI.Application.DTOs;
using UserAPI.Domain.Entities;

namespace UserAPI.Application.Interfaces.Repositories
{
    public interface IClientRepository
    {
        Task<Cliente> CreateAsync(ClienteDTO cliente);
        Task<Cliente> UpdateAsync(ClienteDTO cliente);
        Task<Cliente> DeleteAsync(string clave);
        Task<List<Cliente>> GetClientesAsync();
        Task<List<Cliente>> GetClientesPageAsync(PaginationFilter pagination);
        Task<Cliente> GetClienteAsync(string clave);
    }
}
