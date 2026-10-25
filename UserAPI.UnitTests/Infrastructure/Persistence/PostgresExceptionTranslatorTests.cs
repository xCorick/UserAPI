using Npgsql;
using UserAPI.Application.Common;
using UserAPI.Application.Common.Exceptions;
using UserAPI.Infrastructure.Persistence.Sql;

namespace UserAPI.UnitTests.Infrastructure.Persistence
{
    public class PostgresExceptionTranslatorTests
    {
        [Theory]
        [InlineData(DatabaseErrorCodes.RaiseException, "La clave ya existe", ErrorType.Validation)]
        [InlineData(DatabaseErrorCodes.BusinessRule, "La materia con ese nombre ya existe", ErrorType.NotFound)]
        public void Translate_WhenFunctionRaisesException_ReturnsBusinessRuleWithPostgresMessage(
            string sqlState, string messageText, ErrorType expectedType)
        {
            var postgresException = new PostgresException(messageText, "ERROR", "ERROR", sqlState);

            var exception = PostgresExceptionTranslator.Translate(postgresException);

            var businessRule = Assert.IsType<BusinessRuleException>(exception);
            Assert.Equal(postgresException.Message, businessRule.Message);
            Assert.Equal($"{sqlState}: {messageText}", businessRule.Message);
            Assert.Equal(messageText, businessRule.Detail);
            Assert.Equal(sqlState, businessRule.Code);
            Assert.Equal(expectedType, businessRule.Type);
            Assert.Same(postgresException, businessRule.InnerException);
        }

        [Fact]
        public void Translate_WhenErrorIsNotABusinessRule_ReturnsUnexpectedDatabaseError()
        {
            var postgresException = new PostgresException(
                "insert or update on table \"scores\" violates foreign key constraint", "ERROR", "ERROR", PostgresErrorCodes.ForeignKeyViolation);

            var exception = PostgresExceptionTranslator.Translate(postgresException);

            var databaseException = Assert.IsType<DatabaseException>(exception);
            Assert.Equal(postgresException.Message, databaseException.Message);
            Assert.Equal(PostgresErrorCodes.ForeignKeyViolation, databaseException.Code);
            Assert.Equal(ErrorType.Unexpected, databaseException.Type);
        }
    }
}
