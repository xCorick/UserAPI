using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserAPI.Core.Models;
using UserAPI.Data.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }
        //// GET: api/<AuthController>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/<AuthController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/<AuthController>
        [HttpPost("LogIn")]
        public async Task<IActionResult> Login(LoginRequest login)
        {
            var user = await _authRepository.SignInAsync(
                login.UserName!,
                login.Password!);

            var token = _authRepository.GenerateToken(user);

            return Ok(new
            {
                token
            });
        }

        [Authorize]
        [HttpGet("GetSession")]
        public IActionResult GetSession()
        {
            // Obtener el rol desde los claims
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            // O también: User.FindFirst("role")?.Value

            return Ok(new
            {
                UserName = User.Identity?.Name,
                Role = roleClaim // o "Sin rol" si es null
            });
        }

        [HttpPost("Create/User")]
        public async Task<IActionResult> CreateUserAsync(User user)
        {
            var result = await _authRepository.CreateUserAsync(user);
            return Ok(result);
        }

        [HttpPut("Update/User")]
        public async Task<IActionResult> UpdateUserAsync(User user)
        {
            var result = await _authRepository.UpdateUserAsync(user);
            return Ok(result);
        }

        [HttpGet("Get/Users")]
        public async Task<IActionResult> GetUsersAsync()
        {
            var result = await _authRepository.GetUsersAsync();
            return Ok(result);
        }

        [HttpGet("Get/User/{Id}")]
        public async Task<IActionResult> GetUserByIdAsync(Guid Id)
        {
            var result = await _authRepository.GetUserByIdAsync(Id);
            return Ok(result);
        }

        [HttpPut("Delete/User/{Id}")]
        public async Task<IActionResult> DeleteUserAsync(Guid Id)
        {
            var result = await _authRepository.DeleteUserAsync(Id);
            return Ok(result);
        }
        //// PUT api/<AuthController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<AuthController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
