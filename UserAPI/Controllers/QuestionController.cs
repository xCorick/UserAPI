using Microsoft.AspNetCore.Mvc;
using UserAPI.Core.Interface;
using UserAPI.Core.Models;
using UserAPI.Core.Models.Pagination;
using UserAPI.Core.Models.QuestionsDTOs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionRepository _questionRepository;
        public QuestionController(IQuestionRepository questionRepository) => _questionRepository = questionRepository;

        [HttpGet("Get/QuestionWithAnswers/Page")]
        public async Task<IActionResult> GetQuestionWithAnswersPageAsync([FromQuery] QuestionPaginationFilters filters)
        {
            var result = await _questionRepository.GetQuestionWithAnswersPageAsync(filters);
            return Ok(result);
        }

        [HttpGet("Get/QuestionWithAnswers/{Id}")]
        public async Task<IActionResult> GetQuestionWithAnswersByQuestionIdAsync(Guid Id)
        {
            var result = await _questionRepository.GetQuestionWithAnswersByQuestionIdAsync(Id);
            return Ok(result);
        }

        [HttpGet("Get/RandomQuestions")]
        public async Task<IActionResult> GetRandomQuestionsAsync([FromQuery] SetQuestions set)
        {
            var result = await _questionRepository.GetRandomQuestionsAsync(set);
            return Ok(result);
        }

        [HttpPost("Create/QuestionWithOptions")]
        public async Task<IActionResult> CreateQuestionWithOptionsAsync([FromBody] Questions question)
        {
            var result = await _questionRepository.CreateQuestionWithOptionsAsync(question);
            return Ok(result);
        }

        [HttpPut("Update/QuestionAndOptions")]
        public async Task<IActionResult> UpdateQuestionAndOptionsAsync([FromBody] Questions question)
        {
            var result = await _questionRepository.UpdateQuestionAndOptionsAsync(question);
            return Ok(result);
        }

        [HttpPut("Delete/Question")]
        public async Task<IActionResult> DeleteQuestionAsync(Guid Id)
        {
            var result = await _questionRepository.DeleteQuestionAsync(Id);
            return Ok(result);
        }
    }
}
