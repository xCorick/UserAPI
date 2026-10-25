using UserAPI.Application.DTOs;
using UserAPI.Domain.Entities;

namespace UserAPI.Application.Interfaces.Repositories
{
    public interface IScoreRepository
    {
        Task<Scores> CreateScoreAsync(Scores scores);
        Task<Progress> GetGeneralProgressAsync(Guid Id);
        Task<List<User>> GetLastestScoresPageAsync(ScorePaginationFilters filters);
        Task<UserGlobalProgress> GetUserGlobalProgressAsync(Guid Id);
    }
}
