using System.Text.RegularExpressions;
using UserAPI.Infrastructure.Persistence.Sql;

namespace UserAPI.UnitTests.Helpers
{
    public static class SqlAssert
    {
        public static void CallsFunction(SqlStatement statement, string function) =>
            Assert.Contains($"encriptacion.{function}(", statement.Sql);

        public static object? Parameter(SqlStatement statement, string name) =>
            Assert.Single(statement.Parameters, p => p.Name == name).Value;

        public static SqlParam ParameterInfo(SqlStatement statement, string name) =>
            Assert.Single(statement.Parameters, p => p.Name == name);

        // Cada @Parametro del SQL debe tener un valor y no debe sobrar ninguno
        public static void BindsEveryPlaceholder(SqlStatement statement)
        {
            var placeholders = Regex.Matches(statement.Sql, @"@\w+").Select(m => m.Value).Order();
            var parameters = statement.Parameters.Select(p => p.Name).Order();

            Assert.Equal(placeholders, parameters);
        }
    }
}
