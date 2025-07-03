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
using System.Security.Claims;

namespace Ferremas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CarritoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CarritoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarritoItemDTO>>> GetCarrito()
        {
            try
            {
                var usuarioId = GetUsuarioIdFromToken();
                if (usuarioId == null)
                    return Unauthorized("Usuario no autenticado");

                var carritoItems = await _context.Carrito
                    .Include(c => c.Producto)
                        .ThenInclude(p => p.Categoria)
                    .Where(c => c.UsuarioId == usuarioId && c.Activo)
                    .OrderByDescending(c => c.FechaAgregado)
                    .Select(c => new CarritoItemDTO
                    {
                        Id = c.Id,
                        ProductoId = c.ProductoId,
                        ProductoNombre = c.Producto.Nombre,
                        ProductoPrecio = c.Producto.Precio,
                        PrecioOriginal = c.Producto.Precio,
                        PrecioConDescuento = c.Producto.Categoria != null && c.Producto.Categoria.DescuentoPorcentaje > 0
                            ? Math.Round(c.Producto.Precio * (1 - (c.Producto.Categoria.DescuentoPorcentaje / 100)), 0)
                            : c.Producto.Precio,
                        ProductoImagen = c.Producto.ImagenUrl,
                        Cantidad = c.Cantidad,
                        Subtotal = c.Producto.Precio * c.Cantidad,
                        FechaAgregado = c.FechaAgregado
                    })
                    .ToListAsync();

                Console.WriteLine($"[DEBUG] GetCarrito usuarioId: {usuarioId}, items encontrados: {carritoItems.Count}");

                return Ok(carritoItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<CarritoItemDTO>> AgregarAlCarrito([FromBody] AgregarAlCarritoDTO dto)
        {
            try
            {
                var usuarioId = GetUsuarioIdFromToken();
                Console.WriteLine($"[DEBUG] AgregarAlCarrito usuarioId: {usuarioId}");
                if (usuarioId == null)
                    return Unauthorized("Usuario no autenticado");

                // Verificar que el producto existe y tiene stock
                var producto = await _context.Productos
                    .FirstOrDefaultAsync(p => p.Id == dto.ProductoId && p.Activo);
                
                if (producto == null)
                    return NotFound("Producto no encontrado");

                if (producto.Stock < dto.Cantidad)
                    return BadRequest($"Stock insuficiente. Disponible: {producto.Stock}");

                // Verificar si el producto ya está en el carrito
                var itemExistente = await _context.Carrito
                    .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId && 
                                             c.ProductoId == dto.ProductoId && 
                                             c.Activo);

                if (itemExistente != null)
                {
                    // Si el item ya existe, actualizamos la cantidad
                    var nuevaCantidad = itemExistente.Cantidad + dto.Cantidad;
                    if (nuevaCantidad > producto.Stock)
                        return BadRequest($"Stock insuficiente para la cantidad solicitada. Disponible: {producto.Stock}");

                    itemExistente.Cantidad = nuevaCantidad;
                    itemExistente.FechaAgregado = DateTime.UtcNow;
                }
                else
                {
                    // Si el item no existe, lo creamos
                    var carritoItem = new Carrito
                    {
                        UsuarioId = usuarioId.Value,
                        ProductoId = dto.ProductoId,
                        Cantidad = dto.Cantidad,
                        FechaAgregado = DateTime.UtcNow,
                        Activo = true
                    };
                    _context.Carrito.Add(carritoItem);
                }

                await _context.SaveChangesAsync();
                Console.WriteLine($"[DEBUG] Carrito guardado para usuario {usuarioId}. Total items: " +
                    $"{_context.Carrito.Count(c => c.UsuarioId == usuarioId && c.Activo)}");

                // Retornar el item actualizado o recién creado
                var itemActualizado = await _context.Carrito
                    .Include(c => c.Producto)
                    .Where(c => c.UsuarioId == usuarioId && 
                               c.ProductoId == dto.ProductoId && 
                               c.Activo)
                    .Select(c => new CarritoItemDTO
                    {
                        Id = c.Id,
                        ProductoId = c.ProductoId,
                        ProductoNombre = c.Producto.Nombre,
                        ProductoPrecio = c.Producto.Precio,
                        PrecioOriginal = c.Producto.Precio,
                        PrecioConDescuento = c.Producto.Categoria != null && c.Producto.Categoria.DescuentoPorcentaje > 0
                            ? Math.Round(c.Producto.Precio * (1 - (c.Producto.Categoria.DescuentoPorcentaje / 100)), 0)
                            : c.Producto.Precio,
                        ProductoImagen = c.Producto.ImagenUrl,
                        Cantidad = c.Cantidad,
                        Subtotal = c.Producto.Precio * c.Cantidad,
                        FechaAgregado = c.FechaAgregado
                    })
                    .FirstOrDefaultAsync();

                return Ok(itemActualizado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CarritoItemDTO>> ActualizarCantidad(int id, [FromBody] ActualizarCantidadDTO dto)
        {
            try
            {
                var usuarioId = GetUsuarioIdFromToken();
                if (usuarioId == null)
                    return Unauthorized("Usuario no autenticado");

                var carritoItem = await _context.Carrito
                    .Include(c => c.Producto)
                    .FirstOrDefaultAsync(c => c.Id == id && 
                                             c.UsuarioId == usuarioId && 
                                             c.Activo);

                if (carritoItem == null)
                    return NotFound("Item del carrito no encontrado");

                // Verificar stock
                if (dto.Cantidad > carritoItem.Producto.Stock)
                    return BadRequest($"Stock insuficiente. Disponible: {carritoItem.Producto.Stock}");

                if (dto.Cantidad <= 0)
                {
                    // Eliminar item si cantidad es 0 o menor
                    carritoItem.Activo = false;
                }
                else
                {
                    carritoItem.Cantidad = dto.Cantidad;
                }

                await _context.SaveChangesAsync();

                if (dto.Cantidad <= 0)
                    return NoContent();

                return Ok(new CarritoItemDTO
                {
                    Id = carritoItem.Id,
                    ProductoId = carritoItem.ProductoId,
                    ProductoNombre = carritoItem.Producto.Nombre,
                    ProductoPrecio = carritoItem.Producto.Precio,
                    PrecioOriginal = carritoItem.Producto.Precio,
                    PrecioConDescuento = carritoItem.Producto.Categoria != null && carritoItem.Producto.Categoria.DescuentoPorcentaje > 0
                        ? Math.Round(carritoItem.Producto.Precio * (1 - (carritoItem.Producto.Categoria.DescuentoPorcentaje / 100)), 0)
                        : carritoItem.Producto.Precio,
                    ProductoImagen = carritoItem.Producto.ImagenUrl,
                    Cantidad = carritoItem.Cantidad,
                    Subtotal = carritoItem.Producto.Precio * carritoItem.Cantidad,
                    FechaAgregado = carritoItem.FechaAgregado
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarDelCarrito(int id)
        {
            try
            {
                var usuarioId = GetUsuarioIdFromToken();
                if (usuarioId == null)
                    return Unauthorized("Usuario no autenticado");

                var carritoItem = await _context.Carrito
                    .FirstOrDefaultAsync(c => c.Id == id && 
                                             c.UsuarioId == usuarioId && 
                                             c.Activo);

                if (carritoItem == null)
                    return NotFound("Item del carrito no encontrado");

                carritoItem.Activo = false;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> LimpiarCarrito()
        {
            try
            {
                var usuarioId = GetUsuarioIdFromToken();
                if (usuarioId == null)
                    return Unauthorized("Usuario no autenticado");

                var carritoItems = await _context.Carrito
                    .Where(c => c.UsuarioId == usuarioId && c.Activo)
                    .ToListAsync();

                foreach (var item in carritoItems)
                {
                    item.Activo = false;
                }

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("resumen")]
        public async Task<ActionResult<CarritoResumenDTO>> GetResumenCarrito()
        {
            try
            {
                var usuarioId = GetUsuarioIdFromToken();
                if (usuarioId == null)
                    return Unauthorized("Usuario no autenticado");

                var carritoItems = await _context.Carrito
                    .Include(c => c.Producto)
                    .Where(c => c.UsuarioId == usuarioId && c.Activo)
                    .ToListAsync();

                // Filtrar items cuyo producto sea nulo (por ejemplo, si fue eliminado)
                var itemsValidos = carritoItems.Where(c => c.Producto != null).ToList();

                var totalItems = itemsValidos.Sum(c => c.Cantidad);
                var subtotal = itemsValidos.Sum(c => c.Producto.Precio * c.Cantidad);
                var total = subtotal; // Por ahora sin impuestos ni descuentos

                return Ok(new CarritoResumenDTO
                {
                    TotalItems = totalItems,
                    Subtotal = subtotal,
                    Total = total,
                    CantidadProductos = itemsValidos.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        private int? GetUsuarioIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine($"[DEBUG] userIdClaim: {userIdClaim}");
            if (int.TryParse(userIdClaim, out int userId))
                return userId;
            return null;
        }
    }
} 