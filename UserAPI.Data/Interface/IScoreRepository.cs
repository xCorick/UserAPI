using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAPI.Core.Models;
using UserAPI.Core.Models.Pagination;

namespace UserAPI.Core.Interface
{
    public interface IScoreRepository
    {
        Task<Scores> CreateScoreAsync(Scores scores);
        Task<Progress> GetGeneralProgressAsync(Guid Id);
        Task<List<User>> GetLastestScoresPageAsync(ScorePaginationFilters filters);
    }
}
