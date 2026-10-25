using Microsoft.AspNetCore.Mvc;
using UserAPI.Application.Interfaces.Services;
using UserAPI.Domain.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectController(ISubjectService subjectService) => _subjectService = subjectService;

        [HttpGet("Get/SubjectsForOptions")]
        public async Task<IActionResult> GetSubjectsForOptionsAsync()
        {
            var result = await _subjectService.GetSubjectsForOptionsAsync();
            return Ok(result);
        }

        [HttpGet("Get/GetSubjectWithQuestionsCount")]
        public async Task<IActionResult> GetSubjectWithQuestionsCountAsync()
        {
            var result = await _subjectService.GetSubjectWithQuestionsCountAsync();
            return Ok(result);
        }

        [HttpPost("Create/Subject")]
        public async Task<IActionResult> CreateSubjectAsync([FromBody] Subjects subject)
        {
            var result = await _subjectService.CreateSubjectAsync(subject);
            return Ok(result);
        }

        [HttpPut("Update/Subject")]
        public async Task<IActionResult> UpdateSubjectAsync([FromBody] Subjects subject)
        {
            var result = await _subjectService.UpdateSubjectAsync(subject);
            return Ok(result);
        }

        [HttpPut("Delete/Subject")]
        public async Task<IActionResult> DeleteSubjectAsync([FromBody] Guid Id)
        {
            var result = await _subjectService.DeleteSubjectAsync(Id);
            return Ok(result);
        }

    }
}
