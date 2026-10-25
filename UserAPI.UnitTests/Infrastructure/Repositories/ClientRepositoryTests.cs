using System.Data;
using UserAPI.Application.Common;
using UserAPI.Application.Common.Exceptions;
using UserAPI.Application.DTOs;
using UserAPI.Domain.Entities;
using UserAPI.Infrastructure.Repositories;
using UserAPI.UnitTests.Helpers;

namespace UserAPI.UnitTests.Infrastructure.Repositories
{
    public class ClientRepositoryTests
    {
        private const string Clave = "CLI00001";

        private readonly FakeSqlExecutor _executor = new();
        private readonly ClientRepository _repository;

        public ClientRepositoryTests() => _repository = new ClientRepository(_executor);

        public static TheoryData<string> Operations => new()
        {
            nameof(ClientRepository.CreateAsync),
            nameof(ClientRepository.UpdateAsync),
            nameof(ClientRepository.DeleteAsync),
            nameof(ClientRepository.GetClienteAsync),
            nameof(ClientRepository.GetClientesAsync),
            nameof(ClientRepository.GetClientesPageAsync)
        };

        public static TheoryData<string, string> OperationErrorMessages => new()
        {
            { nameof(ClientRepository.CreateAsync), ErrorMessages.Clients.Create },
            { nameof(ClientRepository.UpdateAsync), ErrorMessages.Clients.Update },
            { nameof(ClientRepository.DeleteAsync), ErrorMessages.Clients.Delete },
            { nameof(ClientRepository.GetClienteAsync), ErrorMessages.Clients.GetByClave },
            { nameof(ClientRepository.GetClientesAsync), ErrorMessages.Clients.GetAll },
            { nameof(ClientRepository.GetClientesPageAsync), ErrorMessages.Clients.GetPage }
        };

        [Fact]
        public async Task CreateAsync_SendsClientDataToInsertFunction()
        {
            await _repository.CreateAsync(NewClienteDTO());

            var statement = _executor.ExecutedStatement;
            SqlAssert.CallsFunction(statement, "insert_client");
            Assert.Equal(Clave, SqlAssert.Parameter(statement, "@Clave"));
            Assert.Equal("Juan Pérez", SqlAssert.Parameter(statement, "@Nombre"));
            Assert.Equal(30, SqlAssert.Parameter(statement, "@Edad"));
            Assert.Equal(new DateOnly(1995, 5, 20), SqlAssert.Parameter(statement, "@FechaNacimiento"));
        }

        [Fact]
        public async Task CreateAsync_ReturnsInsertedClient()
        {
            _executor.Rows = ClienteRows((Clave, "Juan Pérez", 30, new DateTime(1995, 5, 20)));

            var cliente = await _repository.CreateAsync(NewClienteDTO());

            AssertCliente(Clave, "Juan Pérez", 30, new DateOnly(1995, 5, 20), cliente);
        }

        [Fact]
        public async Task CreateAsync_WhenFunctionReturnsNoRows_ReturnsEmptyClient()
        {
            _executor.Rows = ClienteRows();

            var cliente = await _repository.CreateAsync(NewClienteDTO());

            Assert.Null(cliente.Clave);
        }

        [Fact]
        public async Task UpdateAsync_SendsChangesToUpdateFunctionAndReturnsUpdatedClient()
        {
            var dto = NewClienteDTO();
            dto.Nombre = "Juan Actualizado";
            dto.Edad = 31;
            _executor.Rows = ClienteRows((Clave, "Juan Actualizado", 31, new DateTime(1995, 5, 20)));

            var cliente = await _repository.UpdateAsync(dto);

            var statement = _executor.ExecutedStatement;
            SqlAssert.CallsFunction(statement, "update_client");
            Assert.Equal(Clave, SqlAssert.Parameter(statement, "@Clave"));
            Assert.Equal("Juan Actualizado", SqlAssert.Parameter(statement, "@Nombre"));
            Assert.Equal(31, SqlAssert.Parameter(statement, "@Edad"));
            AssertCliente(Clave, "Juan Actualizado", 31, new DateOnly(1995, 5, 20), cliente);
        }

        [Fact]
        public async Task DeleteAsync_SendsClaveToDeleteFunctionAndReturnsDeletedClient()
        {
            _executor.Rows = ClienteRows((Clave, "Juan Pérez", 30, new DateTime(1995, 5, 20)));

            var cliente = await _repository.DeleteAsync(Clave);

            var statement = _executor.ExecutedStatement;
            SqlAssert.CallsFunction(statement, "delete_client");
            Assert.Equal(Clave, SqlAssert.Parameter(statement, "@Clave"));
            AssertCliente(Clave, "Juan Pérez", 30, new DateOnly(1995, 5, 20), cliente);
        }

        [Fact]
        public async Task GetClienteAsync_WhenClientExists_ReturnsClient()
        {
            _executor.Rows = ClienteRows((Clave, "Juan Pérez", 30, new DateTime(1995, 5, 20)));

            var cliente = await _repository.GetClienteAsync(Clave);

            var statement = _executor.ExecutedStatement;
            SqlAssert.CallsFunction(statement, "get_client");
            Assert.Equal(Clave, SqlAssert.Parameter(statement, "@Clave"));
            AssertCliente(Clave, "Juan Pérez", 30, new DateOnly(1995, 5, 20), cliente);
        }

