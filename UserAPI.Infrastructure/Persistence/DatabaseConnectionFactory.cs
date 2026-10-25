using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace UserAPI.Infrastructure.Persistence
{
    public class DatabaseConnectionFactory : IDatabaseConnectionFactory
    {
        private readonly string _databaseConnectionString;
        private readonly ILogger<DatabaseConnectionFactory> _logger;

        public DatabaseConnectionFactory(IConfiguration configuration, ILogger<DatabaseConnectionFactory> logger)
        {
            _databaseConnectionString = configuration.GetConnectionString("ApplicationDBServer")
                ?? configuration["CONNECTION_STRING"] ?? throw new ArgumentNullException(nameof(_databaseConnectionString), "Connection string is missing");
            _logger = logger;
        }

        public async Task<NpgsqlConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var connection = new NpgsqlConnection(_databaseConnectionString);
                await connection.OpenAsync(cancellationToken);
                _logger.LogInformation("Connection started");
                return connection;
            }
            catch (NpgsqlException ex)
            {
                _logger.LogInformation("Connection failed");
                throw new NpgsqlException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Connection unexpected failed");
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
