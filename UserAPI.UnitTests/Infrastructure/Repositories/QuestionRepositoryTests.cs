using System.Data;
using System.Text.Json;
using UserAPI.Application.Common;
using UserAPI.Application.Common.Exceptions;
using UserAPI.Application.DTOs;
using UserAPI.Domain.Entities;
using UserAPI.Infrastructure.Persistence.Sql;
using UserAPI.Infrastructure.Repositories;
using UserAPI.UnitTests.Helpers;

namespace UserAPI.UnitTests.Infrastructure.Repositories
{
    public class QuestionRepositoryTests
    {
        private static readonly Guid QuestionId = Guid.Parse("8a0b5f2e-1c3d-4e5f-9a7b-6c5d4e3f2a1b");
        private static readonly Guid SubjectId = Guid.Parse("1f2e3d4c-5b6a-4978-8a9b-0c1d2e3f4a5b");

        private readonly FakeSqlExecutor _executor = new();
        private readonly QuestionRepository _repository;

        public QuestionRepositoryTests() => _repository = new QuestionRepository(_executor);

        public static TheoryData<string> Operations => new()
        {
            nameof(QuestionRepository.CreateQuestionWithOptionsAsync),
            nameof(QuestionRepository.UpdateQuestionAndOptionsAsync),
            nameof(QuestionRepository.DeleteQuestionAsync),
            nameof(QuestionRepository.GetQuestionWithAnswersByQuestionIdAsync),
            nameof(QuestionRepository.GetQuestionWithAnswersPageAsync),
            nameof(QuestionRepository.GetRandomQuestionsAsync)
        };

        public static TheoryData<string, string> OperationErrorMessages => new()
        {
            { nameof(QuestionRepository.CreateQuestionWithOptionsAsync), ErrorMessages.Questions.Create },
            { nameof(QuestionRepository.UpdateQuestionAndOptionsAsync), ErrorMessages.Questions.Update },
            { nameof(QuestionRepository.DeleteQuestionAsync), ErrorMessages.Questions.Delete },
            { nameof(QuestionRepository.GetQuestionWithAnswersByQuestionIdAsync), ErrorMessages.Questions.GetById },
            { nameof(QuestionRepository.GetQuestionWithAnswersPageAsync), ErrorMessages.Questions.GetPage },
            { nameof(QuestionRepository.GetRandomQuestionsAsync), ErrorMessages.Questions.GetRandom }
        };

        [Fact]
        public async Task CreateQuestionWithOptionsAsync_SendsQuestionAsJsonbWithTheKeysTheFunctionReads()
        {
            _executor.ScalarResult = QuestionId;

            await _repository.CreateQuestionWithOptionsAsync(NewQuestion());

            var statement = _executor.ExecutedStatement;
            SqlAssert.CallsFunction(statement, "insert_question_with_options");
            var parameter = SqlAssert.ParameterInfo(statement, "@Question");
            Assert.Equal(SqlParamType.Jsonb, parameter.Type);

            // insert_question_with_options lee: question, idSubject, options[].option y options[]."isAnswer"
            using var json = JsonDocument.Parse((string)parameter.Value!);
            var root = json.RootElement;
            Assert.Equal("¿Cuál es la capital de México?", root.GetProperty("question").GetString());
            Assert.Equal(SubjectId, root.GetProperty("idSubject").GetGuid());
            var options = root.GetProperty("options").EnumerateArray().ToList();
            Assert.Equal(4, options.Count);
            Assert.Equal("Ciudad de México", options[0].GetProperty("option").GetString());
            Assert.True(options[0].GetProperty("isAnswer").GetBoolean());
            Assert.All(options.Skip(1), o => Assert.False(o.GetProperty("isAnswer").GetBoolean()));
        }

        [Fact]
        public async Task CreateQuestionWithOptionsAsync_ReturnsIdGeneratedByDatabase()
        {
            _executor.ScalarResult = QuestionId;

            var id = await _repository.CreateQuestionWithOptionsAsync(NewQuestion());

            Assert.Equal(QuestionId, id);
        }

        [Fact]
        public async Task CreateQuestionWithOptionsAsync_WhenNoIdIsReturned_ReturnsNull()
        {
            _executor.ScalarResult = null;

            var id = await _repository.CreateQuestionWithOptionsAsync(NewQuestion());

            Assert.Null(id);
        }

