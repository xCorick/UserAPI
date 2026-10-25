using UserAPI.Application.Common.Exceptions;

namespace UserAPI.Infrastructure.Repositories
{
    internal static class RepositoryGuard
    {
        // Los errores ya clasificados (reglas de PostgreSQL, validaciones) pasan tal cual;
        // cualquier otro fallo se envuelve con el mensaje de la operación.
        public static async Task<T> ExecuteAsync<T>(string errorMessage, Func<Task<T>> operation)
        {
            try
            {
                return await operation();
            }
            catch (AppException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new UnexpectedException(errorMessage, ex);
            }
        }
    }
}
