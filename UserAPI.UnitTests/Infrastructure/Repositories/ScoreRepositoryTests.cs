using System.Data;
using UserAPI.Application.Common;
using UserAPI.Application.Common.Exceptions;
using UserAPI.Application.DTOs;
using UserAPI.Domain.Entities;
using UserAPI.Infrastructure.Repositories;
using UserAPI.UnitTests.Helpers;

namespace UserAPI.UnitTests.Infrastructure.Repositories
{
    public class ScoreRepositoryTests
    {
        private static readonly Guid ScoreId = Guid.Parse("3c2b1a09-8f7e-4d6c-9b5a-4f3e2d1c0b9a");
        private static readonly Guid UserId = Guid.Parse("6d5c4b3a-2f1e-4a0b-8c9d-7e6f5a4b3c2d");
        private static readonly Guid SubjectId = Guid.Parse("1f2e3d4c-5b6a-4978-8a9b-0c1d2e3f4a5b");

        private readonly FakeSqlExecutor _executor = new();
        private readonly ScoreRepository _repository;

        public ScoreRepositoryTests() => _repository = new ScoreRepository(_executor);

        public static TheoryData<string> Operations => new()
        {
            nameof(ScoreRepository.CreateScoreAsync),
            nameof(ScoreRepository.GetGeneralProgressAsync),
            nameof(ScoreRepository.GetLastestScoresPageAsync),
            nameof(ScoreRepository.GetUserGlobalProgressAsync)
        };

        public static TheoryData<string, string> OperationErrorMessages => new()
        {
            { nameof(ScoreRepository.CreateScoreAsync), ErrorMessages.Scores.Create },
            { nameof(ScoreRepository.GetGeneralProgressAsync), ErrorMessages.Scores.GetGeneralProgress },
            { nameof(ScoreRepository.GetLastestScoresPageAsync), ErrorMessages.Scores.GetLatestScoresPage },
            { nameof(ScoreRepository.GetUserGlobalProgressAsync), ErrorMessages.Scores.GetUserGlobalProgress }
        };

        [Fact]
        public async Task CreateScoreAsync_SendsScoreToInsertFunction()
        {
            await _repository.CreateScoreAsync(NewScore());

            var statement = _executor.ExecutedStatement;
            SqlAssert.CallsFunction(statement, "insert_score");
            Assert.Equal(8.5m, SqlAssert.Parameter(statement, "@Score"));
            Assert.Equal(UserId, SqlAssert.Parameter(statement, "@IdUser"));
            Assert.Equal(false, SqlAssert.Parameter(statement, "@IsMixed"));
            Assert.Equal(85, SqlAssert.Parameter(statement, "@Points"));
            Assert.Equal(SubjectId, SqlAssert.Parameter(statement, "@IdSubject"));
        }

        [Fact]
        public async Task CreateScoreAsync_ForMixedGame_SendsDbNullSubject()
        {
            var score = NewScore();
            score.IsMixed = true;
            score.IdSubject = null;

            await _repository.CreateScoreAsync(score);

            Assert.Equal(DBNull.Value, SqlAssert.Parameter(_executor.ExecutedStatement, "@IdSubject"));
        }

        [Fact]
        public async Task CreateScoreAsync_ReturnsInsertedScore()
        {
            _executor.Rows = ScoreRows((ScoreId, 8.5m, 85, SubjectId, false));

            var score = await _repository.CreateScoreAsync(NewScore());

            Assert.Equal(ScoreId, score.Id);
            Assert.Equal(8.5m, score.Score);
            Assert.Equal(85, score.Points);
            Assert.Equal(SubjectId, score.IdSubject);
            Assert.False(score.IsMixed);
        }

        [Fact]
        public async Task CreateScoreAsync_WhenInsertedScoreHasNoSubject_MapsNullSubject()
        {
            _executor.Rows = ScoreRows((ScoreId, 7m, 70, DBNull.Value, true));

            var score = await _repository.CreateScoreAsync(NewScore());

            Assert.Null(score.IdSubject);
            Assert.True(score.IsMixed);
        }

        [Fact]
        public async Task GetGeneralProgressAsync_SendsUserIdAndMapsAggregates()
        {
            _executor.Rows = Rows.Table(("score", typeof(decimal)), ("points", typeof(int)), ("games", typeof(int)));
            _executor.Rows.Rows.Add(8.25m, 330, 4);

            var progress = await _repository.GetGeneralProgressAsync(UserId);

            var statement = _executor.ExecutedStatement;
            SqlAssert.CallsFunction(statement, "get_general_progress");
            Assert.Equal(UserId, SqlAssert.Parameter(statement, "@Id"));
            Assert.Equal(8.25m, progress.Score);
            Assert.Equal(330, progress.Points);
            Assert.Equal(4, progress.Games);
        }

