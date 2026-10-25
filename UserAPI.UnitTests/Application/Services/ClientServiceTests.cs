using Moq;
using UserAPI.Application.Common;
using UserAPI.Application.Common.Exceptions;
using UserAPI.Application.DTOs;
using UserAPI.Application.Interfaces.Repositories;
using UserAPI.Application.Services;
using UserAPI.Domain.Entities;

namespace UserAPI.UnitTests.Application.Services
{
    public class ClientServiceTests
    {
        private readonly Mock<IClientRepository> _repository = new();
        private readonly ClientService _service;

        public ClientServiceTests() => _service = new ClientService(_repository.Object);

        [Fact]
        public async Task CreateAsync_ReturnsClientCreatedByRepository()
        {
            var dto = new ClienteDTO { Clave = "CLI00001", Nombre = "Juan Pérez", Edad = 30 };
            var created = new Cliente { Clave = "CLI00001", Nombre = "Juan Pérez", Edad = 30 };
            _repository.Setup(r => r.CreateAsync(dto)).ReturnsAsync(created);

            var result = await _service.CreateAsync(dto);

            Assert.Same(created, result);
        }

        [Fact]
        public async Task CreateAsync_WhenClaveAlreadyExists_PropagatesPostgresError()
        {
            var postgresError = new BusinessRuleException(DatabaseErrorCodes.RaiseException, "La clave ya existe");
            _repository.Setup(r => r.CreateAsync(It.IsAny<ClienteDTO>())).ThrowsAsync(postgresError);

            var exception = await Assert.ThrowsAsync<BusinessRuleException>(() => _service.CreateAsync(new ClienteDTO()));

            Assert.Equal("P0001: La clave ya existe", exception.Message);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsClientUpdatedByRepository()
        {
            var dto = new ClienteDTO { Clave = "CLI00001", Nombre = "Nuevo nombre" };
            var updated = new Cliente { Clave = "CLI00001", Nombre = "Nuevo nombre" };
            _repository.Setup(r => r.UpdateAsync(dto)).ReturnsAsync(updated);

            var result = await _service.UpdateAsync(dto);

            Assert.Same(updated, result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsClientDeletedByRepository()
        {
            var deleted = new Cliente { Clave = "CLI00001" };
            _repository.Setup(r => r.DeleteAsync("CLI00001")).ReturnsAsync(deleted);

            var result = await _service.DeleteAsync("CLI00001");

            Assert.Same(deleted, result);
        }

        [Fact]
        public async Task GetClienteAsync_ReturnsClientFromRepository()
        {
            var cliente = new Cliente { Clave = "CLI00001" };
            _repository.Setup(r => r.GetClienteAsync("CLI00001")).ReturnsAsync(cliente);

            var result = await _service.GetClienteAsync("CLI00001");

            Assert.Same(cliente, result);
        }

        [Fact]
        public async Task GetClientesPageAsync_ReturnsPageFromRepository()
        {
            var pagination = new PaginationFilter { Page = 1, PageSize = 10 };
            var clientes = new List<Cliente> { new() { Clave = "CLI00001" } };
            _repository.Setup(r => r.GetClientesPageAsync(pagination)).ReturnsAsync(clientes);

            var result = await _service.GetClientesPageAsync(pagination);

            Assert.Same(clientes, result);
        }
    }
}
