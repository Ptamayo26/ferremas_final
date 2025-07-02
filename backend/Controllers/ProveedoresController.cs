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
    public class ProveedoresController : ControllerBase
    {
        private readonly IProveedoresService _proveedoresService;

        public ProveedoresController(IProveedoresService proveedoresService)
        {
            _proveedoresService = proveedoresService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProveedorResponseDTO>>> GetProveedores()
        {
            var proveedores = await _proveedoresService.ObtenerTodos();
            return Ok(proveedores);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProveedorResponseDTO>> GetProveedor(int id)
        {
            var proveedor = await _proveedoresService.ObtenerPorId(id);
            if (proveedor == null)
                return NotFound();

            return Ok(proveedor);
        }

        [HttpPost]
        [Authorize(Policy = "RequireAdministrador")]
        public async Task<ActionResult<ProveedorResponseDTO>> CreateProveedor(ProveedorCreateDTO dto)
        {
            var proveedor = await _proveedoresService.Crear(dto);
            return CreatedAtAction(nameof(GetProveedor), new { id = proveedor.Id }, proveedor);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireAdministrador")]
        public async Task<IActionResult> DeleteProveedor(int id)
        {
            var result = await _proveedoresService.Eliminar(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
} 