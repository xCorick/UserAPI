using Moq;
using UserAPI.Application.Common;
using UserAPI.Application.Common.Exceptions;
using UserAPI.Application.Interfaces.Repositories;
using UserAPI.Application.Services;
using UserAPI.Domain.Entities;

namespace UserAPI.UnitTests.Application.Services
{
    public class SubjectServiceTests
    {
        private readonly Mock<ISubjectRepository> _repository = new();
        private readonly SubjectService _service;

        public SubjectServiceTests() => _service = new SubjectService(_repository.Object);

        [Fact]
        public async Task CreateSubjectAsync_ReturnsSubjectCreatedByRepository()
        {
            var subject = new Subjects { Name = "Matemáticas", Description = "Álgebra" };
            var created = new Subjects { Id = Guid.NewGuid(), Name = "Matemáticas", Description = "Álgebra", IsActive = true };
            _repository.Setup(r => r.CreateSubjectAsync(subject)).ReturnsAsync(created);

            var result = await _service.CreateSubjectAsync(subject);

            Assert.Same(created, result);
        }

        [Fact]
        public async Task CreateSubjectAsync_WhenNameAlreadyExists_PropagatesPostgresError()
        {
            var postgresError = new BusinessRuleException(DatabaseErrorCodes.BusinessRule, "La materia con ese nombre ya existe");
            _repository.Setup(r => r.CreateSubjectAsync(It.IsAny<Subjects>())).ThrowsAsync(postgresError);

            var exception = await Assert.ThrowsAsync<BusinessRuleException>(() => _service.CreateSubjectAsync(new Subjects()));

            Assert.Equal("P0409: La materia con ese nombre ya existe", exception.Message);
        }

        [Fact]
        public async Task UpdateSubjectAsync_ReturnsSubjectUpdatedByRepository()
        {
            var subject = new Subjects { Id = Guid.NewGuid(), Name = "Historia" };
            _repository.Setup(r => r.UpdateSubjectAsync(subject)).ReturnsAsync(subject);

            var result = await _service.UpdateSubjectAsync(subject);

            Assert.Same(subject, result);
        }

        [Fact]
        public async Task DeleteSubjectAsync_ReturnsSubjectDeletedByRepository()
        {
            var id = Guid.NewGuid();
            var deleted = new Subjects { Id = id, IsActive = false };
            _repository.Setup(r => r.DeleteSubjectAsync(id)).ReturnsAsync(deleted);

            var result = await _service.DeleteSubjectAsync(id);

            Assert.Same(deleted, result);
        }

        [Fact]
        public async Task GetSubjectWithQuestionsCountAsync_ReturnsSubjectsFromRepository()
        {
            var subjects = new List<Subjects> { new() { Name = "Matemáticas", TotalQuestions = 12 } };
            _repository.Setup(r => r.GetSubjectWithQuestionsCountAsync()).ReturnsAsync(subjects);

            var result = await _service.GetSubjectWithQuestionsCountAsync();

            Assert.Same(subjects, result);
        }
    }
}
