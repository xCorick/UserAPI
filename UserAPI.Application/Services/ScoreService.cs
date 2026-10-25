using UserAPI.Application.Common;
using UserAPI.Application.Common.Exceptions;
using UserAPI.Application.DTOs;
using UserAPI.Application.Interfaces.Repositories;
using UserAPI.Application.Interfaces.Services;
using UserAPI.Domain.Entities;

namespace UserAPI.Application.Services
{
    public class ScoreService : IScoreService
    {
        private readonly IScoreRepository _scoreRepository;

        public ScoreService(IScoreRepository scoreRepository) => _scoreRepository = scoreRepository;

        // Mismas reglas que encriptacion.insert_score, validadas antes de ir a la base de datos
        public async Task<Scores> CreateScoreAsync(Scores scores)
        {
            if (!scores.IsMixed && scores.IdSubject is null)
                throw new BusinessRuleException(DatabaseErrorCodes.BusinessRule, ErrorMessages.Scores.NotMixedWithoutSubject);
            if (scores.IsMixed && scores.IdSubject is not null)
                throw new BusinessRuleException(DatabaseErrorCodes.BusinessRule, ErrorMessages.Scores.MixedWithSubject);

            return await _scoreRepository.CreateScoreAsync(scores);
        }

        public Task<Progress> GetGeneralProgressAsync(Guid Id) => _scoreRepository.GetGeneralProgressAsync(Id);

        public Task<List<User>> GetLastestScoresPageAsync(ScorePaginationFilters filters) =>
            _scoreRepository.GetLastestScoresPageAsync(filters);

        public Task<UserGlobalProgress> GetUserGlobalProgressAsync(Guid Id) =>
            _scoreRepository.GetUserGlobalProgressAsync(Id);
    }
}
