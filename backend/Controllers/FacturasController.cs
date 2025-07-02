using Ferremas.Api.DTOs;
using Ferremas.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ferremas.Api.Services;
using Ferremas.Api.Data;
using Ferremas.Api.Models;

namespace Ferremas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FacturasController : ControllerBase
    {
        private readonly IFacturaService _facturaService;
        private readonly AppDbContext _context;
        private readonly FacturaPdfService _pdfService;

        public FacturasController(IFacturaService facturaService, AppDbContext context)
        {
            _facturaService = facturaService;
            _context = context;
            _pdfService = new FacturaPdfService();
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

        [HttpGet("boleta/{pedidoId}")]
        [AllowAnonymous]
        public async Task<IActionResult> DescargarBoleta(int pedidoId)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Detalles)
                .ThenInclude(d => d.Producto)
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Id == pedidoId);
            if (pedido == null)
                return NotFound();

            // Buscar cliente por usuario_id
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UsuarioId == pedido.UsuarioId);
            if (cliente == null)
                return NotFound();

            var pdfBytes = _pdfService.GenerarBoleta(pedido, cliente);
            return File(pdfBytes, "application/pdf", $"Boleta_{pedidoId:D8}.pdf");
        }

        [HttpGet("factura/{pedidoId}")]
        [AllowAnonymous]
        public async Task<IActionResult> DescargarFactura(int pedidoId)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Detalles)
                .ThenInclude(d => d.Producto)
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Id == pedidoId);
            if (pedido == null)
                return NotFound();

            // Buscar cliente por usuario_id
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UsuarioId == pedido.UsuarioId);
            if (cliente == null)
                return NotFound();

            // Buscar datos de empresa para la factura
            var datosEmpresa = await _context.Set<DatosFacturaEmpresa>().FirstOrDefaultAsync(d => d.PedidoId == pedidoId);
            if (datosEmpresa == null)
                return BadRequest("No se encontraron datos de empresa para la factura.");

            var pdfBytes = _pdfService.GenerarFactura(pedido, cliente, datosEmpresa);
            return File(pdfBytes, "application/pdf", $"Factura_{pedidoId:D8}.pdf");
        }
    }
} 