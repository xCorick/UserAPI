namespace UserAPI.Application.Common.Exceptions
{
    /// <summary>
    /// Regla de negocio rota, ya sea lanzada por una función de PostgreSQL (RAISE EXCEPTION)
    /// o validada antes por el backend. El mensaje conserva el formato de PostgreSQL: "{código}: {mensaje}".
    /// </summary>
    public sealed class BusinessRuleException : AppException
    {
        public BusinessRuleException(string code, string detail, Exception? innerException = null)
            : base($"{code}: {detail}", ToErrorType(code), code, innerException)
        {
            Detail = detail;
        }

        public string Detail { get; }

        private static ErrorType ToErrorType(string code) => code switch
        {
            DatabaseErrorCodes.RaiseException => ErrorType.Validation,
            DatabaseErrorCodes.BusinessRule => ErrorType.NotFound,
            _ => ErrorType.Unexpected
        };
    }
}
