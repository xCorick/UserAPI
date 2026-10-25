namespace UserAPI.Application.Common.Exceptions
{
    /// <summary>
    /// Error de PostgreSQL que no proviene de una regla de negocio (llaves foráneas, tipos inválidos, etc.).
    /// </summary>
    public sealed class DatabaseException : AppException
    {
        public DatabaseException(string code, string message, Exception innerException)
            : base(message, ErrorType.Unexpected, code, innerException)
        {
        }
    }
}
