using UserAPI.Application.Interfaces.Repositories;
using UserAPI.Application.Interfaces.Services;
using UserAPI.Domain.Entities;

namespace UserAPI.Application.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _subjectRepository;

        public SubjectService(ISubjectRepository subjectRepository) => _subjectRepository = subjectRepository;

        public Task<Subjects> CreateSubjectAsync(Subjects subject) => _subjectRepository.CreateSubjectAsync(subject);

        public Task<Subjects> UpdateSubjectAsync(Subjects subject) => _subjectRepository.UpdateSubjectAsync(subject);

        public Task<Subjects> DeleteSubjectAsync(Guid Id) => _subjectRepository.DeleteSubjectAsync(Id);

        public Task<List<Subjects>> GetSubjectsForOptionsAsync() => _subjectRepository.GetSubjectsForOptionsAsync();

        public Task<List<Subjects>> GetSubjectWithQuestionsCountAsync() =>
            _subjectRepository.GetSubjectWithQuestionsCountAsync();
    }
}
