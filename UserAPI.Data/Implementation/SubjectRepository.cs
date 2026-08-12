using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using UserAPI.Core.Interface;
using UserAPI.Core.Models;
using UserAPI.Data.Models;
using UserAPI.DataAccess.Interface;

namespace UserAPI.Core.Implementation
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SubjectRepository> _logger;

        public SubjectRepository(IUnitOfWork unitOfWork, ILogger<SubjectRepository> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Subjects> CreateSubjectAsync(Subjects subject)
        {
            const string sql = "select * from encriptacion.insert_subject(" +
                "p_name := @Name," +
                "p_description := @Description" +
                ");";
            try
            {
                await _unitOfWork.EnsureConnectionAsync();

                await using var command = new NpgsqlCommand(sql, _unitOfWork.Connection, _unitOfWork.Transaction)
                {
                    CommandType = CommandType.Text
                };

                command.Parameters.AddWithValue("@Name", subject.Name!);
                command.Parameters.AddWithValue("@Description", subject.Description!);

                await using var reader = await command.ExecuteReaderAsync();

                if (await  reader.ReadAsync())
                {
                    return new Subjects
                    {
                        Id = reader.GetGuid("id"),
                        Name = reader.GetString("name"),
                        Description = reader.GetString("description"),
                        IsActive = reader.GetBoolean("is_active"),
                    };
                }
            }
            catch (PostgresException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error during create subject", ex);
            }
            return new Subjects();
        }

        public async Task<Subjects> DeleteSubjectAsync(Guid Id)
        {
            const string sql = "select * from encriptacion.soft_delete_subject(" +
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
                    return new Subjects
                    {
                        Id = reader.GetGuid("id"),
                        Name = reader.GetString("name"),
                        Description = reader.GetString("description"),
                        IsActive = reader.GetBoolean("is_active"),
                    };
                }
            }
            catch (PostgresException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error during deleting subject", ex);
            }
            return new Subjects();
        }

        public async Task<List<Subjects>> GetSubjectsForOptionsAsync()
        {
            const string sql = " select * from encriptacion.get_subjects_for_options();";
            var subjects = new List<Subjects>();
            try
            {
                await _unitOfWork.EnsureConnectionAsync();

                await using var command = new NpgsqlCommand(sql, _unitOfWork.Connection, _unitOfWork.Transaction)
                {
                    CommandType = CommandType.Text
                };

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    subjects.Add(new Subjects
                    {
                        Id = reader.GetGuid("id"),
                        Name = reader.GetString("name")
                    });
                }
            }
            catch (PostgresException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error during getting subject questions for options", ex);
            }
            return subjects;
        }

        public async Task<List<Subjects>> GetSubjectWithQuestionsCountAsync()
        {
            const string sql = "select * from encriptacion.get_subjects_with_questions_count()";
            var subjects = new List<Subjects>();
            try
            {
                await _unitOfWork.EnsureConnectionAsync();

                await using var command = new NpgsqlCommand(sql, _unitOfWork.Connection, _unitOfWork.Transaction)
                {
                    CommandType = CommandType.Text
                };

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    subjects.Add(new Subjects
                    {
                        Id = reader.GetGuid("subject_id"),
                        Name = reader.GetString("subject_name"),
                        TotalQuestions = reader.GetInt32("total_questions")
                    });
                }
            }
            catch (PostgresException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error during getting subject questions count", ex);
            }
            return subjects;
        }

        public async Task<Subjects> UpdateSubjectAsync(Subjects subject)
        {
            const string sql = "select * from encriptacion.update_subject(" +
                "p_id := @Id," +
                "p_name := @Name," +
                "p_description := @Description" +
                ");";
            try
            {
                await _unitOfWork.EnsureConnectionAsync();

                await using var command = new NpgsqlCommand(sql, _unitOfWork.Connection, _unitOfWork.Transaction)
                {
                    CommandType = CommandType.Text
                };

                command.Parameters.AddWithValue("@Id", subject.Id);
                command.Parameters.AddWithValue("@Name", subject.Name!);
                command.Parameters.AddWithValue("@Description", subject.Description!);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new Subjects
                    {
                        Id = reader.GetGuid("id"),
                        Name = reader.GetString("name"),
                        Description = reader.GetString("description"),
                        IsActive = reader.GetBoolean("is_active"),
                    };
                }
            }
            catch (PostgresException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error during create subject", ex);
            }
            return new Subjects();
        }
    }
}
