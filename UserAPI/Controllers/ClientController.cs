using Microsoft.AspNetCore.Mvc;
using UserAPI.Application.DTOs;
using UserAPI.Application.Interfaces.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;
        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        // GET: api/<ClientController>
        [HttpGet("Get/Clientes")]
        public async Task<IActionResult> GetClientesAsync()
        {
            var result = await _clientService.GetClientesAsync();
            return Ok(result);
        }

        // GET api/<ClientController>/5
        [HttpGet("Get/Cliente{clave}")]
        public async Task<IActionResult> GetClienteAsync(string clave)
        {
            var result = await _clientService.GetClienteAsync(clave);
            return Ok(result);
        }

        [HttpGet("Get/Clientes/Page")]
        public async Task<IActionResult> GetClientesPageAsync([FromQuery] PaginationFilter pagination)
        {
            var result = await _clientService.GetClientesPageAsync(pagination);
            return Ok(result);
        }

        // POST api/<ClientController>
        [HttpPost("Create/Cliente")]
        public async Task<IActionResult> CreateAsync([FromBody] ClienteDTO cliente)
        {
            var result = await _clientService.CreateAsync(cliente);
            return Ok(result);
        }

        // PUT api/<ClientController>/5
        [HttpPut("Update/Cliente")]
        public async Task<IActionResult> UpdateAsync([FromBody] ClienteDTO cliente)
        {
            var result = await _clientService.UpdateAsync(cliente);
            return Ok(result);
        }

        // DELETE api/<ClientController>/5
        [HttpDelete("{clave}")]
        public async Task<IActionResult> DeleteAsync(string clave)
        {
            var result = await _clientService.DeleteAsync(clave);
            return Ok(result);
        }
    }
}
