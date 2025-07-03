using Ferremas.Api.DTOs;
using Ferremas.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Ferremas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "RequireAdministrador")]
    public class ReportesController : ControllerBase
    {
        private readonly IReportesService _reportesService;

        public ReportesController(IReportesService reportesService)
        {
            _reportesService = reportesService;
        }

        [HttpGet("ventas/mes/{mes}/{anio}")]
        public async Task<ActionResult<decimal>> GetVentasTotalesMes(int mes, int anio)
        {
            var total = await _reportesService.ObtenerVentasTotalesMes(anio, mes);
            return Ok(total);
        }

        [HttpGet("productos/top/{cantidad}")]
        public async Task<ActionResult<List<ProductoVentaDTO>>> GetTopProductosVendidos(int cantidad)
        {
            var productos = await _reportesService.ObtenerTopProductosVendidos(cantidad);
            return Ok(productos);
        }

        [HttpGet("inventario/bajo-stock")]
        public async Task<ActionResult<List<ProductoDTO>>> GetProductosBajoStock()
        {
            var productos = await _reportesService.ObtenerProductosBajoStock();
            return Ok(productos);
        }

        [HttpGet("dashboard/ventas/{mes}/{anio}")]
        public async Task<ActionResult<object>> GetDashboardVentas(int mes, int anio)
        {
            var totalVentas = await _reportesService.ObtenerVentasTotalesMes(anio, mes);
            var topProductos = await _reportesService.ObtenerTopProductosVendidos(10);
            
            return Ok(new
            {
                totalVentas = totalVentas,
                cantidadVentas = 0, // TODO: Implementar contador de ventas
                productosVendidos = topProductos.Select(p => new
                {
                    productoId = p.Id,
                    nombre = p.Nombre,
                    cantidad = p.CantidadVendida,
                    total = p.TotalVendido
                }).ToList()
            });
        }

        [HttpGet("dashboard/inventario")]
        public async Task<ActionResult<object>> GetDashboardInventario()
        {
            var productosBajoStock = await _reportesService.ObtenerProductosBajoStock();
            var (valorTotal, totalProductos) = await _reportesService.ObtenerValorInventarioAsync();
            return Ok(new
            {
                valorTotal = valorTotal,
                totalProductos = totalProductos,
                productosAgotados = productosBajoStock.Count(p => p.Stock == 0),
                productosBajoStock = productosBajoStock
            });
        }
    }
} 