using System.Data;
using System.Data.Common;
using UserAPI.Infrastructure.Persistence.Sql;

namespace UserAPI.UnitTests.Helpers
{
    /// <summary>
    /// Reemplaza a PostgreSQL: guarda cada consulta recibida y devuelve las filas configuradas
    /// a través de un DbDataReader real, así que el mapeo de los repositorios se ejecuta tal cual.
    /// </summary>
    public sealed class FakeSqlExecutor : ISqlExecutor
    {
        public List<SqlStatement> Statements { get; } = new();
        // Cada operación de repositorio debe ejecutar exactamente una consulta
        public SqlStatement ExecutedStatement => Assert.Single(Statements);

        public DataTable Rows { get; set; } = new();
        public object? ScalarResult { get; set; }
        public Exception? Exception { get; set; }

        public async Task<List<T>> QueryAsync<T>(SqlStatement statement, Func<DbDataReader, T> map)
        {
            await RecordAsync(statement);

            using var reader = Rows.CreateDataReader();
            var results = new List<T>();
            while (reader.Read())
                results.Add(map(reader));

            return results;
        }

        public async Task<T?> QueryFirstOrDefaultAsync<T>(SqlStatement statement, Func<DbDataReader, T> map)
        {
            await RecordAsync(statement);

            using var reader = Rows.CreateDataReader();
            return reader.Read() ? map(reader) : default;
        }

        public async Task<object?> ExecuteScalarAsync(SqlStatement statement)
        {
            await RecordAsync(statement);
            return ScalarResult;
        }

        private async Task RecordAsync(SqlStatement statement)
        {
            await Task.Yield();
            Statements.Add(statement);

            if (Exception is not null)
                throw Exception;
        }
    }
}
