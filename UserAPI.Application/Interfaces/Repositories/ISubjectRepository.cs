using UserAPI.Domain.Entities;

namespace UserAPI.Application.Interfaces.Repositories
{
    public interface ISubjectRepository
    {
        Task<Subjects> CreateSubjectAsync(Subjects subject);
        Task<Subjects> UpdateSubjectAsync(Subjects subject);
        Task<Subjects> DeleteSubjectAsync(Guid Id);
        Task<List<Subjects>> GetSubjectsForOptionsAsync();
        Task<List<Subjects>> GetSubjectWithQuestionsCountAsync();
    }
}
