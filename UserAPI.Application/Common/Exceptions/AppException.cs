namespace UserAPI.Application.Common.Exceptions
{
    public abstract class AppException : Exception
    {
        protected AppException(string message, ErrorType type, string? code = null, Exception? innerException = null)
            : base(message, innerException)
        {
            Type = type;
            Code = code;
        }

        public ErrorType Type { get; }
        public string? Code { get; }
    }
}
