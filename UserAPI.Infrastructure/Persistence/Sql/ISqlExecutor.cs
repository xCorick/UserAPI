using System.Data.Common;

namespace UserAPI.Infrastructure.Persistence.Sql
{
    /// <summary>
    /// Único punto que habla con PostgreSQL. Los repositorios definen la consulta y el mapeo;
    /// esta abstracción permite reemplazar la base de datos en las pruebas unitarias.
    /// </summary>
    public interface ISqlExecutor
    {
        Task<List<T>> QueryAsync<T>(SqlStatement statement, Func<DbDataReader, T> map);
        Task<T?> QueryFirstOrDefaultAsync<T>(SqlStatement statement, Func<DbDataReader, T> map);
        Task<object?> ExecuteScalarAsync(SqlStatement statement);
    }
}
