namespace UserAPI.Infrastructure.Persistence.Sql
{
    public enum SqlParamType
    {
        Default,
        Jsonb
    }

    public sealed record SqlParam(string Name, object? Value, SqlParamType Type = SqlParamType.Default);

    /// <summary>
    /// Consulta SQL con sus parámetros, sin depender de Npgsql, para poder inspeccionarla en las pruebas.
    /// </summary>
    public sealed class SqlStatement
    {
        public SqlStatement(string sql, params SqlParam[] parameters)
        {
            Sql = sql;
            Parameters = parameters;
        }

        public string Sql { get; }
        public IReadOnlyList<SqlParam> Parameters { get; }
    }
}
