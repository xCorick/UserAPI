using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserAPI.DataAccess.Interface
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        NpgsqlConnection Connection { get; }
        NpgsqlTransaction Transaction { get; }

        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        Task ExecuteInTransactionAsync(Func<Task> action);
        Task EnsureConnectionAsync();
    }
}
