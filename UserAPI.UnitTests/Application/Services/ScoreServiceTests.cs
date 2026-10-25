using Moq;
using UserAPI.Application.Common;
using UserAPI.Application.Common.Exceptions;
using UserAPI.Application.DTOs;
using UserAPI.Application.Interfaces.Repositories;
using UserAPI.Application.Services;
using UserAPI.Domain.Entities;

namespace UserAPI.UnitTests.Application.Services
{
    public class ScoreServiceTests
    {
        private readonly Mock<IScoreRepository> _repository = new();
        private readonly ScoreService _service;

        public ScoreServiceTests() => _service = new ScoreService(_repository.Object);

        [Fact]
        public async Task CreateScoreAsync_WhenGameIsNotMixedAndHasNoSubject_ThrowsSameErrorAsPostgres()
        {
            var score = new Scores { IsMixed = false, IdSubject = null, IdUser = Guid.NewGuid() };

            var exception = await Assert.ThrowsAsync<BusinessRuleException>(() => _service.CreateScoreAsync(score));

            // Mismo texto y código que RAISE EXCEPTION ... USING ERRCODE = 'P0409' en encriptacion.insert_score
            Assert.Equal("P0409: No puede guardarse el score si no hay materia asignada y el modo de juego no es mixed", exception.Message);
            Assert.Equal(DatabaseErrorCodes.BusinessRule, exception.Code);
            _repository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CreateScoreAsync_WhenMixedGameHasSubject_ThrowsSameErrorAsPostgres()
        {
            var score = new Scores { IsMixed = true, IdSubject = Guid.NewGuid(), IdUser = Guid.NewGuid() };

            var exception = await Assert.ThrowsAsync<BusinessRuleException>(() => _service.CreateScoreAsync(score));

            Assert.Equal("P0409: No puede guardarse el score del modo mixed con una materia asignada", exception.Message);
            Assert.Equal(DatabaseErrorCodes.BusinessRule, exception.Code);
            _repository.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public async Task CreateScoreAsync_WhenModeAndSubjectAreConsistent_SavesScore(bool isMixed, bool hasSubject)
        {
            var score = new Scores
            {
                IsMixed = isMixed,
                IdSubject = hasSubject ? Guid.NewGuid() : null,
                IdUser = Guid.NewGuid(),
                Score = 9,
                Points = 90
            };
            var saved = new Scores { Id = Guid.NewGuid(), Score = 9, Points = 90 };
            _repository.Setup(r => r.CreateScoreAsync(score)).ReturnsAsync(saved);

            var result = await _service.CreateScoreAsync(score);

            Assert.Same(saved, result);
        }

        [Fact]
        public async Task GetGeneralProgressAsync_ReturnsProgressFromRepository()
        {
            var userId = Guid.NewGuid();
            var progress = new Progress { Score = 8.5m, Points = 170, Games = 2 };
            _repository.Setup(r => r.GetGeneralProgressAsync(userId)).ReturnsAsync(progress);

            var result = await _service.GetGeneralProgressAsync(userId);

            Assert.Same(progress, result);
        }

        [Fact]
        public async Task GetLastestScoresPageAsync_ReturnsScoresFromRepository()
        {
            var filters = new ScorePaginationFilters { Page = 1, PageSize = 5, IdUser = Guid.NewGuid() };
            var users = new List<User> { new() { Id = filters.IdUser } };
            _repository.Setup(r => r.GetLastestScoresPageAsync(filters)).ReturnsAsync(users);

            var result = await _service.GetLastestScoresPageAsync(filters);

            Assert.Same(users, result);
        }

        [Fact]
        public async Task GetUserGlobalProgressAsync_ReturnsProgressFromRepository()
        {
            var userId = Guid.NewGuid();
            var progress = new UserGlobalProgress { GeneralScore = 9, Streak = 2 };
            _repository.Setup(r => r.GetUserGlobalProgressAsync(userId)).ReturnsAsync(progress);

            var result = await _service.GetUserGlobalProgressAsync(userId);

            Assert.Same(progress, result);
        }
    }
}
