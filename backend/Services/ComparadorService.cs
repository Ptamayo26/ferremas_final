using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ferremas.Api.Data;
using Ferremas.Api.Models;
using Ferremas.Api.DTOs;
using Ferremas.Api.Services.Interfaces;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Ferremas.Api.Services
{
    public class ComparadorService : IComparadorService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ComparadorService> _logger;

        public ComparadorService(AppDbContext context, ILogger<ComparadorService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ComparadorPrecioDTO>> GetAllPreciosAsync()
        {
            try
            {
                var precios = await _context.ComparadorPrecios
                    .Include(cp => cp.Producto)
                    .Select(cp => new ComparadorPrecioDTO
                    {
                        Id = cp.Id,
                        ProductoId = cp.ProductoId,
                        ProductoNombre = cp.Producto.Nombre ?? "Desconocido",
                        Competidor = cp.Competidor,
                        PrecioCompetidor = cp.PrecioCompetidor,
                        FechaConsulta = cp.FechaConsulta
                    })
                    .ToListAsync();

                return precios;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los precios");
                throw;
            }
        }

        public async Task<ComparadorPrecioDTO?> GetPrecioByIdAsync(int id)
        {
            try
            {
                var cp = await _context.ComparadorPrecios
                    .Include(cp => cp.Producto)
                    .Select(cp => new ComparadorPrecioDTO
                    {
                        Id = cp.Id,
                        ProductoId = cp.ProductoId,
                        ProductoNombre = cp.Producto.Nombre ?? "Desconocido",
                        Competidor = cp.Competidor,
                        PrecioCompetidor = cp.PrecioCompetidor,
                        FechaConsulta = cp.FechaConsulta
                    })
                    .FirstOrDefaultAsync(cp => cp.Id == id);

                return cp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el precio por ID {Id}", id);
                throw;
            }
        }

        public async Task<ComparadorPrecioDTO> CreatePrecioAsync(ComparadorPrecioCreateDTO dto)
        {
            var comparadorPrecio = new ComparadorPrecio
            {
                ProductoId = dto.ProductoId,
                Competidor = dto.Competidor,
                PrecioCompetidor = dto.PrecioCompetidor,
                FechaConsulta = DateTime.UtcNow,
                TipoFuente = dto.TipoFuente,
                FuenteId = dto.FuenteId,
                UrlProducto = dto.UrlProducto
            };

            _context.ComparadorPrecios.Add(comparadorPrecio);
            await _context.SaveChangesAsync();

            var producto = await _context.Productos.FindAsync(dto.ProductoId);

            return new ComparadorPrecioDTO
            {
                Id = comparadorPrecio.Id,
                ProductoId = comparadorPrecio.ProductoId,
                ProductoNombre = producto?.Nombre ?? "Desconocido",
                Competidor = comparadorPrecio.Competidor,
                PrecioCompetidor = comparadorPrecio.PrecioCompetidor,
                FechaConsulta = comparadorPrecio.FechaConsulta
            };
        }

        public async Task<ComparadorPrecioDTO?> UpdatePrecioAsync(int id, ComparadorPrecioUpdateDTO dto)
        {
            try
            {
                var comparadorPrecio = await _context.ComparadorPrecios
                    .FirstOrDefaultAsync(cp => cp.Id == id);

                if (comparadorPrecio == null)
                    return null;

                comparadorPrecio.PrecioCompetidor = dto.PrecioCompetidor;
                comparadorPrecio.FechaConsulta = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                var producto = await _context.Productos
                    .Select(p => new { p.Id, p.Nombre })
                    .FirstOrDefaultAsync(p => p.Id == comparadorPrecio.ProductoId);

                return new ComparadorPrecioDTO
                {
                    Id = comparadorPrecio.Id,
                    ProductoId = comparadorPrecio.ProductoId,
                    ProductoNombre = producto?.Nombre ?? "Desconocido",
                    Competidor = comparadorPrecio.Competidor,
                    PrecioCompetidor = comparadorPrecio.PrecioCompetidor,
                    FechaConsulta = comparadorPrecio.FechaConsulta
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el precio con ID {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeletePrecioAsync(int id)
        {
            var comparadorPrecio = await _context.ComparadorPrecios.FindAsync(id);
            if (comparadorPrecio == null)
                return false;

            _context.ComparadorPrecios.Remove(comparadorPrecio);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ComparadorPrecioDTO>> GetPreciosByProductoAsync(int productoId)
        {
            try
            {
                var precios = await _context.ComparadorPrecios
                    .Where(cp => cp.ProductoId == productoId)
                    .Select(cp => new ComparadorPrecioDTO
                    {
                        Id = cp.Id,
                        ProductoId = cp.ProductoId,
                        ProductoNombre = cp.Producto.Nombre ?? "Desconocido",
                        Competidor = cp.Competidor,
                        PrecioCompetidor = cp.PrecioCompetidor,
                        FechaConsulta = cp.FechaConsulta
                    })
                    .ToListAsync();

                return precios;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener precios por producto {ProductoId}", productoId);
                throw;
            }
        }

        // Obtener todas las comparaciones por ID de producto
        public async Task<IEnumerable<ComparadorPrecio>> GetComparacionesByProducto(int productoId)
        {
            return await _context.ComparadorPrecios
                .Where(cp => cp.ProductoId == productoId)
                .ToListAsync();
        }

        // Obtener una comparación específica por ID
        public async Task<ComparadorPrecio?> GetComparacionById(int id)
        {
            return await _context.ComparadorPrecios.FindAsync(id);
        }

        // Comparar precios de productos similares
        public async Task<IEnumerable<ProductoResponseDTO>> CompararPrecios(int productoId)
        {
            try
            {
                var producto = await _context.Productos
                    .Select(p => new { p.Id, p.Nombre, p.Precio })
                    .FirstOrDefaultAsync(p => p.Id == productoId);

                if (producto == null)
                    return new List<ProductoResponseDTO>();

                var productosSimilares = await _context.Productos
                    .Select(p => new ProductoResponseDTO
                    {
                        Id = p.Id,
                        Codigo = p.Codigo ?? "Sin Código",
                        Nombre = p.Nombre ?? "Desconocido",
                        Descripcion = p.Descripcion ?? "No disponible",
                        Precio = p.Precio,
                        ImagenUrl = p.ImagenUrl ?? "Sin imagen",
                        Especificaciones = p.Especificaciones ?? "No especificadas",
                        FechaCreacion = p.FechaCreacion,
                        FechaModificacion = p.FechaModificacion ?? DateTime.MinValue
                    })
                    .Where(p => p.Id != productoId)
                    .ToListAsync();

                return productosSimilares;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al comparar precios para el producto {ProductoId}", productoId);
                throw;
            }
        }

        // Obtener el historial de precios de un producto
        public async Task<IEnumerable<object>> ObtenerHistorialPrecios(int productoId)
        {
            try
            {
                var producto = await _context.Productos
                    .Select(p => new { p.Id, p.Precio })
                    .FirstOrDefaultAsync(p => p.Id == productoId);

                if (producto == null)
                    return new List<object>();

                var historial = new List<object>();
                var fechaBase = DateTime.Now.AddMonths(-6);
                var precioBase = producto.Precio * 0.8m;

                for (int i = 0; i < 6; i++)
                {
                    historial.Add(new
                    {
                        Fecha = fechaBase.AddMonths(i),
                        Precio = precioBase * (1 + (i * 0.05m))
                    });
                }

                return historial;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener historial de precios para el producto {ProductoId}", productoId);
                throw;
            }
        }

        // Modificado para que el tipo de retorno sea nullable
        public async Task<ComparadorPrecioResponseDTO?> GetComparacionResponseDTO(int id)
        {
            var cp = await _context.ComparadorPrecios.FindAsync(id);
            if (cp == null)
                return null;

            var response = new ComparadorPrecioResponseDTO
            {
                Id = cp.Id,
                ProductoId = cp.ProductoId,
                PrecioCompetidor = cp.PrecioCompetidor,
                Competidor = cp.Competidor,
                FechaConsulta = cp.FechaConsulta
            };

            return response;
        }

        public async Task GuardarHistorialComparacion(int userId, string producto, decimal precioFerremas, List<Models.ComparacionResultado> resultados)
        {
            try
            {
                var historial = new ComparacionHistorial
                {
                    UsuarioId = userId,
                    Producto = producto,
                    PrecioFerremas = precioFerremas,
                    Resultados = System.Text.Json.JsonSerializer.Serialize(resultados),
                    FechaComparacion = DateTime.UtcNow
                };

                _context.ComparacionesHistorial.Add(historial);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar el historial de comparación");
                throw new InvalidOperationException("Error al guardar el historial de comparación", ex);
            }
        }

        public async Task<List<ComparacionHistorial>> ObtenerHistorialComparacion(int userId)
        {
            try
            {
                return await _context.ComparacionesHistorial
                    .Where(h => h.UsuarioId == userId)
                    .OrderByDescending(h => h.FechaComparacion)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el historial de comparación");
                throw new InvalidOperationException("Error al obtener el historial de comparación", ex);
            }
        }

        public async Task<List<ComparacionHistorial>> ObtenerHistorialUsuario(int userId)
        {
            try
            {
                return await _context.ComparacionesHistorial
                    .Where(h => h.UsuarioId == userId)
                    .OrderByDescending(h => h.FechaComparacion)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el historial del usuario");
                throw new InvalidOperationException("Error al obtener el historial del usuario", ex);
            }
        }

        public List<ComparacionHistorial> ObtenerHistorialTodos()
        {
            try
            {
                return _context.ComparacionesHistorial
                    .OrderByDescending(h => h.FechaComparacion)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el historial completo");
                throw new InvalidOperationException("Error al obtener el historial completo", ex);
            }
        }
    }
}