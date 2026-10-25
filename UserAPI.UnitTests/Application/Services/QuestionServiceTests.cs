using Moq;
using UserAPI.Application.Common;
using UserAPI.Application.Common.Exceptions;
using UserAPI.Application.DTOs;
using UserAPI.Application.Interfaces.Repositories;
using UserAPI.Application.Services;
using UserAPI.Domain.Entities;

namespace UserAPI.UnitTests.Application.Services
{
    public class QuestionServiceTests
    {
        private readonly Mock<IQuestionRepository> _repository = new();
        private readonly QuestionService _service;

        public QuestionServiceTests() => _service = new QuestionService(_repository.Object);

        [Fact]
        public async Task CreateQuestionWithOptionsAsync_WhenQuestionIsValid_SavesItAndReturnsNewId()
        {
            var question = NewQuestion(optionsCount: 4);
            var newId = Guid.NewGuid();
            _repository.Setup(r => r.CreateQuestionWithOptionsAsync(question)).ReturnsAsync(newId);

            var id = await _service.CreateQuestionWithOptionsAsync(question);

            Assert.Equal(newId, id);
            _repository.Verify(r => r.CreateQuestionWithOptionsAsync(question), Times.Once);
        }

        [Fact]
        public async Task CreateQuestionWithOptionsAsync_WhenQuestionIsNull_ThrowsWithoutSaving()
        {
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _service.CreateQuestionWithOptionsAsync(null!));

            Assert.Equal(ErrorMessages.Questions.Required, exception.Message);
            _repository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CreateQuestionWithOptionsAsync_WhenOptionsAreNull_ThrowsWithoutSaving()
        {
            var question = NewQuestion(optionsCount: 4);
            question.Options = null;

            var exception = await Assert.ThrowsAsync<ValidationException>(() => _service.CreateQuestionWithOptionsAsync(question));

            Assert.Equal("La pregunta debe contar con 4 respuestas", exception.Message);
            _repository.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(3)]
        [InlineData(5)]
        public async Task CreateQuestionWithOptionsAsync_WhenOptionsAreNotFour_ThrowsWithoutSaving(int optionsCount)
        {
            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => _service.CreateQuestionWithOptionsAsync(NewQuestion(optionsCount)));

            Assert.Equal("La pregunta debe contar con 4 respuestas", exception.Message);
            Assert.Equal(ErrorType.Validation, exception.Type);
            _repository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CreateQuestionWithOptionsAsync_WhenSubjectIsMissing_ThrowsWithoutSaving()
        {
            var question = NewQuestion(optionsCount: 4);
            question.IdSubject = null;

            var exception = await Assert.ThrowsAsync<ValidationException>(() => _service.CreateQuestionWithOptionsAsync(question));

            Assert.Equal("La pregunta debe pertenecer a una materia", exception.Message);
            _repository.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(5)]
        [InlineData(8)]
        public async Task UpdateQuestionAndOptionsAsync_WhenMoreThanFourOptions_ThrowsWithoutSaving(int optionsCount)
        {
            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => _service.UpdateQuestionAndOptionsAsync(NewQuestion(optionsCount)));

            Assert.Equal("La pregunta no puede contar con más de 4 respuestas", exception.Message);
            _repository.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        [InlineData(4)]
        public async Task UpdateQuestionAndOptionsAsync_WithFourOrFewerOptions_SavesChanges(int optionsCount)
        {
            var question = NewQuestion(optionsCount);
            _repository.Setup(r => r.UpdateQuestionAndOptionsAsync(question)).ReturnsAsync(question.Id);

            var id = await _service.UpdateQuestionAndOptionsAsync(question);

            Assert.Equal(question.Id, id);
        }

        [Fact]
        public async Task UpdateQuestionAndOptionsAsync_WhenOnlyQuestionTextChanges_SavesChanges()
        {
            var question = NewQuestion(optionsCount: 0);
            question.Options = null;

            await _service.UpdateQuestionAndOptionsAsync(question);

            _repository.Verify(r => r.UpdateQuestionAndOptionsAsync(question), Times.Once);
        }

        [Fact]
        public async Task UpdateQuestionAndOptionsAsync_WhenQuestionIsNull_ThrowsWithoutSaving()
        {
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _service.UpdateQuestionAndOptionsAsync(null!));

            Assert.Equal(ErrorMessages.Questions.Required, exception.Message);
            _repository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CreateQuestionWithOptionsAsync_WhenDatabaseRejectsIt_PropagatesError()
        {
            var postgresError = new BusinessRuleException(DatabaseErrorCodes.RaiseException, "Error de prueba");
            _repository.Setup(r => r.CreateQuestionWithOptionsAsync(It.IsAny<Questions>())).ThrowsAsync(postgresError);

            var exception = await Assert.ThrowsAsync<BusinessRuleException>(
                () => _service.CreateQuestionWithOptionsAsync(NewQuestion(optionsCount: 4)));

            Assert.Same(postgresError, exception);
        }

        [Fact]
        public async Task GetRandomQuestionsAsync_ReturnsQuestionsFromRepository()
        {
            var set = new SetQuestions { TotalQuestions = 10 };
            var questions = new List<Questions> { NewQuestion(optionsCount: 4) };
            _repository.Setup(r => r.GetRandomQuestionsAsync(set)).ReturnsAsync(questions);

            var result = await _service.GetRandomQuestionsAsync(set);

            Assert.Same(questions, result);
        }

        [Fact]
        public async Task GetQuestionWithAnswersPageAsync_ReturnsPageFromRepository()
        {
            var filters = new QuestionPaginationFilters { Page = 1, PageSize = 10 };
            var questions = new List<Questions> { NewQuestion(optionsCount: 4) };
            _repository.Setup(r => r.GetQuestionWithAnswersPageAsync(filters)).ReturnsAsync(questions);

            var result = await _service.GetQuestionWithAnswersPageAsync(filters);

            Assert.Same(questions, result);
        }

        [Fact]
        public async Task GetQuestionWithAnswersByQuestionIdAsync_ReturnsQuestionFromRepository()
        {
            var question = NewQuestion(optionsCount: 4);
            _repository.Setup(r => r.GetQuestionWithAnswersByQuestionIdAsync(question.Id!.Value)).ReturnsAsync(question);

            var result = await _service.GetQuestionWithAnswersByQuestionIdAsync(question.Id!.Value);

            Assert.Same(question, result);
        }

        [Fact]
        public async Task DeleteQuestionAsync_ReturnsDeletedIdFromRepository()
        {
            var id = Guid.NewGuid();
            _repository.Setup(r => r.DeleteQuestionAsync(id)).ReturnsAsync(id);

            var result = await _service.DeleteQuestionAsync(id);

            Assert.Equal(id, result);
        }

        private static Questions NewQuestion(int optionsCount) => new()
        {
            Id = Guid.NewGuid(),
            Question = "¿Cuál es la capital de México?",
            IdSubject = Guid.NewGuid(),
            Options = Enumerable.Range(1, optionsCount)
                .Select(i => new Options { Option = $"Opción {i}", IsAnswer = i == 1 })
                .ToList()
        };
    }
}
