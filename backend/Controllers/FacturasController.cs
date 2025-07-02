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
    public class FacturasController : ControllerBase
    {
        private readonly IFacturaService _facturaService;

        public FacturasController(IFacturaService facturaService)
        {
            _facturaService = facturaService;
        }

        [HttpGet]
        public async Task<ActionResult<List<FacturaResponseDTO>>> GetFacturas()
        {
            var facturas = await _facturaService.ObtenerTodas();
            return Ok(facturas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FacturaResponseDTO>> GetFactura(int id)
        {
            var factura = await _facturaService.ObtenerPorId(id);
            if (factura == null)
                return NotFound();

            return Ok(factura);
        }

        [HttpPost]
        [Authorize(Policy = "RequireAdministrador")]
        public async Task<ActionResult<FacturaResponseDTO>> CreateFactura(FacturaCreateDTO dto)
        {
            var factura = await _facturaService.Crear(dto);
            return CreatedAtAction(nameof(GetFactura), new { id = factura.Id }, factura);
        }

        [HttpPost("{id}/anular")]
        [Authorize(Policy = "RequireAdministrador")]
        public async Task<IActionResult> AnularFactura(int id)
        {
            var result = await _facturaService.Anular(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
} 