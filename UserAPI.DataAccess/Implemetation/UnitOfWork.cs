using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAPI.DataAccess.Interface;

namespace UserAPI.DataAccess.Implemetation
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly ILogger<UnitOfWork> _logger;
        private bool _disposed;
        public NpgsqlConnection Connection { get; private set; } = default!;
        public NpgsqlTransaction Transaction { get; private set; } = default!;

        public UnitOfWork(IDatabaseConnectionFactory databaseConnectionFactory, ILogger<UnitOfWork> logger)
        {
            _databaseConnectionFactory = databaseConnectionFactory;
            _logger = logger;
        }

        public async Task BeginTransactionAsync()
        {
            Connection ??= await _databaseConnectionFactory.CreateConnectionAsync();
            Transaction ??= await Connection.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (Transaction != null)
                await Transaction.CommitAsync();
        }

        public void Dispose() { DisposeAsync().GetAwaiter().GetResult(); GC.SuppressFinalize(this); }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            if (Connection != null)
                await Connection.DisposeAsync();
            if (Transaction != null)
                await Transaction.DisposeAsync();
            _disposed = true;
        }

        public async Task EnsureConnectionAsync()
        {
            Connection ??= await _databaseConnectionFactory.CreateConnectionAsync();
        }

        public async Task ExecuteInTransactionAsync(Func<Task> action)
        {
            Connection ??= await _databaseConnectionFactory.CreateConnectionAsync();
            await using var transaction = await Connection.BeginTransactionAsync();
            Transaction = transaction;
            try
            {
                await action();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Error during transaction. Rolling back ...");
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task RollbackAsync()
        {
            if(Transaction != null)
                await Transaction.RollbackAsync();
        }
    }
}
