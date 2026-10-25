using System.Data;
using UserAPI.Application.Common;
using UserAPI.Application.Common.Exceptions;
using UserAPI.Domain.Entities;
using UserAPI.Infrastructure.Repositories;
using UserAPI.UnitTests.Helpers;

namespace UserAPI.UnitTests.Infrastructure.Repositories
{
    public class SubjectRepositoryTests
    {
        private static readonly Guid SubjectId = Guid.Parse("1f2e3d4c-5b6a-4978-8a9b-0c1d2e3f4a5b");

        private readonly FakeSqlExecutor _executor = new();
        private readonly SubjectRepository _repository;

        public SubjectRepositoryTests() => _repository = new SubjectRepository(_executor);

        public static TheoryData<string> Operations => new()
        {
            nameof(SubjectRepository.CreateSubjectAsync),
            nameof(SubjectRepository.UpdateSubjectAsync),
            nameof(SubjectRepository.DeleteSubjectAsync),
            nameof(SubjectRepository.GetSubjectsForOptionsAsync),
            nameof(SubjectRepository.GetSubjectWithQuestionsCountAsync)
        };

        public static TheoryData<string, string> OperationErrorMessages => new()
        {
            { nameof(SubjectRepository.CreateSubjectAsync), ErrorMessages.Subjects.Create },
            { nameof(SubjectRepository.UpdateSubjectAsync), ErrorMessages.Subjects.Update },
            { nameof(SubjectRepository.DeleteSubjectAsync), ErrorMessages.Subjects.Delete },
            { nameof(SubjectRepository.GetSubjectsForOptionsAsync), ErrorMessages.Subjects.GetForOptions },
            { nameof(SubjectRepository.GetSubjectWithQuestionsCountAsync), ErrorMessages.Subjects.GetWithQuestionsCount }
        };

        [Fact]
        public async Task CreateSubjectAsync_SendsSubjectToInsertFunctionAndReturnsCreatedSubject()
        {
            _executor.Rows = SubjectRows((SubjectId, "Matemáticas", "Álgebra y geometría", true));

            var subject = await _repository.CreateSubjectAsync(NewSubject());

            var statement = _executor.ExecutedStatement;
            SqlAssert.CallsFunction(statement, "insert_subject");
            Assert.Equal("Matemáticas", SqlAssert.Parameter(statement, "@Name"));
            Assert.Equal("Álgebra y geometría", SqlAssert.Parameter(statement, "@Description"));
            AssertSubject(SubjectId, "Matemáticas", "Álgebra y geometría", true, subject);
        }

        [Fact]
        public async Task CreateSubjectAsync_WhenFunctionReturnsNoRows_ReturnsEmptySubject()
        {
            _executor.Rows = SubjectRows();

            var subject = await _repository.CreateSubjectAsync(NewSubject());

            Assert.Null(subject.Id);
        }

        [Fact]
        public async Task UpdateSubjectAsync_SendsIdAndChangesAndReturnsUpdatedSubject()
        {
            var changes = NewSubject();
            changes.Id = SubjectId;
            changes.Name = "Matemáticas II";
            _executor.Rows = SubjectRows((SubjectId, "Matemáticas II", "Álgebra y geometría", true));

            var subject = await _repository.UpdateSubjectAsync(changes);

            var statement = _executor.ExecutedStatement;
            SqlAssert.CallsFunction(statement, "update_subject");
            Assert.Equal(SubjectId, SqlAssert.Parameter(statement, "@Id"));
            Assert.Equal("Matemáticas II", SqlAssert.Parameter(statement, "@Name"));
            Assert.Equal("Álgebra y geometría", SqlAssert.Parameter(statement, "@Description"));
            AssertSubject(SubjectId, "Matemáticas II", "Álgebra y geometría", true, subject);
        }

        [Fact]
        public async Task DeleteSubjectAsync_SendsIdToSoftDeleteAndReturnsInactiveSubject()
        {
            _executor.Rows = SubjectRows((SubjectId, "Matemáticas", "Álgebra y geometría", false));

            var subject = await _repository.DeleteSubjectAsync(SubjectId);

            var statement = _executor.ExecutedStatement;
            SqlAssert.CallsFunction(statement, "soft_delete_subject");
            Assert.Equal(SubjectId, SqlAssert.Parameter(statement, "@Id"));
            AssertSubject(SubjectId, "Matemáticas", "Álgebra y geometría", false, subject);
        }

