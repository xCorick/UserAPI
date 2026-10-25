using Npgsql;
using NpgsqlTypes;
using System.Data;
using System.Data.Common;

namespace UserAPI.Infrastructure.Persistence.Sql
{
    public class NpgsqlSqlExecutor : ISqlExecutor
    {
        private readonly IUnitOfWork _unitOfWork;

        public NpgsqlSqlExecutor(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<List<T>> QueryAsync<T>(SqlStatement statement, Func<DbDataReader, T> map)
        {
            try
            {
                await using var command = await CreateCommandAsync(statement);
                await using var reader = await command.ExecuteReaderAsync();

                var results = new List<T>();
                while (await reader.ReadAsync())
                    results.Add(map(reader));

                return results;
            }
            catch (PostgresException ex)
            {
                throw PostgresExceptionTranslator.Translate(ex);
            }
        }

        public async Task<T?> QueryFirstOrDefaultAsync<T>(SqlStatement statement, Func<DbDataReader, T> map)
        {
            try
            {
                await using var command = await CreateCommandAsync(statement);
                await using var reader = await command.ExecuteReaderAsync();

                return await reader.ReadAsync() ? map(reader) : default;
            }
            catch (PostgresException ex)
            {
                throw PostgresExceptionTranslator.Translate(ex);
            }
        }

        public async Task<object?> ExecuteScalarAsync(SqlStatement statement)
        {
            try
            {
                await using var command = await CreateCommandAsync(statement);
                return await command.ExecuteScalarAsync();
            }
            catch (PostgresException ex)
            {
                throw PostgresExceptionTranslator.Translate(ex);
            }
        }

        private async Task<NpgsqlCommand> CreateCommandAsync(SqlStatement statement)
        {
            await _unitOfWork.EnsureConnectionAsync();

            var command = new NpgsqlCommand(statement.Sql, _unitOfWork.Connection, _unitOfWork.Transaction)
            {
                CommandType = CommandType.Text
            };

            foreach (var parameter in statement.Parameters)
            {
                if (parameter.Type == SqlParamType.Jsonb)
                    command.Parameters.AddWithValue(parameter.Name, NpgsqlDbType.Jsonb, parameter.Value!);
                else
                    command.Parameters.AddWithValue(parameter.Name, parameter.Value!);
            }

            return command;
        }
    }
}