        [Fact]
        public async Task UpdateQuestionAndOptionsAsync_SendsQuestionAndOptionIdsAsJsonb()
        {
            var question = NewQuestion();
            question.Id = QuestionId;
            var optionId = Guid.NewGuid();
            question.Options![0].Id = optionId;
            _executor.ScalarResult = QuestionId;

            var id = await _repository.UpdateQuestionAndOptionsAsync(question);

            var statement = _executor.ExecutedStatement;
            SqlAssert.CallsFunction(statement, "update_question_and_options");
            var parameter = SqlAssert.ParameterInfo(statement, "@Question");
            Assert.Equal(SqlParamType.Jsonb, parameter.Type);

            // update_question_and_options busca la pregunta por "id" y cada opción por options[].id
            using var json = JsonDocument.Parse((string)parameter.Value!);
            Assert.Equal(QuestionId, json.RootElement.GetProperty("id").GetGuid());
            Assert.Equal(optionId, json.RootElement.GetProperty("options")[0].GetProperty("id").GetGuid());
            Assert.Equal(QuestionId, id);
        }

        [Fact]
        public async Task DeleteQuestionAsync_SendsIdToSoftDeleteAndReturnsDeletedId()
        {
            _executor.ScalarResult = QuestionId;

            var id = await _repository.DeleteQuestionAsync(QuestionId);

            var statement = _executor.ExecutedStatement;
            SqlAssert.CallsFunction(statement, "soft_delete_question");
            Assert.Equal(QuestionId, SqlAssert.Parameter(statement, "@Id"));
            Assert.Equal(QuestionId, id);
        }

        [Fact]
        public async Task GetQuestionWithAnswersByQuestionIdAsync_MapsJsonReturnedByFunction()
        {
            var optionId = Guid.NewGuid();
            _executor.Rows = Rows.Json("response", $$"""
                {
                    "id": "{{QuestionId}}",
                    "question": "¿Cuál es la capital de México?",
                    "idSubject": "{{SubjectId}}",
                    "options": [
                        { "id": "{{optionId}}", "option": "Ciudad de México", "isAnswer": true },
                        { "id": "{{Guid.NewGuid()}}", "option": "Guadalajara", "isAnswer": false }
                    ]
                }
                """);

            var question = await _repository.GetQuestionWithAnswersByQuestionIdAsync(QuestionId);

            Assert.Equal(QuestionId, SqlAssert.Parameter(_executor.ExecutedStatement, "@Id"));
            Assert.Equal(QuestionId, question.Id);
            Assert.Equal("¿Cuál es la capital de México?", question.Question);
            Assert.Equal(SubjectId, question.IdSubject);
            Assert.Collection(question.Options!,
                o =>
                {
                    Assert.Equal(optionId, o.Id);
                    Assert.Equal("Ciudad de México", o.Option);
                    Assert.True(o.IsAnswer);
                },
                o =>
                {
                    Assert.Equal("Guadalajara", o.Option);
                    Assert.False(o.IsAnswer);
                });
        }

        [Fact]
        public async Task GetQuestionWithAnswersByQuestionIdAsync_WhenNoRowsAreReturned_ReturnsEmptyQuestion()
        {
            _executor.Rows = Rows.Table(("response", typeof(string)));

            var question = await _repository.GetQuestionWithAnswersByQuestionIdAsync(QuestionId);

            Assert.Null(question.Id);
            Assert.Empty(question.Options!);
        }

        [Fact]
        public async Task GetQuestionWithAnswersPageAsync_SendsFiltersAndMapsEachRow()
        {
            _executor.Rows = Rows.Table(
                ("id_question", typeof(Guid)),
                ("question", typeof(string)),
                ("subject_name", typeof(string)),
                ("options", typeof(string)));
            _executor.Rows.Rows.Add(QuestionId, "¿2 + 2?", "Matemáticas",
                $$"""[{ "id": "{{Guid.NewGuid()}}", "option": "4", "idQuestion": "{{QuestionId}}", "isAnswer": true }]""");

            var questions = await _repository.GetQuestionWithAnswersPageAsync(
                new QuestionPaginationFilters { Page = 3, PageSize = 5, IdSubject = SubjectId });

            var statement = _executor.ExecutedStatement;
            SqlAssert.CallsFunction(statement, "get_question_with_answers_page");
            Assert.Equal(3, SqlAssert.Parameter(statement, "@Page"));
            Assert.Equal(5, SqlAssert.Parameter(statement, "@PageSize"));
            Assert.Equal(SubjectId, SqlAssert.Parameter(statement, "@IdSubject"));

            var question = Assert.Single(questions);
            Assert.Equal(QuestionId, question.Id);
            Assert.Equal("¿2 + 2?", question.Question);
            Assert.Equal("Matemáticas", question.SubjectName);
            var option = Assert.Single(question.Options!);
            Assert.Equal("4", option.Option);
            Assert.Equal(QuestionId, option.IdQuestion);
            Assert.True(option.IsAnswer);
        }