        [Fact]
        public async Task GetSubjectsForOptionsAsync_MapsIdAndNameOfEveryRow()
        {
            var otherId = Guid.NewGuid();
            _executor.Rows = Rows.Table(("id", typeof(Guid)), ("name", typeof(string)));
            _executor.Rows.Rows.Add(SubjectId, "Matemáticas");
            _executor.Rows.Rows.Add(otherId, "Historia");

            var subjects = await _repository.GetSubjectsForOptionsAsync();

            SqlAssert.CallsFunction(_executor.ExecutedStatement, "get_subjects_for_options");
            Assert.Collection(subjects,
                s =>
                {
                    Assert.Equal(SubjectId, s.Id);
                    Assert.Equal("Matemáticas", s.Name);
                },
                s =>
                {
                    Assert.Equal(otherId, s.Id);
                    Assert.Equal("Historia", s.Name);
                });
        }

        [Fact]
        public async Task GetSubjectWithQuestionsCountAsync_MapsCountColumns()
        {
            _executor.Rows = Rows.Table(("subject_id", typeof(Guid)), ("subject_name", typeof(string)), ("total_questions", typeof(int)));
            _executor.Rows.Rows.Add(SubjectId, "Matemáticas", 12);

            var subjects = await _repository.GetSubjectWithQuestionsCountAsync();

            SqlAssert.CallsFunction(_executor.ExecutedStatement, "get_subjects_with_questions_count");
            var subject = Assert.Single(subjects);
            Assert.Equal(SubjectId, subject.Id);
            Assert.Equal("Matemáticas", subject.Name);
            Assert.Equal(12, subject.TotalQuestions);
        }

        [Theory]
        [MemberData(nameof(Operations))]
        public async Task Operation_BindsEverySqlPlaceholder(string operation)
        {
            await InvokeAsync(operation);

            SqlAssert.BindsEveryPlaceholder(_executor.ExecutedStatement);
        }

        [Theory]
        [MemberData(nameof(Operations))]
        public async Task Operation_WhenPostgresRejectsIt_PropagatesBusinessRule(string operation)
        {
            var postgresError = new BusinessRuleException(DatabaseErrorCodes.BusinessRule, "La materia con ese nombre ya existe");
            _executor.Exception = postgresError;

            var exception = await Assert.ThrowsAsync<BusinessRuleException>(() => InvokeAsync(operation));

            Assert.Same(postgresError, exception);
        }

        [Theory]
        [MemberData(nameof(OperationErrorMessages))]
        public async Task Operation_WhenExecutionFailsUnexpectedly_WrapsError(string operation, string expectedMessage)
        {
            var connectionError = new TimeoutException("Connection timed out");
            _executor.Exception = connectionError;

            var exception = await Assert.ThrowsAsync<UnexpectedException>(() => InvokeAsync(operation));

            Assert.Equal(expectedMessage, exception.Message);
            Assert.Same(connectionError, exception.InnerException);
        }

        private Task InvokeAsync(string operation) => operation switch
        {
            nameof(SubjectRepository.CreateSubjectAsync) => _repository.CreateSubjectAsync(NewSubject()),
            nameof(SubjectRepository.UpdateSubjectAsync) => _repository.UpdateSubjectAsync(NewSubject()),
            nameof(SubjectRepository.DeleteSubjectAsync) => _repository.DeleteSubjectAsync(SubjectId),
            nameof(SubjectRepository.GetSubjectsForOptionsAsync) => _repository.GetSubjectsForOptionsAsync(),
            nameof(SubjectRepository.GetSubjectWithQuestionsCountAsync) => _repository.GetSubjectWithQuestionsCountAsync(),
            _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
        };

        private static Subjects NewSubject() => new()
        {
            Id = SubjectId,
            Name = "Matemáticas",
            Description = "Álgebra y geometría"
        };

        // Columnas de la tabla encriptacion.subjects que lee el repositorio
        private static DataTable SubjectRows(params (Guid Id, string Name, string Description, bool IsActive)[] rows)
        {
            var table = Rows.Table(
                ("id", typeof(Guid)),
                ("name", typeof(string)),
                ("description", typeof(string)),
                ("is_active", typeof(bool)));

            foreach (var row in rows)
                table.Rows.Add(row.Id, row.Name, row.Description, row.IsActive);

            return table;
        }

        private static void AssertSubject(Guid id, string name, string description, bool isActive, Subjects actual)
        {
            Assert.Equal(id, actual.Id);
            Assert.Equal(name, actual.Name);
            Assert.Equal(description, actual.Description);
            Assert.Equal(isActive, actual.IsActive);
        }
    }
}
