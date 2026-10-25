using System.Data;
using System.Text.Json;
using UserAPI.Application.Common;
using UserAPI.Application.DTOs;
using UserAPI.Application.Interfaces.Repositories;
using UserAPI.Domain.Entities;
using UserAPI.Infrastructure.Persistence.Sql;

namespace UserAPI.Infrastructure.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private static readonly JsonSerializerOptions serializeOptionsJson = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        private readonly ISqlExecutor _sqlExecutor;

        public QuestionRepository(ISqlExecutor sqlExecutor) => _sqlExecutor = sqlExecutor;

        public Task<Guid?> CreateQuestionWithOptionsAsync(Questions question)
        {
            const string sql = "select * from encriptacion.insert_question_with_options(" +
                "p_question := @Question" +
                ");";

            return RepositoryGuard.ExecuteAsync<Guid?>(ErrorMessages.Questions.Create, async () =>
            {
                var statement = new SqlStatement(sql,
                    new SqlParam("@Question", JsonSerializer.Serialize(question, serializeOptionsJson), SqlParamType.Jsonb));

                var result = await _sqlExecutor.ExecuteScalarAsync(statement);

                return result != null ? (Guid)result : null;
            });
        }

        public Task<Guid?> DeleteQuestionAsync(Guid Id)
        {
            const string sql = "select * from encriptacion.soft_delete_question(" +
                "p_id :=@Id" +
                ");";

            return RepositoryGuard.ExecuteAsync<Guid?>(ErrorMessages.Questions.Delete, async () =>
            {
                var result = await _sqlExecutor.ExecuteScalarAsync(new SqlStatement(sql, new SqlParam("@Id", Id)));

                return result != null ? (Guid)result : null;
            });
        }

        public Task<Questions> GetQuestionWithAnswersByQuestionIdAsync(Guid Id)
        {
            const string sql = "select encriptacion.get_question_with_answers_by_question_id(" +
                "p_id := @Id" +
                ") as response;";

            return RepositoryGuard.ExecuteAsync(ErrorMessages.Questions.GetById, async () =>
            {
                var statement = new SqlStatement(sql, new SqlParam("@Id", Id));

                return await _sqlExecutor.QueryFirstOrDefaultAsync(statement,
                    reader => JsonSerializer.Deserialize<Questions>(reader.GetString("response"), serializeOptionsJson)!)
                    ?? new Questions();
            });
        }

        public Task<List<Questions>> GetQuestionWithAnswersPageAsync(QuestionPaginationFilters filters)
        {
            const string sql = "select * from encriptacion.get_question_with_answers_page(" +
                "p_page := @Page," +
                "p_page_size := @PageSize," +
                "p_subject_id := @IdSubject" +
                ");";

            return RepositoryGuard.ExecuteAsync(ErrorMessages.Questions.GetPage, () =>
            {
                var statement = new SqlStatement(sql,
                    new SqlParam("@Page", filters.Page),
                    new SqlParam("@PageSize", filters.PageSize),
                    new SqlParam("@IdSubject", filters.IdSubject ?? (object)DBNull.Value));

                return _sqlExecutor.QueryAsync(statement, reader => new Questions
                {
                    Id = reader.GetGuid("id_question"),
                    Question = reader.GetString("question"),
                    SubjectName = reader.GetString("subject_name"),
                    Options = JsonSerializer.Deserialize<List<Options>>(reader.GetString("options"), serializeOptionsJson)
                });
            });
        }

        public Task<List<Questions>> GetRandomQuestionsAsync(SetQuestions set)
        {
            const string sql = "select encriptacion.get_random_questions(" +
                "p_total_questions := @TotalQuestions," +
                "p_subject_id := @IdSubject" +
                ") as response;";

            return RepositoryGuard.ExecuteAsync(ErrorMessages.Questions.GetRandom, async () =>
            {
                var statement = new SqlStatement(sql,
                    new SqlParam("@TotalQuestions", set.TotalQuestions),
                    new SqlParam("@IdSubject", set.IdSubject ?? (object)DBNull.Value));

                return await _sqlExecutor.QueryFirstOrDefaultAsync(statement,
                    reader => JsonSerializer.Deserialize<List<Questions>>(reader.GetString("response"), serializeOptionsJson))
                    ?? new List<Questions>();
            });
        }

        public Task<Guid?> UpdateQuestionAndOptionsAsync(Questions question)
        {
            const string sql = "select * from encriptacion.update_question_and_options(" +
                "p_question := @Question" +
                ");";

            return RepositoryGuard.ExecuteAsync<Guid?>(ErrorMessages.Questions.Update, async () =>
            {
                var statement = new SqlStatement(sql,
                    new SqlParam("@Question", JsonSerializer.Serialize(question, serializeOptionsJson), SqlParamType.Jsonb));

                var result = await _sqlExecutor.ExecuteScalarAsync(statement);

                return result != null ? (Guid)result : null;
            });
        }
    }
}
