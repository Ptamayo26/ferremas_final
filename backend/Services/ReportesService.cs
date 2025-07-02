using Ferremas.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ferremas.Api.Data;
using Ferremas.Api.DTOs;

namespace Ferremas.Api.Services
{
    public class ReportesService : IReportesService
    {
        private readonly AppDbContext _context;

        public ReportesService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> ObtenerVentasTotalesMes(int anio, int mes)
        {
            return await _context.Pedidos
                .Where(p => p.FechaPedido.HasValue && p.FechaPedido.Value.Month == mes && p.FechaPedido.Value.Year == anio)
                .SumAsync(p => p.Total ?? 0);
        }

        public async Task<List<ProductoVentaDTO>> ObtenerTopProductosVendidos(int cantidad)
        {
            var resultado = await _context.DetallesPedido
                .Include(d => d.Producto)
                .GroupBy(d => new { d.Producto.Id, d.Producto.Nombre })
                .Select(g => new ProductoVentaDTO
                {
                    Id = g.Key.Id,
                    Nombre = g.Key.Nombre,
                    CantidadVendida = g.Sum(x => x.Cantidad ?? 0),
                    TotalVendido = g.Sum(x => (x.Cantidad ?? 0) * (x.PrecioUnitario ?? 0))
                })
                .OrderByDescending(x => x.CantidadVendida)
                .Take(cantidad)
                .ToListAsync();
            return resultado;
        }

        public async Task<Dictionary<string, int>> ObtenerPedidosPorCliente()
        {
            var resultado = await _context.Pedidos
                .Include(p => p.Usuario)
                .GroupBy(p => p.Usuario.Nombre)
                .Select(g => new
                {
                    Cliente = g.Key,
                    Cantidad = g.Count()
                })
                .ToDictionaryAsync(x => x.Cliente, x => x.Cantidad);

            return resultado;
        }

        public async Task<List<ProductoDTO>> ObtenerProductosBajoStock()
        {
            var productos = await _context.Productos
                .Where(p => p.Stock <= p.StockMinimo && p.Activo)
                .Select(p => new ProductoDTO
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Precio = p.Precio,
                    Stock = p.Stock,
                    StockMinimo = p.StockMinimo,
                    ImagenUrl = p.ImagenUrl,
                    Activo = p.Activo,
                    CategoriaId = p.CategoriaId ?? 0,
                    MarcaId = p.MarcaId ?? 0
                })
                .ToListAsync();
            return productos;
        }
    }
} 