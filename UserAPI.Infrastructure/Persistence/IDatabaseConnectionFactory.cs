using Npgsql;

namespace UserAPI.Infrastructure.Persistence
{
    public interface IDatabaseConnectionFactory
    {
        Task<NpgsqlConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
    }
}
