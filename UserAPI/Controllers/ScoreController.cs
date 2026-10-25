using Microsoft.AspNetCore.Mvc;
using UserAPI.Application.DTOs;
using UserAPI.Application.Interfaces.Services;
using UserAPI.Domain.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScoreController : ControllerBase
    {
        private readonly IScoreService _scoreService;

        public ScoreController(IScoreService scoreService) => _scoreService = scoreService;

        [HttpGet("Get/GeneralProgress/{Id}")]
        public async Task<IActionResult> GetGeneralProgressAsync(Guid Id)
        {
            var result = await _scoreService.GetGeneralProgressAsync(Id);
            return Ok(result);
        }

        [HttpGet("Get/LastestScores/Page")]
        public async Task<IActionResult> GetLastestScoresPageAsync([FromQuery] ScorePaginationFilters filters)
        {
            var result = await _scoreService.GetLastestScoresPageAsync(filters);
            return Ok(result);
        }

        [HttpGet("Get/UserGlobalProgress/{Id}")]
        public async Task<IActionResult> GetUserGlobalProgressAsync(Guid Id)
        {
            var result = await _scoreService.GetUserGlobalProgressAsync(Id);
            return Ok(result);
        }

        [HttpPost("Create/Score")]
        public async Task<IActionResult> CreateScoreAsync([FromBody] Scores score)
        {
            var result = await _scoreService.CreateScoreAsync(score);
            return Ok(result);
        }
    }
}
