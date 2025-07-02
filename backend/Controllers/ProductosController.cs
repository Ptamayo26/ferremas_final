using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Ferremas.Api.Models;
using Ferremas.Api.Data;
using Ferremas.Api.DTOs;
using Ferremas.Api.Services.Interfaces;

namespace Ferremas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IProductosService _productosService;

        public ProductosController(AppDbContext context, IProductosService productosService)
        {
            _context = context;
            _productosService = productosService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductoResponseDTO>>> GetProductos()
        {
            try
            {
                var productos = await _context.Productos
                    .Include(p => p.Categoria)
                    .Include(p => p.Marca)
                    .Select(p => new ProductoResponseDTO
                    {
                        Id = p.Id,
                        Codigo = p.Codigo,
                        Nombre = p.Nombre,
                        Descripcion = p.Descripcion,
                        Precio = p.Precio,
                        Stock = p.Stock,
                        CategoriaId = p.CategoriaId ?? 0,
                        CategoriaNombre = p.Categoria != null ? p.Categoria.Nombre : "Sin categoría",
                        MarcaId = p.MarcaId ?? 0,
                        MarcaNombre = p.Marca != null ? p.Marca.Nombre : "Sin marca",
                        ImagenUrl = p.ImagenUrl ?? "",
                        Especificaciones = p.Especificaciones ?? "",
                        FechaCreacion = p.FechaCreacion,
                        FechaModificacion = p.FechaModificacion,
                        Activo = p.Activo,
                        PrecioOriginal = p.Precio,
                        PrecioConDescuento = p.Categoria != null && p.Categoria.DescuentoPorcentaje > 0
                            ? Math.Round(p.Precio * (1 - (p.Categoria.DescuentoPorcentaje / 100)), 0)
                            : p.Precio
                    })
                    .ToListAsync();

                return Ok(new {
                    descripcion = "Lista de productos activos en el sistema",
                    total = productos.Count,
                    productos = productos
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener productos: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { 
                    error = "Error al obtener los productos",
                    detalles = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductoResponseDTO>> GetProducto(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
            {
                return NotFound();
            }

            var response = new ProductoResponseDTO
            {
                Id = producto.Id,
                Codigo = producto.Codigo,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion ?? string.Empty,
                Precio = producto.Precio,
                Stock = producto.Stock,
                CategoriaId = producto.CategoriaId ?? 0,
                CategoriaNombre = producto.Categoria != null ? producto.Categoria.Nombre : null,
                MarcaId = producto.MarcaId ?? 0,
                MarcaNombre = producto.Marca != null ? producto.Marca.Nombre : null,
                ImagenUrl = producto.ImagenUrl,
                Especificaciones = producto.Especificaciones,
                FechaCreacion = producto.FechaCreacion,
                FechaModificacion = producto.FechaModificacion,
                Activo = producto.Activo
            };

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "administrador,bodeguero")]
        public async Task<ActionResult<ProductoResponseDTO>> CrearProducto([FromBody] ProductoCreateDTO dto)
        {
            var producto = new Producto
            {
                Codigo = dto.Codigo,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Precio = dto.Precio,
                Stock = dto.Stock,
                CategoriaId = dto.CategoriaId,
                MarcaId = dto.MarcaId,
                ImagenUrl = dto.ImagenUrl,
                Especificaciones = dto.Especificaciones,
                FechaCreacion = DateTime.UtcNow,
                Activo = true
            };
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            // Mapear a DTO de respuesta
            var response = new ProductoResponseDTO
            {
                Id = producto.Id,
                Codigo = producto.Codigo,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion ?? string.Empty,
                Precio = producto.Precio,
                Stock = producto.Stock,
                CategoriaId = producto.CategoriaId ?? 0,
                CategoriaNombre = null, // Se puede poblar si se desea
                MarcaId = producto.MarcaId ?? 0,
                MarcaNombre = null, // Se puede poblar si se desea
                ImagenUrl = producto.ImagenUrl,
                Especificaciones = producto.Especificaciones,
                FechaCreacion = producto.FechaCreacion,
                FechaModificacion = producto.FechaModificacion,
                Activo = producto.Activo
            };

            return CreatedAtAction(nameof(GetProducto), new { id = producto.Id }, response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "administrador,bodeguero")]
        public async Task<ActionResult<ProductoResponseDTO>> ActualizarProducto(int id, [FromBody] ProductoUpdateDTO dto)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                return NotFound();

            if (dto.Codigo != null) producto.Codigo = dto.Codigo;
            if (dto.Nombre != null) producto.Nombre = dto.Nombre;
            if (dto.Descripcion != null) producto.Descripcion = dto.Descripcion;
            if (dto.Precio.HasValue) producto.Precio = dto.Precio.Value;
            if (dto.Stock.HasValue) producto.Stock = dto.Stock.Value;
            if (dto.CategoriaId.HasValue) producto.CategoriaId = dto.CategoriaId.Value;
            if (dto.MarcaId.HasValue) producto.MarcaId = dto.MarcaId.Value;
            if (dto.ImagenUrl != null) producto.ImagenUrl = dto.ImagenUrl;
            if (dto.Especificaciones != null) producto.Especificaciones = dto.Especificaciones;

            producto.FechaModificacion = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var response = new ProductoResponseDTO
            {
                Id = producto.Id,
                Codigo = producto.Codigo,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion ?? string.Empty,
                Precio = producto.Precio,
                Stock = producto.Stock,
                CategoriaId = producto.CategoriaId ?? 0,
                CategoriaNombre = producto.Categoria != null ? producto.Categoria.Nombre : null,
                MarcaId = producto.MarcaId ?? 0,
                MarcaNombre = producto.Marca != null ? producto.Marca.Nombre : null,
                ImagenUrl = producto.ImagenUrl,
                Especificaciones = producto.Especificaciones,
                FechaCreacion = producto.FechaCreacion,
                FechaModificacion = producto.FechaModificacion,
                Activo = producto.Activo
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireAdministrador")]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Producto eliminado exitosamente" });
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }

        [HttpGet("categoria/{categoriaId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductoResponseDTO>>> GetByCategoria(int categoriaId)
        {
            var productos = await _productosService.GetByCategoria(categoriaId);
            return Ok(productos);
        }

        [HttpGet("marca/{marcaId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductoResponseDTO>>> GetByMarca(int marcaId)
        {
            var productos = await _productosService.GetByMarca(marcaId);
            if (productos == null || !productos.Any())
            {
                return NotFound(new { mensaje = "No hay productos para esta marca." });
            }
            return Ok(productos);
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductoResponseDTO>>> Search([FromQuery] string termino)
        {
            var productos = await _productosService.Search(termino);
            return Ok(productos);
        }

        [HttpGet("filtrar")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductoResponseDTO>>> Filtrar(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = "nombre",
            [FromQuery] string? sortDir = "asc",
            [FromQuery] int? categoriaId = null,
            [FromQuery] int? marcaId = null,
            [FromQuery] string? termino = null)
        {
            var query = _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Where(p => p.Activo);

            if (categoriaId.HasValue)
                query = query.Where(p => p.CategoriaId == categoriaId);
            if (marcaId.HasValue)
                query = query.Where(p => p.MarcaId == marcaId);
            if (!string.IsNullOrEmpty(termino))
                query = query.Where(p => (p.Nombre != null && p.Nombre.Contains(termino)) || (p.Descripcion != null && p.Descripcion.Contains(termino)));

            // Ordenamiento
            if (sortBy != null)
            {
                if (sortBy.ToLower() == "precio")
                    query = sortDir == "desc" ? query.OrderByDescending(p => p.Precio) : query.OrderBy(p => p.Precio);
                else if (sortBy.ToLower() == "stock")
                    query = sortDir == "desc" ? query.OrderByDescending(p => p.Stock) : query.OrderBy(p => p.Stock);
                else
                    query = sortDir == "desc" ? query.OrderByDescending(p => p.Nombre) : query.OrderBy(p => p.Nombre);
            }

            var total = await query.CountAsync();
            var productos = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var productosDto = productos.Select(p => new ProductoResponseDTO
            {
                Id = p.Id,
                Codigo = p.Codigo,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion ?? string.Empty,
                Precio = p.Precio,
                Stock = p.Stock,
                CategoriaId = p.CategoriaId ?? 0,
                CategoriaNombre = p.Categoria != null ? p.Categoria.Nombre : null,
                MarcaId = p.MarcaId ?? 0,
                MarcaNombre = p.Marca != null ? p.Marca.Nombre : null,
                ImagenUrl = p.ImagenUrl,
                Especificaciones = p.Especificaciones,
                FechaCreacion = p.FechaCreacion,
                FechaModificacion = p.FechaModificacion,
                Activo = p.Activo
            }).ToList();

            return Ok(new { total, page, pageSize, productos = productosDto });
        }

        [HttpPatch("{id}/stock")]
        [Authorize(Roles = "administrador,bodeguero")]
        public async Task<ActionResult<ProductoResponseDTO>> UpdateStock(int id, [FromBody] int cantidad)
        {
            var producto = await _productosService.UpdateStock(id, cantidad);
            if (producto == null)
                return NotFound();

            var response = new ProductoResponseDTO
            {
                Id = producto.Id,
                Codigo = producto.Codigo,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion ?? string.Empty,
                Precio = producto.Precio,
                Stock = producto.Stock,
                CategoriaId = producto.CategoriaId,
                CategoriaNombre = null,
                MarcaId = producto.MarcaId,
                MarcaNombre = null,
                ImagenUrl = producto.ImagenUrl,
                Especificaciones = producto.Especificaciones,
                FechaCreacion = producto.FechaCreacion,
                FechaModificacion = producto.FechaModificacion,
                Activo = producto.Activo
            };

            // Si el stock es 0 o menor, agrega un mensaje
            if (producto.Stock <= 0)
            {
                return Ok(new { mensaje = "¡Atención! El producto está sin stock.", producto = response });
            }

            return Ok(response);
        }

        [HttpGet("activos")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductoResponseDTO>>> GetActivos()
        {
            var productos = await _context.Productos
                .Where(p => p.Activo)
                .ToListAsync();

            var productosDto = productos.Select(p => new ProductoResponseDTO
            {
                Id = p.Id,
                Codigo = p.Codigo,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion ?? string.Empty,
                Precio = p.Precio,
                Stock = p.Stock,
                CategoriaId = p.CategoriaId ?? 0,
                CategoriaNombre = null,
                MarcaId = p.MarcaId ?? 0,
                MarcaNombre = null,
                ImagenUrl = p.ImagenUrl,
                Especificaciones = p.Especificaciones,
                FechaCreacion = p.FechaCreacion,
                FechaModificacion = p.FechaModificacion,
                Activo = p.Activo
            }).ToList();

            return Ok(productosDto);
        }

        [AllowAnonymous]
        [HttpGet("publico/whatsapp-link")]
        public IActionResult GenerarLinkWhatsapp([FromQuery] string telefono, [FromQuery] string producto)
        {
            if (string.IsNullOrWhiteSpace(telefono) || string.IsNullOrWhiteSpace(producto))
                return BadRequest("Debe proporcionar un número de teléfono y un nombre de producto.");

            var telefonoLimpio = telefono.Replace("+", "").Replace(" ", "").Trim();
            var mensaje = $"Hola, estoy interesado en el producto '{producto}'. ¿Podrías darme más información?";
            var mensajeEncoded = Uri.EscapeDataString(mensaje);

            var url = $"https://wa.me/{telefonoLimpio}?text={mensajeEncoded}";
            return Ok(new { whatsapp_url = url });
        }

        [HttpPatch("{id}/imagen")]
        [Authorize(Roles = "administrador,bodeguero")]
        public async Task<ActionResult<ProductoResponseDTO>> UpdateImagen(int id, [FromBody] string imagenUrl)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                return NotFound();

            producto.ImagenUrl = imagenUrl;
            producto.FechaModificacion = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var response = new ProductoResponseDTO
            {
                Id = producto.Id,
                Codigo = producto.Codigo,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion ?? string.Empty,
                Precio = producto.Precio,
                Stock = producto.Stock,
                CategoriaId = producto.CategoriaId ?? 0,
                CategoriaNombre = producto.Categoria != null ? producto.Categoria.Nombre : null,
                MarcaId = producto.MarcaId ?? 0,
                MarcaNombre = producto.Marca != null ? producto.Marca.Nombre : null,
                ImagenUrl = producto.ImagenUrl,
                Especificaciones = producto.Especificaciones,
                FechaCreacion = producto.FechaCreacion,
                FechaModificacion = producto.FechaModificacion,
                Activo = producto.Activo
            };

            return Ok(response);
        }
    }
}