using UserAPI.Application.Common;
using UserAPI.Application.Common.Exceptions;
using UserAPI.Application.DTOs;
using UserAPI.Application.Interfaces.Repositories;
using UserAPI.Application.Interfaces.Services;
using UserAPI.Domain.Entities;

namespace UserAPI.Application.Services
{
    public class QuestionService : IQuestionService
    {
        private const int RequiredOptions = 4;

        private readonly IQuestionRepository _questionRepository;

        public QuestionService(IQuestionRepository questionRepository) => _questionRepository = questionRepository;

        public async Task<Guid?> CreateQuestionWithOptionsAsync(Questions question)
        {
            if (question is null)
                throw new ValidationException(ErrorMessages.Questions.Required);
            if (question.Options is null || question.Options.Count != RequiredOptions)
                throw new ValidationException(ErrorMessages.Questions.MustHaveFourOptions);
            if (question.IdSubject is null)
                throw new ValidationException(ErrorMessages.Questions.MustHaveSubject);

            return await _questionRepository.CreateQuestionWithOptionsAsync(question);
        }

        public async Task<Guid?> UpdateQuestionAndOptionsAsync(Questions question)
        {
            if (question is null)
                throw new ValidationException(ErrorMessages.Questions.Required);
            if (question.Options?.Count > RequiredOptions)
                throw new ValidationException(ErrorMessages.Questions.CannotHaveMoreThanFourOptions);

            return await _questionRepository.UpdateQuestionAndOptionsAsync(question);
        }

        public Task<Guid?> DeleteQuestionAsync(Guid Id) => _questionRepository.DeleteQuestionAsync(Id);

        public Task<Questions> GetQuestionWithAnswersByQuestionIdAsync(Guid Id) =>
            _questionRepository.GetQuestionWithAnswersByQuestionIdAsync(Id);

        public Task<List<Questions>> GetQuestionWithAnswersPageAsync(QuestionPaginationFilters filters) =>
            _questionRepository.GetQuestionWithAnswersPageAsync(filters);

        public Task<List<Questions>> GetRandomQuestionsAsync(SetQuestions set) =>
            _questionRepository.GetRandomQuestionsAsync(set);
    }
}
