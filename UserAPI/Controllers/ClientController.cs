using Microsoft.AspNetCore.Mvc;
using UserAPI.Data.DTOs;
using UserAPI.Data.Interface;
using UserAPI.Data.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;
        public ClientController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        // GET: api/<ClientController>
        [HttpGet("Get/Clientes")]
        public async Task<IActionResult> GetClientesAsync()
        {
            var result = await _clientRepository.GetClientesAsync();
            return Ok(result);
        }

        // GET api/<ClientController>/5
        [HttpGet("Get/Cliente{clave}")]
        public async Task<IActionResult> GetClienteAsync(string clave)
        {
            var result = await _clientRepository.GetClienteAsync(clave);
            Console.WriteLine(result);
            return Ok(result);
        }

        [HttpGet("Get/Clientes/Page")]
        public async Task<IActionResult> GetClientesPageAsync([FromQuery] PaginationFilter pagination)
        {
            var result = await _clientRepository.GetClientesPageAsync(pagination);
            return Ok(result);
        }

        // POST api/<ClientController>
        [HttpPost("Create/Cliente")]
        public async Task<IActionResult> CreateAsync([FromBody] ClienteDTO cliente)
        {
            var result = await _clientRepository.CreateAsync(cliente);
            return Ok(result);
        }

        // PUT api/<ClientController>/5
        [HttpPut("Update/Cliente")]
        public async Task<IActionResult> UpdateAsync([FromBody] ClienteDTO cliente)
        {
            var result = await _clientRepository.UpdateAsync(cliente);
            return Ok(result);
        }

        // DELETE api/<ClientController>/5
        [HttpDelete("{clave}")]
        public async Task<IActionResult> DeleteAsync(string clave)
        {
            var result = await _clientRepository.DeleteAsync(clave);
            return Ok(result);
        }
    }
}
