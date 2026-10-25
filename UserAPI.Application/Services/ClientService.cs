using UserAPI.Application.DTOs;
using UserAPI.Application.Interfaces.Repositories;
using UserAPI.Application.Interfaces.Services;
using UserAPI.Domain.Entities;

namespace UserAPI.Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository) => _clientRepository = clientRepository;

        public Task<Cliente> CreateAsync(ClienteDTO cliente) => _clientRepository.CreateAsync(cliente);

        public Task<Cliente> UpdateAsync(ClienteDTO cliente) => _clientRepository.UpdateAsync(cliente);

        public Task<Cliente> DeleteAsync(string clave) => _clientRepository.DeleteAsync(clave);

        public Task<Cliente> GetClienteAsync(string clave) => _clientRepository.GetClienteAsync(clave);

        public Task<List<Cliente>> GetClientesAsync() => _clientRepository.GetClientesAsync();

        public Task<List<Cliente>> GetClientesPageAsync(PaginationFilter pagination) =>
            _clientRepository.GetClientesPageAsync(pagination);
    }
}
