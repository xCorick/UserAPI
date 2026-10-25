using System.Data;
using System.Text.Json;
using UserAPI.Application.Common;
using UserAPI.Application.DTOs;
using UserAPI.Application.Interfaces.Repositories;
using UserAPI.Domain.Entities;
using UserAPI.Infrastructure.Persistence.Sql;

namespace UserAPI.Infrastructure.Repositories
{
    public class ScoreRepository : IScoreRepository
    {
        private static readonly JsonSerializerOptions serializeOptionsJson = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        private readonly ISqlExecutor _sqlExecutor;

        public ScoreRepository(ISqlExecutor sqlExecutor) => _sqlExecutor = sqlExecutor;

        public Task<Scores> CreateScoreAsync(Scores scores)
        {
            const string sql = "select * from encriptacion.insert_score(" +
                "p_score := @Score," +
                "p_id_user := @IdUser," +
                "p_is_mixed := @IsMixed," +
                "p_points := @Points," +
                "p_id_subject := @IdSubject" +
                ");";

            return RepositoryGuard.ExecuteAsync(ErrorMessages.Scores.Create, async () =>
            {
                var statement = new SqlStatement(sql,
                    new SqlParam("@Score", scores.Score),
                    new SqlParam("@IdUser", scores.IdUser),
                    new SqlParam("@IsMixed", scores.IsMixed),
                    new SqlParam("@Points", scores.Points),
                    new SqlParam("@IdSubject", scores.IdSubject ?? (object)DBNull.Value));

                return await _sqlExecutor.QueryFirstOrDefaultAsync(statement, reader => new Scores
                {
                    Id = reader.GetGuid("id"),
                    Score = reader.GetDecimal("score"),
                    Points = reader.GetInt32("points"),
                    IdSubject = reader.IsDBNull("id_subject") ?
                        null : reader.GetGuid("id_subject"),
                    IsMixed = reader.GetBoolean("is_mixed")
                }) ?? new Scores();
            });
        }

        public Task<Progress> GetGeneralProgressAsync(Guid Id)
        {
            const string sql = "select * from encriptacion.get_general_progress(" +
                "p_id := @Id" +
                ");";

            return RepositoryGuard.ExecuteAsync(ErrorMessages.Scores.GetGeneralProgress, async () =>
            {
                var statement = new SqlStatement(sql, new SqlParam("@Id", Id));

                return await _sqlExecutor.QueryFirstOrDefaultAsync(statement, reader => new Progress
                {
                    Score = reader.GetDecimal("score"),
                    Points = reader.GetInt32("points"),
                    Games = reader.GetInt32("games")
                }) ?? new Progress();
            });
        }

        public Task<List<User>> GetLastestScoresPageAsync(ScorePaginationFilters filters)
        {
            const string sql = "select encriptacion.get_latest_scores_page(" +
                "p_page := @Page," +
                "p_page_size := @PageSize," +
                "p_id_user := @IdUser" +
                ") as result;";

            return RepositoryGuard.ExecuteAsync(ErrorMessages.Scores.GetLatestScoresPage, async () =>
            {
                var statement = new SqlStatement(sql,
                    new SqlParam("@Page", filters.Page),
                    new SqlParam("@PageSize", filters.PageSize),
                    new SqlParam("@IdUser", filters.IdUser));

                return await _sqlExecutor.QueryFirstOrDefaultAsync(statement,
                    reader => JsonSerializer.Deserialize<List<User>>(reader.GetString("result"), serializeOptionsJson))
                    ?? new List<User>();
            });
        }

        public Task<UserGlobalProgress> GetUserGlobalProgressAsync(Guid Id)
        {
            const string sql = "select encriptacion.get_user_global_progress(" +
                "p_id := @Id" +
                ") as result;";

            return RepositoryGuard.ExecuteAsync(ErrorMessages.Scores.GetUserGlobalProgress, async () =>
            {
                var statement = new SqlStatement(sql, new SqlParam("@Id", Id));

                return await _sqlExecutor.QueryFirstOrDefaultAsync(statement,
                    reader => JsonSerializer.Deserialize<UserGlobalProgress>(reader.GetString("result"), serializeOptionsJson))
                    ?? new UserGlobalProgress();
            });
        }
    }
}