        [Fact]
        public async Task GetGeneralProgressAsync_WhenNoRowsAreReturned_ReturnsEmptyProgress()
        {
            _executor.Rows = Rows.Table(("score", typeof(decimal)), ("points", typeof(int)), ("games", typeof(int)));

            var progress = await _repository.GetGeneralProgressAsync(UserId);

            Assert.Equal(0, progress.Games);
        }

        [Fact]
        public async Task GetLastestScoresPageAsync_SendsFiltersAndMapsJson()
        {
            _executor.Rows = Rows.Json("result", $$"""
                [
                    {
                        "id": "{{UserId}}",
                        "score": {
                            "id": "{{ScoreId}}",
                            "score": 9.5,
                            "subject": { "id": "{{SubjectId}}", "name": "Matemáticas" }
                        }
                    }
                ]
                """);

            var users = await _repository.GetLastestScoresPageAsync(new ScorePaginationFilters { Page = 1, PageSize = 5, IdUser = UserId });

            var statement = _executor.ExecutedStatement;
            SqlAssert.CallsFunction(statement, "get_latest_scores_page");
            Assert.Equal(1, SqlAssert.Parameter(statement, "@Page"));
            Assert.Equal(5, SqlAssert.Parameter(statement, "@PageSize"));
            Assert.Equal(UserId, SqlAssert.Parameter(statement, "@IdUser"));

            var user = Assert.Single(users);
            Assert.Equal(UserId, user.Id);
            Assert.Equal(ScoreId, user.Score!.Id);
            Assert.Equal(9.5m, user.Score.Score);
            Assert.Equal(SubjectId, user.Score.Subject!.Id);
            Assert.Equal("Matemáticas", user.Score.Subject.Name);
        }

        [Fact]
        public async Task GetLastestScoresPageAsync_WhenUserHasNoScores_ReturnsEmptyList()
        {
            _executor.Rows = Rows.Json("result", "[]");

            var users = await _repository.GetLastestScoresPageAsync(new ScorePaginationFilters { Page = 1, PageSize = 5, IdUser = UserId });

            Assert.Empty(users);
        }

        [Fact]
        public async Task GetUserGlobalProgressAsync_SendsUserIdAndMapsJson()
        {
            _executor.Rows = Rows.Json("result", $$"""
                {
                    "generalScore": 8.75,
                    "subjectsPlayed": 2,
                    "totalPreguntas": 20,
                    "streak": 3,
                    "subjects": [
                        { "id": "{{SubjectId}}", "name": "Matemáticas", "averageScore": 9.00, "totalPoints": 90, "gamesPlayed": 1 }
                    ]
                }
                """);

            var progress = await _repository.GetUserGlobalProgressAsync(UserId);

            var statement = _executor.ExecutedStatement;
            SqlAssert.CallsFunction(statement, "get_user_global_progress");
            Assert.Equal(UserId, SqlAssert.Parameter(statement, "@Id"));
            Assert.Equal(8.75m, progress.GeneralScore);
            Assert.Equal(2, progress.SubjectsPlayed);
            Assert.Equal(20, progress.TotalPreguntas);
            Assert.Equal(3, progress.Streak);
            var subject = Assert.Single(progress.Subjects);
            Assert.Equal(SubjectId, subject.Id);
            Assert.Equal("Matemáticas", subject.Name);
            Assert.Equal(9.00m, subject.AverageScore);
            Assert.Equal(90, subject.TotalPoints);
            Assert.Equal(1, subject.GamesPlayed);
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
            var postgresError = new BusinessRuleException(DatabaseErrorCodes.BusinessRule, ErrorMessages.Scores.MixedWithSubject);
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
            nameof(ScoreRepository.CreateScoreAsync) => _repository.CreateScoreAsync(NewScore()),
            nameof(ScoreRepository.GetGeneralProgressAsync) => _repository.GetGeneralProgressAsync(UserId),
            nameof(ScoreRepository.GetLastestScoresPageAsync) => _repository.GetLastestScoresPageAsync(
                new ScorePaginationFilters { Page = 1, PageSize = 10, IdUser = UserId }),
            nameof(ScoreRepository.GetUserGlobalProgressAsync) => _repository.GetUserGlobalProgressAsync(UserId),
            _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
        };

        private static Scores NewScore() => new()
        {
            Score = 8.5m,
            Points = 85,
            IsMixed = false,
            IdSubject = SubjectId,
            IdUser = UserId
        };

        // Columnas de la tabla encriptacion.scores que lee el repositorio
        private static DataTable ScoreRows(params (Guid Id, decimal Score, int Points, object IdSubject, bool IsMixed)[] rows)
        {
            var table = Rows.Table(
                ("id", typeof(Guid)),
                ("score", typeof(decimal)),
                ("points", typeof(int)),
                ("id_subject", typeof(Guid)),
                ("is_mixed", typeof(bool)));

            foreach (var row in rows)
                table.Rows.Add(row.Id, row.Score, row.Points, row.IdSubject, row.IsMixed);

            return table;
        }
    }
}
