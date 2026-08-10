using Microsoft.AspNetCore.Mvc;
using UserAPI.Core.Interface;
using UserAPI.Core.Models;
using UserAPI.Core.Models.Pagination;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScoreController : ControllerBase
    {
        private readonly IScoreRepository _scoreRepository;

        public ScoreController(IScoreRepository scoreRepository) => _scoreRepository = scoreRepository;

        [HttpGet("Get/GeneralProgress/{Id}")]
        public async Task<IActionResult> GetGeneralProgressAsync(Guid Id)
        {
            var result = await _scoreRepository.GetGeneralProgressAsync(Id);
            return Ok(result);
        }

        [HttpGet("Get/LastestScores/Page")]
        public async Task<IActionResult> GetLastestScoresPageAsync([FromQuery] ScorePaginationFilters filters)
        {
            var result = await _scoreRepository.GetLastestScoresPageAsync(filters);
            return Ok(result);
        }

        [HttpPost("Create/Score")]
        public async Task<IActionResult> CreateScoreAsync([FromBody] Scores score)
        {
            var result = await _scoreRepository.CreateScoreAsync(score);
            return Ok(result);
        }
    }
}
