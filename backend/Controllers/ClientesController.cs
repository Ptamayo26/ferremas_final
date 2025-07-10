using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ferremas.Api.DTOs;
using Ferremas.Api.Services.Interfaces;
using Ferremas.Api.Constants;
using System.Linq;

namespace Ferremas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClientesController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet]
        [Authorize(Roles = "administrador,vendedor")]
        public async Task<ActionResult<IEnumerable<ClienteResponseDTO>>> GetAll()
        {
            try
            {
                var clientes = await _clienteService.GetAll();
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "administrador,vendedor")]
        public async Task<ActionResult<ClienteResponseDTO>> GetById(int id)
        {
            try
            {
                var cliente = await _clienteService.GetById(id);
                if (cliente == null)
                    return NotFound($"Cliente con ID {id} no encontrado");
                return Ok(cliente);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "administrador,vendedor")]
        public async Task<ActionResult<ClienteResponseDTO>> Create([FromBody] ClienteCreateDTO dto)
        {
            try
            {
                var cliente = await _clienteService.Create(dto);
                return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "administrador,vendedor")]
        public async Task<ActionResult<ClienteResponseDTO>> Update(int id, [FromBody] ClienteUpdateDTO dto)
        {
            try
            {
                var cliente = await _clienteService.Update(id, dto);
                if (cliente == null)
                    return NotFound($"Cliente con ID {id} no encontrado");
                return Ok(cliente);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var eliminado = await _clienteService.Delete(id);
                if (!eliminado)
                    return NotFound($"Cliente con ID {id} no encontrado");
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("mi-perfil")]
        public async Task<ActionResult<ClienteResponseDTO>> GetMiPerfil()
        {
            try
            {
                // Obtener el ID de usuario autenticado de forma robusta
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int usuarioId))
                    return Unauthorized("No se pudo obtener el usuario autenticado");

                // Buscar el cliente asociado a este usuarioId
                var clientes = await _clienteService.GetAll();
                var cliente = clientes.FirstOrDefault(c => c.UsuarioId == usuarioId);
                if (cliente == null)
                    return NotFound("No se encontró un cliente asociado a este usuario");
                return Ok(cliente);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}