        [Fact]
        public async Task GetClienteAsync_WhenFunctionReturnsNullRow_ReturnsEmptyClient()
        {
            // get_client hace RETURN NULL: PostgreSQL entrega una fila con todas las columnas nulas
            _executor.Rows = ClienteRows((DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value));

            var cliente = await _repository.GetClienteAsync("NOEXISTE");

            Assert.Null(cliente.Clave);
            Assert.Null(cliente.Nombre);
        }

        [Fact]
        public async Task GetClientesAsync_ReturnsEveryRow()
        {
            _executor.Rows = ClienteRows(
                ("CLI00001", "Juan Pérez", 30, new DateTime(1995, 5, 20)),
                ("CLI00002", "Ana López", 25, new DateTime(2000, 1, 15)));

            var clientes = await _repository.GetClientesAsync();

            SqlAssert.CallsFunction(_executor.ExecutedStatement, "get_clients");
            Assert.Collection(clientes,
                c => AssertCliente("CLI00001", "Juan Pérez", 30, new DateOnly(1995, 5, 20), c),
                c => AssertCliente("CLI00002", "Ana López", 25, new DateOnly(2000, 1, 15), c));
        }

        [Fact]
        public async Task GetClientesPageAsync_SendsPaginationAndReturnsRows()
        {
            _executor.Rows = ClienteRows(("CLI00011", "Luis Díaz", 40, new DateTime(1985, 3, 2)));

            var clientes = await _repository.GetClientesPageAsync(new PaginationFilter { Page = 2, PageSize = 10 });

            var statement = _executor.ExecutedStatement;
            SqlAssert.CallsFunction(statement, "get_clients_page");
            Assert.Equal(2, SqlAssert.Parameter(statement, "@Page"));
            Assert.Equal(10, SqlAssert.Parameter(statement, "@PageSize"));
            var cliente = Assert.Single(clientes);
            AssertCliente("CLI00011", "Luis Díaz", 40, new DateOnly(1985, 3, 2), cliente);
        }

        [Fact]
        public async Task GetClientesAsync_WhenRowDoesNotMatchExpectedColumns_WrapsError()
        {
            _executor.Rows = Rows.Table(("clave", typeof(string)), ("nombre", typeof(string)));
            _executor.Rows.Rows.Add(Clave, "Juan Pérez");

            var exception = await Assert.ThrowsAsync<UnexpectedException>(() => _repository.GetClientesAsync());

            Assert.Equal(ErrorMessages.Clients.GetAll, exception.Message);
            Assert.NotNull(exception.InnerException);
        }

        [Theory]
        [MemberData(nameof(Operations))]
        public async Task Operation_BindsEverySqlPlaceholder(string operation)
        {
            await InvokeAsync(operation);

            SqlAssert.BindsEveryPlaceholder(_executor.ExecutedStatement);
        }

        [Theory]
        [MemberData(nameof(Operations))]
        public async Task Operation_WhenPostgresRejectsIt_PropagatesBusinessRule(string operation)
        {
            var postgresError = new BusinessRuleException(DatabaseErrorCodes.RaiseException, "La clave ya existe");
            _executor.Exception = postgresError;

            var exception = await Assert.ThrowsAsync<BusinessRuleException>(() => InvokeAsync(operation));

            Assert.Same(postgresError, exception);
        }

        [Theory]
        [MemberData(nameof(OperationErrorMessages))]
        public async Task Operation_WhenExecutionFailsUnexpectedly_WrapsError(string operation, string expectedMessage)
        {
            var connectionError = new TimeoutException("Connection timed out");
            _executor.Exception = connectionError;

            var exception = await Assert.ThrowsAsync<UnexpectedException>(() => InvokeAsync(operation));

            Assert.Equal(expectedMessage, exception.Message);
            Assert.Same(connectionError, exception.InnerException);
        }

        private Task InvokeAsync(string operation) => operation switch
        {
            nameof(ClientRepository.CreateAsync) => _repository.CreateAsync(NewClienteDTO()),
            nameof(ClientRepository.UpdateAsync) => _repository.UpdateAsync(NewClienteDTO()),
            nameof(ClientRepository.DeleteAsync) => _repository.DeleteAsync(Clave),
            nameof(ClientRepository.GetClienteAsync) => _repository.GetClienteAsync(Clave),
            nameof(ClientRepository.GetClientesAsync) => _repository.GetClientesAsync(),
            nameof(ClientRepository.GetClientesPageAsync) => _repository.GetClientesPageAsync(new PaginationFilter { Page = 1, PageSize = 10 }),
            _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
        };

        private static ClienteDTO NewClienteDTO() => new()
        {
            Clave = Clave,
            Nombre = "Juan Pérez",
            Edad = 30,
            Fecha_Nacimiento = new DateOnly(1995, 5, 20)
        };

        // Columnas de la tabla encriptacion.clientes
        private static DataTable ClienteRows(params (object Clave, object Nombre, object Edad, object FechaNacimiento)[] rows)
        {
            var table = Rows.Table(
                ("clave", typeof(string)),
                ("nombre", typeof(string)),
                ("edad", typeof(int)),
                ("fecha_nacimiento", typeof(DateTime)));

            foreach (var row in rows)
                table.Rows.Add(row.Clave, row.Nombre, row.Edad, row.FechaNacimiento);

            return table;
        }

        private static void AssertCliente(string clave, string nombre, int edad, DateOnly fechaNacimiento, Cliente actual)
        {
            Assert.Equal(clave, actual.Clave);
            Assert.Equal(nombre, actual.Nombre);
            Assert.Equal(edad, actual.Edad);
            Assert.Equal(fechaNacimiento, actual.Fecha_Nacimiento);
        }
    }
}
