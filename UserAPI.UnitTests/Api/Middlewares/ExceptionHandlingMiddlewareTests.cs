using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text.Json;
using UserAPI.Application.Common;
using UserAPI.Application.Common.Exceptions;
using UserAPI.Middlewares;

namespace UserAPI.UnitTests.Api.Middlewares
{
    public class ExceptionHandlingMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_WhenNoErrorOccurs_LetsResponseThrough()
        {
            var (context, body) = NewContext();
            var middleware = NewMiddleware(ctx =>
            {
                ctx.Response.StatusCode = StatusCodes.Status200OK;
                return Task.CompletedTask;
            });

            await middleware.InvokeAsync(context);

            Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
            Assert.Equal(0, body.Length);
        }

        [Fact]
        public async Task InvokeAsync_WhenBackendValidationFails_Returns400WithMessage()
        {
            var (status, json) = await RunWithExceptionAsync(new ValidationException("La pregunta debe contar con 4 respuestas"));

            Assert.Equal(StatusCodes.Status400BadRequest, status);
            Assert.Equal("La pregunta debe contar con 4 respuestas", json.GetProperty("message").GetString());
        }

        [Fact]
        public async Task InvokeAsync_WhenRaiseExceptionWithoutCode_Returns400LikeBefore()
        {
            var (status, json) = await RunWithExceptionAsync(
                new BusinessRuleException(DatabaseErrorCodes.RaiseException, "La clave ya existe"));

            Assert.Equal(StatusCodes.Status400BadRequest, status);
            Assert.Equal("P0001: La clave ya existe", json.GetProperty("message").GetString());
            Assert.Equal("P0001", json.GetProperty("code").GetString());
        }

        [Fact]
        public async Task InvokeAsync_WhenBusinessRuleP0409_Returns404LikeBefore()
        {
            var (status, json) = await RunWithExceptionAsync(
                new BusinessRuleException(DatabaseErrorCodes.BusinessRule, "La materia con ese nombre ya existe"));

            Assert.Equal(StatusCodes.Status404NotFound, status);
            Assert.Equal("P0409: La materia con ese nombre ya existe", json.GetProperty("message").GetString());
            Assert.Equal("P0409", json.GetProperty("code").GetString());
        }

        [Fact]
        public async Task InvokeAsync_WhenCredentialsAreInvalid_Returns401()
        {
            var (status, json) = await RunWithExceptionAsync(new UnauthorizedException("Las credenciales son incorrectas"));

            Assert.Equal(StatusCodes.Status401Unauthorized, status);
            Assert.Equal("Las credenciales son incorrectas", json.GetProperty("message").GetString());
        }

        [Fact]
        public async Task InvokeAsync_WhenRepositoryFailsUnexpectedly_Returns500WithOperationMessage()
        {
            var (status, json) = await RunWithExceptionAsync(
                new UnexpectedException(ErrorMessages.Subjects.Update, new TimeoutException()));

            Assert.Equal(StatusCodes.Status500InternalServerError, status);
            Assert.Equal("Unexpected error during update subject", json.GetProperty("message").GetString());
        }

        [Fact]
        public async Task InvokeAsync_WhenUnknownExceptionIsThrown_Returns500WithItsMessage()
        {
            var (status, json) = await RunWithExceptionAsync(new InvalidOperationException("Algo salió mal"));

            Assert.Equal(StatusCodes.Status500InternalServerError, status);
            Assert.Equal("Algo salió mal", json.GetProperty("message").GetString());
        }

        private static async Task<(int Status, JsonElement Json)> RunWithExceptionAsync(Exception exception)
        {
            var (context, body) = NewContext();
            var middleware = NewMiddleware(_ => throw exception);

            await middleware.InvokeAsync(context);

            body.Position = 0;
            using var document = await JsonDocument.ParseAsync(body);
            return (context.Response.StatusCode, document.RootElement.Clone());
        }

        private static ExceptionHandlingMiddleware NewMiddleware(RequestDelegate next) =>
            new(next, NullLogger<ExceptionHandlingMiddleware>.Instance);

        private static (DefaultHttpContext Context, MemoryStream Body) NewContext()
        {
            var body = new MemoryStream();
            var context = new DefaultHttpContext();
            context.Response.Body = body;
            return (context, body);
        }
    }
}
