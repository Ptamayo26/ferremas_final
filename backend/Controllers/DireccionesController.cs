using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ferremas.Api.DTOs;
using Ferremas.Api.Services.Interfaces;
using Ferremas.Api.Constants;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Ferremas.Api.Data;
using Ferremas.Api.Models;

namespace Ferremas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DireccionesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DireccionesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("usuario/{usuarioId}")]
        [Authorize(Roles = "administrador,vendedor")]
        public async Task<ActionResult<IEnumerable<DireccionDTO>>> GetByUsuario(int usuarioId)
        {
            try
            {
                var direcciones = await _context.Direcciones
                    .Where(d => d.UsuarioId == usuarioId)
                    .OrderByDescending(d => d.EsPrincipal)
                    .ThenBy(d => d.Calle)
                    .Select(d => new DireccionDTO
                    {
                        Id = d.Id,
                        Calle = d.Calle,
                        Numero = d.Numero,
                        Departamento = d.Departamento,
                        Comuna = d.Comuna,
                        Region = d.Region,
                        CodigoPostal = d.CodigoPostal,
                        EsPrincipal = d.EsPrincipal ?? false,
                        FechaModificacion = d.FechaModificacion
                    })
                    .ToListAsync();

                return Ok(direcciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost("usuario/{usuarioId}")]
        [Authorize(Roles = "administrador,vendedor")]
        public async Task<ActionResult<DireccionDTO>> Create(int usuarioId, [FromBody] DireccionDTO dto)
        {
            try
            {
                // Verificar que el usuario existe
                var usuario = await _context.Usuarios.FindAsync(usuarioId);
                if (usuario == null)
                    return NotFound($"Usuario con ID {usuarioId} no encontrado");

                // Si esta dirección será principal, quitar la principal actual
                if (dto.EsPrincipal)
                {
                    var direccionPrincipalActual = await _context.Direcciones
                        .FirstOrDefaultAsync(d => d.UsuarioId == usuarioId && d.EsPrincipal == true);
                    
                    if (direccionPrincipalActual != null)
                    {
                        direccionPrincipalActual.EsPrincipal = false;
                        direccionPrincipalActual.FechaModificacion = DateTime.UtcNow;
                    }
                }

                var direccion = new Direccion
                {
                    UsuarioId = usuarioId,
                    Calle = dto.Calle,
                    Numero = dto.Numero,
                    Departamento = dto.Departamento,
                    Comuna = dto.Comuna,
                    Region = dto.Region,
                    CodigoPostal = dto.CodigoPostal,
                    EsPrincipal = dto.EsPrincipal,
                    FechaCreacion = DateTime.UtcNow,
                    FechaModificacion = DateTime.UtcNow
                };

                _context.Direcciones.Add(direccion);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetByUsuario), new { usuarioId }, new DireccionDTO
                {
                    Id = direccion.Id,
                    Calle = direccion.Calle,
                    Numero = direccion.Numero,
                    Departamento = direccion.Departamento,
                    Comuna = direccion.Comuna,
                    Region = direccion.Region,
                    CodigoPostal = direccion.CodigoPostal,
                    EsPrincipal = direccion.EsPrincipal ?? false,
                    FechaModificacion = direccion.FechaModificacion
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "administrador,vendedor")]
        public async Task<ActionResult<DireccionDTO>> Update(int id, [FromBody] DireccionDTO dto)
        {
            try
            {
                var direccion = await _context.Direcciones.FindAsync(id);
                if (direccion == null)
                    return NotFound($"Dirección con ID {id} no encontrada");

                // Si esta dirección será principal, quitar la principal actual
                if (dto.EsPrincipal && !(direccion.EsPrincipal == true))
                {
                    var direccionPrincipalActual = await _context.Direcciones
                        .FirstOrDefaultAsync(d => d.UsuarioId == direccion.UsuarioId && d.EsPrincipal == true && d.Id != id);
                    
                    if (direccionPrincipalActual != null)
                    {
                        direccionPrincipalActual.EsPrincipal = false;
                        direccionPrincipalActual.FechaModificacion = DateTime.UtcNow;
                    }
                }

                direccion.Calle = dto.Calle;
                direccion.Numero = dto.Numero;
                direccion.Departamento = dto.Departamento;
                direccion.Comuna = dto.Comuna;
                direccion.Region = dto.Region;
                direccion.CodigoPostal = dto.CodigoPostal;
                direccion.EsPrincipal = dto.EsPrincipal;
                direccion.FechaModificacion = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new DireccionDTO
                {
                    Id = direccion.Id,
                    Calle = direccion.Calle,
                    Numero = direccion.Numero,
                    Departamento = direccion.Departamento,
                    Comuna = direccion.Comuna,
                    Region = direccion.Region,
                    CodigoPostal = direccion.CodigoPostal,
                    EsPrincipal = direccion.EsPrincipal ?? false,
                    FechaModificacion = direccion.FechaModificacion
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "administrador,vendedor")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var direccion = await _context.Direcciones.FindAsync(id);
                if (direccion == null)
                    return NotFound($"Dirección con ID {id} no encontrada");

                // Si es la dirección principal, no permitir eliminarla si es la única
                if (direccion.EsPrincipal == true)
                {
                    var totalDirecciones = await _context.Direcciones
                        .CountAsync(d => d.UsuarioId == direccion.UsuarioId);
                    
                    if (totalDirecciones <= 1)
                        return BadRequest("No se puede eliminar la única dirección del usuario");
                }

                _context.Direcciones.Remove(direccion);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPut("{id}/principal")]
        [Authorize(Roles = "administrador,vendedor")]
        public async Task<ActionResult<DireccionDTO>> SetPrincipal(int id)
        {
            try
            {
                var direccion = await _context.Direcciones.FindAsync(id);
                if (direccion == null)
                    return NotFound($"Dirección con ID {id} no encontrada");

                // Quitar la dirección principal actual
                var direccionPrincipalActual = await _context.Direcciones
                    .FirstOrDefaultAsync(d => d.UsuarioId == direccion.UsuarioId && d.EsPrincipal == true);
                
                if (direccionPrincipalActual != null)
                {
                    direccionPrincipalActual.EsPrincipal = false;
                    direccionPrincipalActual.FechaModificacion = DateTime.UtcNow;
                }

                // Marcar esta dirección como principal
                direccion.EsPrincipal = true;
                direccion.FechaModificacion = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new DireccionDTO
                {
                    Id = direccion.Id,
                    Calle = direccion.Calle,
                    Numero = direccion.Numero,
                    Departamento = direccion.Departamento,
                    Comuna = direccion.Comuna,
                    Region = direccion.Region,
                    CodigoPostal = direccion.CodigoPostal,
                    EsPrincipal = direccion.EsPrincipal ?? false,
                    FechaModificacion = direccion.FechaModificacion
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
} 