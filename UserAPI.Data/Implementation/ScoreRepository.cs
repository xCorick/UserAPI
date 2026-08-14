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
using UserAPI.Core.Models.SubjectProgress;
using UserAPI.DataAccess.Interface;

namespace UserAPI.Core.Implementation
{
    public class ScoreRepository : IScoreRepository
    {
        private readonly JsonSerializerOptions serializeOptionsJson = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ScoreRepository> _logger;

        public ScoreRepository(IUnitOfWork unitOfWork, ILogger<ScoreRepository> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Scores> CreateScoreAsync(Scores scores)
        {
            const string sql = "select * from encriptacion.insert_score(" +
                "p_score := @Score," +
                "p_id_user := @IdUser," +
                "p_is_mixed := @IsMixed," +
                "p_points := @Points," +
                "p_id_subject := @IdSubject" +
                ");";
            try
            {
                await _unitOfWork.EnsureConnectionAsync();

                await using var command = new NpgsqlCommand(sql, _unitOfWork.Connection, _unitOfWork.Transaction)
                {
                    CommandType = CommandType.Text
                };

                command.Parameters.AddWithValue("@Score", scores.Score);
                command.Parameters.AddWithValue("@IdUser", scores.IdUser);
                command.Parameters.AddWithValue("@IsMixed", scores.IsMixed);
                command.Parameters.AddWithValue("@Points", scores.Points);
                command.Parameters.AddWithValue("@IdSubject", scores.IdSubject ?? (object)DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                if (await  reader.ReadAsync())
                {
                    return new Scores
                    {
                        Id = reader.GetGuid("id"),
                        Score = reader.GetDecimal("score"),
                        Points = reader.GetInt32("points"),
                        IdSubject = reader.IsDBNull("id_subject") ?
                            null : reader.GetGuid("id_subject"),
                        IsMixed = reader.GetBoolean("is_mixed")
                    };
                }
                return new Scores();
            }
            catch (PostgresException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error during create score", ex);
            }
        }

        public async Task<Progress> GetGeneralProgressAsync(Guid Id)
        {
            const string sql = "select * from encriptacion.get_general_progress(" +
                "p_id := @Id" +
                ");";
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
                    return new Progress
                    {
                        Score = reader.GetDecimal("score"),
                        Points = reader.GetInt32("points"),
                        Games = reader.GetInt32("games")
                    };
                }
                return new Progress();
            }
            catch (PostgresException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error during getting general progress", ex);
            }
        }

        public async Task<List<User>> GetLastestScoresPageAsync(ScorePaginationFilters filters)
        {
            const string sql = "select encriptacion.get_latest_scores_page(" +
                "p_page := @Page," +
                "p_page_size := @PageSize," +
                "p_id_user := @IdUser" +
                ") as result;";
            try
            {
                await _unitOfWork.EnsureConnectionAsync();

                await using var command = new NpgsqlCommand(sql, _unitOfWork.Connection, _unitOfWork.Transaction)
                {
                    CommandType = CommandType.Text
                };

                command.Parameters.AddWithValue("@Page", filters.Page);
                command.Parameters.AddWithValue("@PageSize", filters.PageSize);
                command.Parameters.AddWithValue("@IdUser", filters.IdUser);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var response = JsonSerializer.Deserialize<List<User>>(reader.GetString("result"), serializeOptionsJson);

                    return response != null ? response.ToList() : new List<User>();
                }

                return new List<User>();
            }
            catch (PostgresException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error during getting scores paged", ex);
            }
        }

        public async Task<UserGlobalProgress> GetUserGlobalProgressAsync(Guid Id)
        {
            const string sql = "select encriptacion.get_user_global_progress(" +
                "p_id := @Id" +
                ") as result;";

            try
            {
                await _unitOfWork.EnsureConnectionAsync();

                await using var command = new NpgsqlCommand(
                    sql,
                    _unitOfWork.Connection,
                    _unitOfWork.Transaction
                )
                {
                    CommandType = CommandType.Text
                };

                command.Parameters.AddWithValue("@Id", Id);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var response = JsonSerializer.Deserialize<UserGlobalProgress>(
                        reader.GetString("result"),
                        serializeOptionsJson
                    );

                    return response ?? new UserGlobalProgress();
                }

                return new UserGlobalProgress();
            }
            catch (PostgresException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Unexpected error during getting user global progress",
                    ex
                );
            }
        }
    }
}
