using Npgsql;
using UserAPI.Application.Common.Exceptions;

namespace UserAPI.Infrastructure.Persistence.Sql
{
    public static class PostgresExceptionTranslator
    {
        // Las clases P0xxx son errores lanzados con RAISE EXCEPTION dentro de las funciones
        private const string PlpgsqlErrorClass = "P0";

        public static AppException Translate(PostgresException exception) =>
            exception.SqlState.StartsWith(PlpgsqlErrorClass, StringComparison.Ordinal)
                ? new BusinessRuleException(exception.SqlState, exception.MessageText, exception)
                : new DatabaseException(exception.SqlState, exception.Message, exception);
    }
}
