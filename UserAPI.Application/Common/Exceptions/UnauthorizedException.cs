namespace UserAPI.Application.Common.Exceptions
{
    public sealed class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message)
            : base(message, ErrorType.Unauthorized)
        {
        }
    }
}
