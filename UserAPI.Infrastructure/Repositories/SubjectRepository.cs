using System.Data;
using System.Data.Common;
using UserAPI.Application.Common;
using UserAPI.Application.Interfaces.Repositories;
using UserAPI.Domain.Entities;
using UserAPI.Infrastructure.Persistence.Sql;

namespace UserAPI.Infrastructure.Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly ISqlExecutor _sqlExecutor;

        public SubjectRepository(ISqlExecutor sqlExecutor) => _sqlExecutor = sqlExecutor;

        public Task<Subjects> CreateSubjectAsync(Subjects subject)
        {
            const string sql = "select * from encriptacion.insert_subject(" +
                "p_name := @Name," +
                "p_description := @Description" +
                ");";

            return RepositoryGuard.ExecuteAsync(ErrorMessages.Subjects.Create, async () =>
            {
                var statement = new SqlStatement(sql,
                    new SqlParam("@Name", subject.Name),
                    new SqlParam("@Description", subject.Description));

                return await _sqlExecutor.QueryFirstOrDefaultAsync(statement, MapSubject) ?? new Subjects();
            });
        }

        public Task<Subjects> DeleteSubjectAsync(Guid Id)
        {
            const string sql = "select * from encriptacion.soft_delete_subject(" +
                "p_id := @Id" +
                ");";

            return RepositoryGuard.ExecuteAsync(ErrorMessages.Subjects.Delete, async () =>
            {
                var statement = new SqlStatement(sql, new SqlParam("@Id", Id));

                return await _sqlExecutor.QueryFirstOrDefaultAsync(statement, MapSubject) ?? new Subjects();
            });
        }

        public Task<List<Subjects>> GetSubjectsForOptionsAsync()
        {
            const string sql = " select * from encriptacion.get_subjects_for_options();";

            return RepositoryGuard.ExecuteAsync(ErrorMessages.Subjects.GetForOptions,
                () => _sqlExecutor.QueryAsync(new SqlStatement(sql), reader => new Subjects
                {
                    Id = reader.GetGuid("id"),
                    Name = reader.GetString("name")
                }));
        }

        public Task<List<Subjects>> GetSubjectWithQuestionsCountAsync()
        {
            const string sql = "select * from encriptacion.get_subjects_with_questions_count()";

            return RepositoryGuard.ExecuteAsync(ErrorMessages.Subjects.GetWithQuestionsCount,
                () => _sqlExecutor.QueryAsync(new SqlStatement(sql), reader => new Subjects
                {
                    Id = reader.GetGuid("subject_id"),
                    Name = reader.GetString("subject_name"),
                    TotalQuestions = reader.GetInt32("total_questions")
                }));
        }

        public Task<Subjects> UpdateSubjectAsync(Subjects subject)
        {
            const string sql = "select * from encriptacion.update_subject(" +
                "p_id := @Id," +
                "p_name := @Name," +
                "p_description := @Description" +
                ");";

            return RepositoryGuard.ExecuteAsync(ErrorMessages.Subjects.Update, async () =>
            {
                var statement = new SqlStatement(sql,
                    new SqlParam("@Id", subject.Id),
                    new SqlParam("@Name", subject.Name),
                    new SqlParam("@Description", subject.Description));

                return await _sqlExecutor.QueryFirstOrDefaultAsync(statement, MapSubject) ?? new Subjects();
            });
        }

        private static Subjects MapSubject(DbDataReader reader) => new()
        {
            Id = reader.GetGuid("id"),
            Name = reader.GetString("name"),
            Description = reader.GetString("description"),
            IsActive = reader.GetBoolean("is_active"),
        };
    }
}
