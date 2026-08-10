using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UserAPI.Core.Interface;
using UserAPI.Core.Models;
using UserAPI.Core.Models.Pagination;
using UserAPI.Core.Models.QuestionsDTOs;
using UserAPI.DataAccess.Interface;

namespace UserAPI.Core.Implementation
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly JsonSerializerOptions serializeOptionsJson = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<QuestionRepository> _logger;

        public QuestionRepository(IUnitOfWork unitOfWork, ILogger<QuestionRepository> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Guid?> CreateQuestionWithOptionsAsync(Questions question)
        {
            const string sql = "select * from encriptacion.insert_question_with_options(" +
                "p_question := @Question" +
                ");";
            if (question.Options == null || question.Options.Count() != 4)
                throw new Exception("La pregunta debe contar con 4 repuestas");
            if (question?.IdSubject == null)
                throw new Exception("La pregunta debe pertenecer a una materia");
            try
            {
                var Question = JsonSerializer.Serialize(question, serializeOptionsJson);
                await _unitOfWork.EnsureConnectionAsync();

                await using var command = new NpgsqlCommand(sql, _unitOfWork.Connection, _unitOfWork.Transaction)
                {
                    CommandType = CommandType.Text
                };

                command.Parameters.AddWithValue("@Question", NpgsqlTypes.NpgsqlDbType.Jsonb, Question);

                var result = await command.ExecuteScalarAsync();

                return result != null ? (Guid)result : null;
            }
            catch (PostgresException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error during create question with options", ex);
            }
        }

        public async Task<Guid?> DeleteQuestionAsync(Guid Id)
        {
            const string sql = "select * from encriptacion.soft_delete_question(" +
                "p_id :=@Id" +
                ");";
            try
            {
                await _unitOfWork.EnsureConnectionAsync();

                await using var command = new NpgsqlCommand(sql, _unitOfWork.Connection, _unitOfWork.Transaction)
                {
                    CommandType = CommandType.Text
                };

                command.Parameters.AddWithValue("@Id", Id);

                var result = await command.ExecuteScalarAsync();

                return result != null ? (Guid)result : null;
            }
            catch (PostgresException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error during deleting question with options", ex);
            }
        }

        public async Task<Questions> GetQuestionWithAnswersByQuestionIdAsync(Guid Id)
        {
            const string sql = "select encriptacion.get_question_with_answers_by_question_id(" +
                "p_id := @Id" +
                ") as response;";
            try
            {
                await _unitOfWork.EnsureConnectionAsync();

                await using var command = new NpgsqlCommand(sql, _unitOfWork.Connection, _unitOfWork.Transaction)
                {
                    CommandType = CommandType.Text
                };

                command.Parameters.AddWithValue("@Id", Id);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var result = JsonSerializer.Deserialize<Questions>(reader.GetString("response"), serializeOptionsJson);

                    return result!;
                }
                return new Questions();
            }
            catch (PostgresException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error during get question with options by question id", ex);
            }
        }

        public async Task<List<Questions>> GetQuestionWithAnswersPageAsync(QuestionPaginationFilters filters)
        {
            const string sql = "select * from encriptacion.get_question_with_answers_page(" +
                "p_page := @Page," +
                "p_page_size := @PageSize," +
                "p_subject_id := @IdSubject" +
                ");";

            var questions = new List<Questions>();

            try
            {
                await _unitOfWork.EnsureConnectionAsync();

                await using var command = new NpgsqlCommand(sql, _unitOfWork.Connection, _unitOfWork.Transaction)
                {
                    CommandType = CommandType.Text
                };

                command.Parameters.AddWithValue("@Page", filters.Page);
                command.Parameters.AddWithValue("@PageSize", filters.PageSize);
                command.Parameters.AddWithValue("@IdSubject", filters.IdSubject ?? (object)DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                while (await  reader.ReadAsync())
                {
                    var options = JsonSerializer.Deserialize<List<Options>>(reader.GetString("options"), serializeOptionsJson);
                    questions.Add(new Questions
                    {
                        Id = reader.GetGuid("id_question"),
                        Question = reader.GetString("question"),
                        SubjectName = reader.GetString("subject_name"),
                        Options = options
                    });
                }

                return questions;
            }
            catch (PostgresException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error during get question with options page", ex);
            }
        }

        public async Task<List<Questions>> GetRandomQuestionsAsync(SetQuestions set)
        {
            const string sql = "select encriptacion.get_random_questions(" +
                "p_total_questions := @TotalQuestions," +
                "p_subject_id := @IdSubject" +
                ") as response;";
            try
            {
                await _unitOfWork.EnsureConnectionAsync();

                await using var command = new NpgsqlCommand(sql, _unitOfWork.Connection, _unitOfWork.Transaction)
                {
                    CommandType = CommandType.Text
                };

                command.Parameters.AddWithValue("@TotalQuestions", set.TotalQuestions);
                command.Parameters.AddWithValue("@IdSubject", set.IdSubject ?? (object)DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var response = JsonSerializer.Deserialize<List<Questions>>(reader.GetString("response"), serializeOptionsJson);

                    return response != null ? response.ToList() : new List<Questions>();
                }

                return new List<Questions>();
            }
            catch (PostgresException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error during get random questions", ex);
            }
        }

        public async Task<Guid?> UpdateQuestionAndOptionsAsync(Questions question)
        {
            const string sql = "select * from encriptacion.update_question_and_options(" +
                "p_question := @Question" +
                ");";
            if (question?.Options?.Count() > 4)
                throw new Exception("La pregunta debe contar con más de 4 repuestas");
            try
            {
                var Question = JsonSerializer.Serialize(question, serializeOptionsJson);
                await _unitOfWork.EnsureConnectionAsync();

                await using var command = new NpgsqlCommand(sql, _unitOfWork.Connection, _unitOfWork.Transaction)
                {
                    CommandType = CommandType.Text
                };

                command.Parameters.AddWithValue("@Question", NpgsqlTypes.NpgsqlDbType.Jsonb, Question);

                var result = await command.ExecuteScalarAsync();

                return result != null ? (Guid)result : null;
            }
            catch (PostgresException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error during updating question with options", ex);
            }
        }
    }
}
