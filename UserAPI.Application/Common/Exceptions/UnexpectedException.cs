namespace UserAPI.Application.Common.Exceptions
{
    public sealed class UnexpectedException : AppException
    {
        public UnexpectedException(string message, Exception innerException)
            : base(message, ErrorType.Unexpected, innerException: innerException)
        {
        }
    }
}
