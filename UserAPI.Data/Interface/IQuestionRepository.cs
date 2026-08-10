using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAPI.Core.Models;
using UserAPI.Core.Models.Pagination;
using UserAPI.Core.Models.QuestionsDTOs;

namespace UserAPI.Core.Interface
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
