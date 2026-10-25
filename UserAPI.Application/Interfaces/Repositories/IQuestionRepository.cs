using UserAPI.Application.DTOs;
using UserAPI.Domain.Entities;

namespace UserAPI.Application.Interfaces.Repositories
{
    public interface IQuestionRepository
    {
        Task<Guid?> CreateQuestionWithOptionsAsync(Questions question);
        Task<Guid?> UpdateQuestionAndOptionsAsync(Questions question);
        Task<Guid?> DeleteQuestionAsync(Guid Id);
        Task<List<Questions>> GetQuestionWithAnswersPageAsync(QuestionPaginationFilters filters);
        Task<Questions> GetQuestionWithAnswersByQuestionIdAsync(Guid Id);
        Task<List<Questions>> GetRandomQuestionsAsync(SetQuestions set);
    }
}
