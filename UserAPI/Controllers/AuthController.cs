using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserAPI.Application.DTOs;
using UserAPI.Application.Interfaces.Services;
using UserAPI.Domain.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST api/<AuthController>
        [HttpPost("LogIn")]
        public async Task<IActionResult> Login(LoginRequest login)
        {
            var user = await _authService.SignInAsync(
                login.UserName!,
                login.Password!);

            var token = _authService.GenerateToken(user);

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
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // O también: User.FindFirst("role")?.Value

            return Ok(new
            {
                Id = idClaim,
                UserName = User.Identity?.Name,
                Role = roleClaim // o "Sin rol" si es null
            });
        }

        [HttpPost("Create/User")]
        public async Task<IActionResult> CreateUserAsync(User user)
        {
            var result = await _authService.CreateUserAsync(user);
            return Ok(result);
        }

        [HttpPut("Update/User")]
        public async Task<IActionResult> UpdateUserAsync(User user)
        {
            var result = await _authService.UpdateUserAsync(user);
            return Ok(result);
        }

        [HttpGet("Get/Users")]
        public async Task<IActionResult> GetUsersAsync()
        {
            var result = await _authService.GetUsersAsync();
            return Ok(result);
        }

        [HttpGet("Get/User/{Id}")]
        public async Task<IActionResult> GetUserByIdAsync(Guid Id)
        {
            var result = await _authService.GetUserByIdAsync(Id);
            return Ok(result);
        }

        [HttpPut("Delete/User/{Id}")]
        public async Task<IActionResult> DeleteUserAsync(Guid Id)
        {
            var result = await _authService.DeleteUserAsync(Id);
            return Ok(result);
        }
    }
}
