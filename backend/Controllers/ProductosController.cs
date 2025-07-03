using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Ferremas.Api.Models;
using Ferremas.Api.Data;
using Ferremas.Api.DTOs;
using Ferremas.Api.Services.Interfaces;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using ClosedXML.Excel;

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

        // Endpoint de prueba simple
        [HttpGet("test")]
        [AllowAnonymous]
        public IActionResult Test()
        {
            try
            {
                return Ok(new { 
                    mensaje = "Endpoint de productos funcionando correctamente",
                    timestamp = DateTime.UtcNow,
                    context = _context != null ? "DbContext disponible" : "DbContext nulo",
                    service = _productosService != null ? "ProductosService disponible" : "ProductosService nulo"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    error = "Error en endpoint de prueba",
                    detalles = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductoResponseDTO>>> GetProductos()
        {
            try
            {
                // Primero verificar si hay productos en la base de datos
                var totalProductos = await _context.Productos.CountAsync();
                Console.WriteLine($"Total de productos en BD: {totalProductos}");

                // Verificar si hay categorías
                var totalCategorias = await _context.Categorias.CountAsync();
                Console.WriteLine($"Total de categorías en BD: {totalCategorias}");

                // Verificar si hay marcas
                var totalMarcas = await _context.Marcas.CountAsync();
                Console.WriteLine($"Total de marcas en BD: {totalMarcas}");

                var productos = await _context.Productos
                    .Include(p => p.Categoria)
                    .Include(p => p.Marca)
                    .Select(p => new ProductoResponseDTO
                    {
                        Id = p.Id,
                        Codigo = p.Codigo,
                        Nombre = p.Nombre,
                        Descripcion = p.Descripcion ?? "",
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

                Console.WriteLine($"Productos procesados exitosamente: {productos.Count}");

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

            decimal precioOriginal = producto.Precio;
            decimal precioConDescuento = producto.Precio;
            if (producto.Categoria != null && producto.Categoria.DescuentoPorcentaje > 0)
            {
                precioConDescuento = Math.Round(producto.Precio * (1 - (producto.Categoria.DescuentoPorcentaje / 100)), 0);
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
                Activo = producto.Activo,
                PrecioOriginal = precioOriginal,
                PrecioConDescuento = precioConDescuento
            };

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "administrador,bodeguero")]
        public async Task<ActionResult<ProductoResponseDTO>> CrearProducto([FromBody] ProductoCreateDTO dto)
        {
            var producto = new Producto
            {
                Codigo = "TEMP", // Temporal, se actualizará después
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

            // Generar el código automático: Mxx-Cyy-zzzzz
            string codigoAuto = $"M{producto.MarcaId?.ToString("D2") ?? "00"}-C{producto.CategoriaId?.ToString("D2") ?? "00"}-{producto.Id.ToString("D5")}";
            producto.Codigo = codigoAuto;
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
        [HttpGet("whatsapp-link")]
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

        [HttpPost("actualizar-codigos")]
        [Authorize(Roles = "administrador")]
        public async Task<IActionResult> ActualizarCodigosProductos()
        {
            var productos = _context.Productos.ToList();
            foreach (var producto in productos)
            {
                string codigoAuto = $"M{(producto.MarcaId ?? 0).ToString("D2")}-C{(producto.CategoriaId ?? 0).ToString("D2")}-{producto.Id.ToString("D5")}";
                producto.Codigo = codigoAuto;
            }
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Códigos actualizados correctamente" });
        }

        // Descargar plantilla CSV para carga masiva (todos los campos relevantes)
        [HttpGet("descargar-plantilla-csv")]
        [Authorize(Roles = "administrador,bodeguero")]
        public IActionResult DescargarPlantillaCsv()
        {
            var csv = "Nombre;Codigo;Descripcion;Precio;Stock;CategoriaId;MarcaId;ImagenUrl;Especificaciones\n" +
                      "Taladro Percutor 650W;HE100;Taladro eléctrico de 650W;59990;25;1;1;https://ejemplo.com/taladro.jpg;Potencia:650W\n";
            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", "plantilla_productos.csv");
        }

        // Previsualizar carga masiva de productos desde CSV (todos los campos)
        [HttpPost("previsualizar-carga-csv")]
        [Authorize(Roles = "administrador,bodeguero")]
        public async Task<IActionResult> PrevisualizarCargaCsv([FromForm] IFormFile archivoCsv)
        {
            if (archivoCsv == null || archivoCsv.Length == 0)
                return BadRequest("No se subió ningún archivo");

            var productos = new List<Producto>();
            var errores = new List<string>();
            var columnasEsperadas = new[] { "Nombre", "Codigo", "Descripcion", "Precio", "Stock", "CategoriaId", "MarcaId", "ImagenUrl", "Especificaciones" };
            try
            {
                if (archivoCsv.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    using var stream = archivoCsv.OpenReadStream();
                    using var workbook = new XLWorkbook(stream);
                    var ws = workbook.Worksheet(1); // Hoja principal
                    var headers = ws.Row(1).Cells().Select(c => c.GetString().Trim()).ToArray();
                    if (headers.Length != columnasEsperadas.Length || !columnasEsperadas.SequenceEqual(headers))
                    {
                        return BadRequest($"El archivo Excel debe tener exactamente las columnas: {string.Join(";", columnasEsperadas)} (en ese orden y sin columnas extra).\nEncabezados recibidos: {string.Join(";", headers)}");
                    }
                    int row = 2;
                    while (!ws.Row(row).IsEmpty())
                    {
                        var c = ws.Row(row).Cells(1, columnasEsperadas.Length).Select(c => c.GetString()).ToArray();
                        // Ignorar filas completamente vacías
                        if (c.All(string.IsNullOrWhiteSpace)) { row++; continue; }
                        // Validar cantidad de columnas
                        if (c.Length != columnasEsperadas.Length) { errores.Add($"Fila {row}: Cantidad de columnas incorrecta (esperado: {columnasEsperadas.Length}, encontrado: {c.Length})"); row++; continue; }
                        string nombre = c[0], codigo = c[1], descripcion = c[2], precioStr = c[3], stockStr = c[4], categoriaIdStr = c[5], marcaIdStr = c[6], imagenUrl = c[7], especificaciones = c[8];
                        if (string.IsNullOrWhiteSpace(nombre)) { errores.Add($"Fila {row}: El nombre es obligatorio."); row++; continue; }
                        if (!decimal.TryParse(precioStr, out decimal precio) || precio < 0) { errores.Add($"Fila {row}: Precio inválido."); row++; continue; }
                        if (!int.TryParse(stockStr, out int stock) || stock < 0) { errores.Add($"Fila {row}: Stock inválido."); row++; continue; }
                        if (!int.TryParse(categoriaIdStr, out int categoriaId) || categoriaId <= 0) { errores.Add($"Fila {row}: CategoriaId inválido."); row++; continue; }
                        if (!int.TryParse(marcaIdStr, out int marcaId) || marcaId <= 0) { errores.Add($"Fila {row}: MarcaId inválido."); row++; continue; }
                        var producto = new Producto
                        {
                            Codigo = string.IsNullOrWhiteSpace(codigo) ? "TEMP" : codigo,
                            Nombre = nombre,
                            Descripcion = descripcion,
                            Precio = precio,
                            Stock = stock,
                            CategoriaId = categoriaId,
                            MarcaId = marcaId,
                            ImagenUrl = string.IsNullOrWhiteSpace(imagenUrl) ? "Sin imagen" : imagenUrl,
                            Especificaciones = especificaciones,
                            FechaCreacion = DateTime.UtcNow,
                            Activo = true
                        };
                        productos.Add(producto);
                        row++;
                    }
                }
                else // CSV
                {
                    using var reader = new StreamReader(archivoCsv.OpenReadStream());
                    using var csv = new CsvHelper.CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
                    {
                        Delimiter = ";",
                        HeaderValidated = null,
                        MissingFieldFound = null
                    });
                    csv.Read();
                    csv.ReadHeader();
                    var headers = csv.HeaderRecord;
                    if (headers == null || headers.Length != columnasEsperadas.Length || !columnasEsperadas.SequenceEqual(headers))
                    {
                        return BadRequest($"El archivo CSV debe tener exactamente las columnas: {string.Join(";", columnasEsperadas)} (en ese orden y sin columnas extra).\nEncabezados recibidos: {string.Join(";", headers ?? new string[0])}");
                    }
                    var registros = csv.GetRecords<dynamic>().ToList();
                    int fila = 2;
                    foreach (var reg in registros)
                    {
                        string nombre = reg.Nombre;
                        string codigo = reg.Codigo;
                        string descripcion = reg.Descripcion;
                        string precioStr = reg.Precio;
                        string stockStr = reg.Stock;
                        string categoriaIdStr = reg.CategoriaId;
                        string marcaIdStr = reg.MarcaId;
                        string imagenUrl = reg.ImagenUrl;
                        string especificaciones = reg.Especificaciones;
                        if (string.IsNullOrWhiteSpace(nombre)) { errores.Add($"Fila {fila}: El nombre es obligatorio."); fila++; continue; }
                        if (!decimal.TryParse(precioStr, out decimal precio) || precio < 0) { errores.Add($"Fila {fila}: Precio inválido."); fila++; continue; }
                        if (!int.TryParse(stockStr, out int stock) || stock < 0) { errores.Add($"Fila {fila}: Stock inválido."); fila++; continue; }
                        if (!int.TryParse(categoriaIdStr, out int categoriaId) || categoriaId <= 0) { errores.Add($"Fila {fila}: CategoriaId inválido."); fila++; continue; }
                        if (!int.TryParse(marcaIdStr, out int marcaId) || marcaId <= 0) { errores.Add($"Fila {fila}: MarcaId inválido."); fila++; continue; }
                        var producto = new Producto
                        {
                            Codigo = string.IsNullOrWhiteSpace(codigo) ? "TEMP" : codigo,
                            Nombre = nombre,
                            Descripcion = descripcion,
                            Precio = precio,
                            Stock = stock,
                            CategoriaId = categoriaId,
                            MarcaId = marcaId,
                            ImagenUrl = string.IsNullOrWhiteSpace(imagenUrl) ? "Sin imagen" : imagenUrl,
                            Especificaciones = especificaciones,
                            FechaCreacion = DateTime.UtcNow,
                            Activo = true
                        };
                        productos.Add(producto);
                        fila++;
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al leer el archivo: {ex.Message}");
            }
            if (errores.Count > 0)
                return BadRequest(new { errores });
            var preview = productos.Select(p => new {
                p.Nombre,
                p.Codigo,
                p.Descripcion,
                p.Precio,
                p.Stock,
                p.CategoriaId,
                p.MarcaId,
                p.ImagenUrl,
                p.Especificaciones
            }).ToList();
            return Ok(new { productos = preview });
        }

        // Guardar productos desde carga masiva CSV (todos los campos)
        [HttpPost("guardar-carga-csv")]
        [Authorize(Roles = "administrador,bodeguero")]
        public async Task<IActionResult> GuardarCargaCsv([FromForm] IFormFile archivoCsv)
        {
            if (archivoCsv == null || archivoCsv.Length == 0)
                return BadRequest("No se subió ningún archivo");

            var productos = new List<Producto>();
            var errores = new List<string>();
            var columnasEsperadas = new[] { "Nombre", "Codigo", "Descripcion", "Precio", "Stock", "CategoriaId", "MarcaId", "ImagenUrl", "Especificaciones" };
            try
            {
                if (archivoCsv.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    using var stream = archivoCsv.OpenReadStream();
                    using var workbook = new XLWorkbook(stream);
                    var ws = workbook.Worksheet(1); // Hoja principal
                    var headers = ws.Row(1).Cells().Select(c => c.GetString().Trim()).ToArray();
                    if (headers.Length != columnasEsperadas.Length || !columnasEsperadas.SequenceEqual(headers))
                    {
                        return BadRequest($"El archivo Excel debe tener exactamente las columnas: {string.Join(";", columnasEsperadas)} (en ese orden y sin columnas extra).\nEncabezados recibidos: {string.Join(";", headers)}");
                    }
                    int row = 2;
                    while (!ws.Row(row).IsEmpty())
                    {
                        var c = ws.Row(row).Cells(1, columnasEsperadas.Length).Select(c => c.GetString()).ToArray();
                        // Ignorar filas completamente vacías
                        if (c.All(string.IsNullOrWhiteSpace)) { row++; continue; }
                        // Validar cantidad de columnas
                        if (c.Length != columnasEsperadas.Length) { errores.Add($"Fila {row}: Cantidad de columnas incorrecta (esperado: {columnasEsperadas.Length}, encontrado: {c.Length})"); row++; continue; }
                        string nombre = c[0], codigo = c[1], descripcion = c[2], precioStr = c[3], stockStr = c[4], categoriaIdStr = c[5], marcaIdStr = c[6], imagenUrl = c[7], especificaciones = c[8];
                        if (string.IsNullOrWhiteSpace(nombre)) { errores.Add($"Fila {row}: El nombre es obligatorio."); row++; continue; }
                        if (!decimal.TryParse(precioStr, out decimal precio) || precio < 0) { errores.Add($"Fila {row}: Precio inválido."); row++; continue; }
                        if (!int.TryParse(stockStr, out int stock) || stock < 0) { errores.Add($"Fila {row}: Stock inválido."); row++; continue; }
                        if (!int.TryParse(categoriaIdStr, out int categoriaId) || categoriaId <= 0) { errores.Add($"Fila {row}: CategoriaId inválido."); row++; continue; }
                        if (!int.TryParse(marcaIdStr, out int marcaId) || marcaId <= 0) { errores.Add($"Fila {row}: MarcaId inválido."); row++; continue; }
                        var producto = new Producto
                        {
                            Codigo = string.IsNullOrWhiteSpace(codigo) ? "TEMP" : codigo,
                            Nombre = nombre,
                            Descripcion = descripcion,
                            Precio = precio,
                            Stock = stock,
                            CategoriaId = categoriaId,
                            MarcaId = marcaId,
                            ImagenUrl = string.IsNullOrWhiteSpace(imagenUrl) ? "Sin imagen" : imagenUrl,
                            Especificaciones = especificaciones,
                            FechaCreacion = DateTime.UtcNow,
                            Activo = true
                        };
                        productos.Add(producto);
                        row++;
                    }
                }
                else // CSV
                {
                    using var reader = new StreamReader(archivoCsv.OpenReadStream());
                    using var csv = new CsvHelper.CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
                    {
                        Delimiter = ";",
                        HeaderValidated = null,
                        MissingFieldFound = null
                    });
                    csv.Read();
                    csv.ReadHeader();
                    var headers = csv.HeaderRecord;
                    if (headers == null || headers.Length != columnasEsperadas.Length || !columnasEsperadas.SequenceEqual(headers))
                    {
                        return BadRequest($"El archivo CSV debe tener exactamente las columnas: {string.Join(";", columnasEsperadas)} (en ese orden y sin columnas extra).\nEncabezados recibidos: {string.Join(";", headers ?? new string[0])}");
                    }
                    var registros = csv.GetRecords<dynamic>().ToList();
                    int fila = 2;
                    foreach (var reg in registros)
                    {
                        string nombre = reg.Nombre;
                        string codigo = reg.Codigo;
                        string descripcion = reg.Descripcion;
                        string precioStr = reg.Precio;
                        string stockStr = reg.Stock;
                        string categoriaIdStr = reg.CategoriaId;
                        string marcaIdStr = reg.MarcaId;
                        string imagenUrl = reg.ImagenUrl;
                        string especificaciones = reg.Especificaciones;
                        if (string.IsNullOrWhiteSpace(nombre)) { errores.Add($"Fila {fila}: El nombre es obligatorio."); fila++; continue; }
                        if (!decimal.TryParse(precioStr, out decimal precio) || precio < 0) { errores.Add($"Fila {fila}: Precio inválido."); fila++; continue; }
                        if (!int.TryParse(stockStr, out int stock) || stock < 0) { errores.Add($"Fila {fila}: Stock inválido."); fila++; continue; }
                        if (!int.TryParse(categoriaIdStr, out int categoriaId) || categoriaId <= 0) { errores.Add($"Fila {fila}: CategoriaId inválido."); fila++; continue; }
                        if (!int.TryParse(marcaIdStr, out int marcaId) || marcaId <= 0) { errores.Add($"Fila {fila}: MarcaId inválido."); fila++; continue; }
                        var producto = new Producto
                        {
                            Codigo = string.IsNullOrWhiteSpace(codigo) ? "TEMP" : codigo,
                            Nombre = nombre,
                            Descripcion = descripcion,
                            Precio = precio,
                            Stock = stock,
                            CategoriaId = categoriaId,
                            MarcaId = marcaId,
                            ImagenUrl = string.IsNullOrWhiteSpace(imagenUrl) ? "Sin imagen" : imagenUrl,
                            Especificaciones = especificaciones,
                            FechaCreacion = DateTime.UtcNow,
                            Activo = true
                        };
                        productos.Add(producto);
                        fila++;
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al leer el archivo: {ex.Message}");
            }
            if (errores.Count > 0)
                return BadRequest(new { errores });
            // Guardar productos en la base de datos
            foreach (var producto in productos)
            {
                _context.Productos.Add(producto);
            }
            await _context.SaveChangesAsync();
            // Actualizar códigos automáticos si no se proporcionó uno
            foreach (var producto in productos)
            {
                if (producto.Codigo == "TEMP" || string.IsNullOrWhiteSpace(producto.Codigo))
                {
                    producto.Codigo = $"M{producto.MarcaId?.ToString("D2") ?? "00"}-C{producto.CategoriaId?.ToString("D2") ?? "00"}-{producto.Id.ToString("D5")}";
                }
            }
            await _context.SaveChangesAsync();
            var productosGuardados = productos.Select(p => new {
                p.Id,
                p.Codigo,
                p.Nombre,
                p.Precio,
                p.Stock,
                p.CategoriaId,
                p.MarcaId,
                p.ImagenUrl
            }).ToList();
            return Ok(new { mensaje = "Productos guardados exitosamente", productos = productosGuardados });
        }

        // Descargar plantilla Excel con listas desplegables para carga masiva
        [HttpGet("descargar-plantilla-excel")]
        [Authorize(Roles = "administrador,bodeguero")]
        public async Task<IActionResult> DescargarPlantillaExcel()
        {
            // Obtener categorías y marcas activas
            var categorias = await _context.Categorias.Where(c => c.Activo).Select(c => new { c.Id, c.Nombre }).ToListAsync();
            var marcas = await _context.Marcas.Where(m => m.Activo).Select(m => new { m.Id, m.Nombre }).ToListAsync();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Productos");
            var wsCat = workbook.Worksheets.Add("Categorias");
            var wsMar = workbook.Worksheets.Add("Marcas");

            // Encabezados
            var headers = new[] { "Nombre", "Codigo", "Descripcion", "Precio", "Stock", "CategoriaId", "MarcaId", "ImagenUrl", "Especificaciones" };
            for (int i = 0; i < headers.Length; i++)
                ws.Cell(1, i + 1).Value = headers[i];

            // Fila de ejemplo
            ws.Cell(2, 1).Value = "Taladro Percutor 650W";
            ws.Cell(2, 2).Value = "HE100";
            ws.Cell(2, 3).Value = "Taladro eléctrico de 650W";
            ws.Cell(2, 4).Value = 59990;
            ws.Cell(2, 5).Value = 25;
            ws.Cell(2, 6).Value = categorias.FirstOrDefault()?.Id ?? 1;
            ws.Cell(2, 7).Value = marcas.FirstOrDefault()?.Id ?? 1;
            ws.Cell(2, 8).Value = "https://ejemplo.com/taladro.jpg";
            ws.Cell(2, 9).Value = "Potencia:650W";

            // Hoja de categorías
            wsCat.Cell(1, 1).Value = "Id";
            wsCat.Cell(1, 2).Value = "Nombre";
            for (int i = 0; i < categorias.Count; i++)
            {
                wsCat.Cell(i + 2, 1).Value = categorias[i].Id;
                wsCat.Cell(i + 2, 2).Value = categorias[i].Nombre;
            }

            // Hoja de marcas
            wsMar.Cell(1, 1).Value = "Id";
            wsMar.Cell(1, 2).Value = "Nombre";
            for (int i = 0; i < marcas.Count; i++)
            {
                wsMar.Cell(i + 2, 1).Value = marcas[i].Id;
                wsMar.Cell(i + 2, 2).Value = marcas[i].Nombre;
            }

            // Definir nombres de rango para validación
            var catRange = wsCat.Range(2, 1, categorias.Count + 1, 1);
            var marRange = wsMar.Range(2, 1, marcas.Count + 1, 1);
            catRange.AddToNamed("CategoriasIds", XLScope.Workbook);
            marRange.AddToNamed("MarcasIds", XLScope.Workbook);

            // Validación de datos (listas desplegables)
            var catValidation = ws.Range("F2:F1000").CreateDataValidation();
            catValidation.List("=CategoriasIds");
            var marValidation = ws.Range("G2:G1000").CreateDataValidation();
            marValidation.List("=MarcasIds");

            // Ajustar ancho de columnas
            ws.Columns().AdjustToContents();
            wsCat.Columns().AdjustToContents();
            wsMar.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "plantilla_productos.xlsx");
        }
    }
}