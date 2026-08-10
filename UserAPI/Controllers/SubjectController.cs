using Microsoft.AspNetCore.Mvc;
using UserAPI.Core.Interface;
using UserAPI.Core.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectRepository _subjectRepository;

        public SubjectController(ISubjectRepository subjectRepository) => _subjectRepository = subjectRepository;
        // GET: api/<SubjectController>
        [HttpGet("Get/SubjectsForOptions")]
        public async Task<IActionResult> GetSubjectsForOptionsAsync()
        {
            var result = await _subjectRepository.GetSubjectsForOptionsAsync();
            return Ok(result);
        }

        [HttpGet("Get/GetSubjectWithQuestionsCount")]
        public async Task<IActionResult> GetSubjectWithQuestionsCountAsync()
        {
            var result = await _subjectRepository.GetSubjectWithQuestionsCountAsync();
            return Ok(result);
        }

        // GET api/<SubjectController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/<SubjectController>
        [HttpPost("Create/Subject")]
        public async Task<IActionResult> CreateSubjectAsync([FromBody] Subjects subject)
        {
            var result = await _subjectRepository.CreateSubjectAsync(subject);
            return Ok(result);
        }

        // PUT api/<SubjectController>/5
        [HttpPut("Update/Subject")]
        public async Task<IActionResult> UpdateSubjectAsync([FromBody] Subjects subject)
        {
            var result = await _subjectRepository.UpdateSubjectAsync(subject);
            return Ok(result);
        }

        [HttpPut("Delete/Subject")]
        public async Task<IActionResult> DeleteSubjectAsync([FromBody] Guid Id)
        {
            var result = await _subjectRepository.DeleteSubjectAsync(Id);
            return Ok(result);
        }

        // DELETE api/<SubjectController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
