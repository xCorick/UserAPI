using Npgsql;

namespace UserAPI.Infrastructure.Persistence
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
