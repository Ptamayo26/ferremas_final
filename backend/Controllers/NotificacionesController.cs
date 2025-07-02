using Ferremas.Api.DTOs;
using Ferremas.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ferremas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificacionesController : ControllerBase
    {
        private readonly INotificacionesService _notificacionesService;

        public NotificacionesController(INotificacionesService notificacionesService)
        {
            _notificacionesService = notificacionesService;
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<List<NotificacionResponseDTO>>> GetNotificacionesPorUsuario(int usuarioId)
        {
            var notificaciones = await _notificacionesService.ObtenerPorUsuario(usuarioId);
            return Ok(notificaciones);
        }

        [HttpPost]
        [Authorize(Policy = "RequireAdministrador")]
        public async Task<ActionResult<NotificacionResponseDTO>> CreateNotificacion(NotificacionCreateDTO dto)
        {
            var notificacion = await _notificacionesService.Crear(dto);
            return CreatedAtAction(nameof(GetNotificacionesPorUsuario), new { usuarioId = dto.UsuarioId }, notificacion);
        }

        [HttpPost("{id}/marcar-leida")]
        public async Task<IActionResult> MarcarComoLeida(int id)
        {
            var result = await _notificacionesService.MarcarComoLeida(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
} 