        [Fact]
        public async Task GetQuestionWithAnswersPageAsync_WithoutSubjectFilter_SendsDbNull()
        {
            await _repository.GetQuestionWithAnswersPageAsync(new QuestionPaginationFilters { Page = 1, PageSize = 10 });

            Assert.Equal(DBNull.Value, SqlAssert.Parameter(_executor.ExecutedStatement, "@IdSubject"));
        }

        [Fact]
        public async Task GetRandomQuestionsAsync_SendsSetAndMapsJsonArray()
        {
            _executor.Rows = Rows.Json("response", $$"""
                [
                    {
                        "id": "{{QuestionId}}",
                        "question": "¿2 + 2?",
                        "idSubject": "{{SubjectId}}",
                        "subjectName": "Matemáticas",
                        "options": [{ "id": "{{Guid.NewGuid()}}", "option": "4", "isAnswer": true }]
                    }
                ]
                """);

            var questions = await _repository.GetRandomQuestionsAsync(new SetQuestions { TotalQuestions = 10, IdSubject = SubjectId });

            var statement = _executor.ExecutedStatement;
            SqlAssert.CallsFunction(statement, "get_random_questions");
            Assert.Equal(10, SqlAssert.Parameter(statement, "@TotalQuestions"));
            Assert.Equal(SubjectId, SqlAssert.Parameter(statement, "@IdSubject"));

            var question = Assert.Single(questions);
            Assert.Equal(QuestionId, question.Id);
            Assert.Equal("Matemáticas", question.SubjectName);
            Assert.Single(question.Options!);
        }

        [Fact]
        public async Task GetRandomQuestionsAsync_WithoutSubject_SendsDbNull()
        {
            _executor.Rows = Rows.Json("response", "[]");

            var questions = await _repository.GetRandomQuestionsAsync(new SetQuestions { TotalQuestions = 10 });

            Assert.Equal(DBNull.Value, SqlAssert.Parameter(_executor.ExecutedStatement, "@IdSubject"));
            Assert.Empty(questions);
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
            var postgresError = new BusinessRuleException(DatabaseErrorCodes.RaiseException, "Error de prueba");
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

        private Task InvokeAsync(string operation)
        {
            _executor.ScalarResult = QuestionId;

            return operation switch
            {
                nameof(QuestionRepository.CreateQuestionWithOptionsAsync) => _repository.CreateQuestionWithOptionsAsync(NewQuestion()),
                nameof(QuestionRepository.UpdateQuestionAndOptionsAsync) => _repository.UpdateQuestionAndOptionsAsync(NewQuestion()),
                nameof(QuestionRepository.DeleteQuestionAsync) => _repository.DeleteQuestionAsync(QuestionId),
                nameof(QuestionRepository.GetQuestionWithAnswersByQuestionIdAsync) => _repository.GetQuestionWithAnswersByQuestionIdAsync(QuestionId),
                nameof(QuestionRepository.GetQuestionWithAnswersPageAsync) => _repository.GetQuestionWithAnswersPageAsync(
                    new QuestionPaginationFilters { Page = 1, PageSize = 10, IdSubject = SubjectId }),
                nameof(QuestionRepository.GetRandomQuestionsAsync) => _repository.GetRandomQuestionsAsync(
                    new SetQuestions { TotalQuestions = 10, IdSubject = SubjectId }),
                _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
            };
        }

        private static Questions NewQuestion() => new()
        {
            Question = "¿Cuál es la capital de México?",
            IdSubject = SubjectId,
            Options =
            [
                new Options { Option = "Ciudad de México", IsAnswer = true },
                new Options { Option = "Guadalajara", IsAnswer = false },
                new Options { Option = "Monterrey", IsAnswer = false },
                new Options { Option = "Puebla", IsAnswer = false }
            ]
        };
    }
}
