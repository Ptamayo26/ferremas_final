using Ferremas.Api.DTOs;
using Ferremas.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Ferremas.Api.Data;

namespace Ferremas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "contador,administrador")]
    public class ReportesController : ControllerBase
    {
        private readonly IReportesService _reportesService;
        private readonly AppDbContext _context;

        public ReportesController(AppDbContext context)
        {
            _context = context;
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

        [HttpGet("finanzas")]
        [Authorize(Roles = "contador,administrador")]
        public async Task<IActionResult> GetFinanzas()
        {
            Console.WriteLine("Claims del usuario:");
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"{claim.Type}: {claim.Value}");
            }
            var ventas = await _context.Pedidos.CountAsync();
            // Cambiado: ahora cuenta pagos 'approved' o 'pendiente' como ingresos
            var ingresos = await _context.Pagos
                .Where(p => p.Estado == "approved" || p.Estado == "pendiente")
                .SumAsync(p => (decimal?)p.Monto) ?? 0;
            var egresos = 0;
            var utilidad = ingresos - egresos;
            var reporte = new {
                Fecha = DateTime.Now,
                TotalVentas = ventas,
                TotalIngresos = ingresos,
                TotalEgresos = egresos,
                Utilidad = utilidad
            };
            return Ok(new { exito = true, datos = new[] { reporte } });
        }
    }
} 