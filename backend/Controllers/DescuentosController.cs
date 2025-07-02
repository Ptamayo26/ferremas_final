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
    public class DescuentosController : ControllerBase
    {
        private readonly IDescuentoService _descuentoService;

        public DescuentosController(IDescuentoService descuentoService)
        {
            _descuentoService = descuentoService;
        }

        [HttpGet]
        public async Task<ActionResult<List<DescuentoResponseDTO>>> GetDescuentos()
        {
            var descuentos = await _descuentoService.ObtenerTodos();
            return Ok(descuentos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DescuentoResponseDTO>> GetDescuento(int id)
        {
            var descuento = await _descuentoService.ObtenerPorId(id);
            if (descuento == null)
                return NotFound();

            return Ok(descuento);
        }

        [HttpGet("codigo/{codigo}")]
        public async Task<ActionResult<DescuentoResponseDTO>> GetDescuentoPorCodigo(string codigo)
        {
            var descuento = await _descuentoService.ObtenerPorCodigo(codigo);
            if (descuento == null)
                return NotFound();

            return Ok(descuento);
        }

        [HttpPost]
        [Authorize(Policy = "RequireAdministrador")]
        public async Task<ActionResult<DescuentoResponseDTO>> CreateDescuento(DescuentoCreateDTO dto)
        {
            var descuento = await _descuentoService.Crear(dto);
            return CreatedAtAction(nameof(GetDescuento), new { id = descuento.Id }, descuento);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "RequireAdministrador")]
        public async Task<IActionResult> UpdateDescuento(int id, DescuentoCreateDTO dto)
        {
            var result = await _descuentoService.Actualizar(id, dto);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireAdministrador")]
        public async Task<IActionResult> DeleteDescuento(int id)
        {
            var result = await _descuentoService.Eliminar(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
} 