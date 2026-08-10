using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAPI.Core.Models;

namespace UserAPI.Core.Interface
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
