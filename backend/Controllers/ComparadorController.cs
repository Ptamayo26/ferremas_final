using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ferremas.Api.Models;
using Ferremas.Api.DTOs;
using Ferremas.Api.Services.Interfaces;
using Ferremas.Api.Data;
using Ferremas.Api.Services;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace Ferremas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ComparadorController : ControllerBase
    {
        private readonly IComparadorService _comparadorService;
        private readonly MercadoLibreService _mercadoLibreService;
        private readonly AppDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ComparadorController> _logger;

        public ComparadorController(
            IComparadorService comparadorService,
            MercadoLibreService mercadoLibreService,
            AppDbContext db,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ComparadorController> logger)
        {
            _comparadorService = comparadorService ?? throw new ArgumentNullException(nameof(comparadorService));
            _mercadoLibreService = mercadoLibreService ?? throw new ArgumentNullException(nameof(mercadoLibreService));
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("comparar")]
        [Authorize(Policy = "RequireRole")]
        public async Task<IActionResult> Comparar([FromBody] ComparacionRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Producto))
                {
                    return BadRequest("El nombre del producto es requerido");
                }

                var resultados = new List<Models.ComparacionResultado>();

                // Buscar en Mercado Libre
                try
                {
                    var mlResultados = await _mercadoLibreService.BuscarProductoAsync(request.Producto);
                    
                    if (mlResultados != null && mlResultados.Any())
                    {
                        foreach (var producto in mlResultados.Take(5))
                        {
                            resultados.Add(new Models.ComparacionResultado
                            {
                                Tienda = "Mercado Libre",
                                NombreProducto = producto.NombreProducto,
                                Precio = producto.Precio,
                                Enlace = producto.Enlace,
                                Imagen = producto.Imagen,
                                Vendedor = producto.Vendedor,
                                ReputacionVendedor = producto.ReputacionVendedor,
                                DiferenciaPrecio = request.PrecioFerremas - producto.Precio,
                                PorcentajeDiferencia = ((request.PrecioFerremas - producto.Precio) / request.PrecioFerremas) * 100
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al buscar en Mercado Libre");
                    // Continuamos con la búsqueda en otras tiendas
                }

                // Ordenar resultados por precio
                resultados = resultados.OrderBy(r => r.Precio).ToList();

                // Guardar historial si el usuario está autenticado
                if (User.Identity.IsAuthenticated)
                {
                    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (!string.IsNullOrEmpty(userId))
                    {
                        await _comparadorService.GuardarHistorialComparacion(
                            int.Parse(userId),
                            request.Producto,
                            request.PrecioFerremas,
                            resultados
                        );
                    }
                }

                return Ok(new
                {
                    Resultados = resultados,
                    TotalResultados = resultados.Count,
                    Mensaje = resultados.Any() 
                        ? "Comparación realizada con éxito" 
                        : "No se encontraron resultados para comparar"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar la comparación");
                return StatusCode(500, "Error al realizar la comparación");
            }
        }

        [HttpGet("historial")]
        [Authorize(Policy = "RequireRole")]
        public async Task<IActionResult> ObtenerHistorial()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var historial = await _comparadorService.ObtenerHistorialUsuario(int.Parse(userId));
                var resultados = new List<Models.ComparacionResultado>();

                foreach (var item in historial)
                {
                    var comparaciones = JsonSerializer.Deserialize<List<Models.ComparacionResultado>>(item.Resultados);
                    if (comparaciones != null)
                    {
                        resultados.AddRange(comparaciones);
                    }
                }

                return Ok(new { 
                    success = true,
                    data = resultados,
                    message = "Historial obtenido exitosamente"
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener el historial");
                return StatusCode(500, new { 
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener el historial");
                return StatusCode(500, new { 
                    success = false,
                    message = "Error inesperado al obtener el historial"
                });
            }
        }

        [HttpGet("historial/todos")]
        [Authorize(Policy = "RequireAdministrador")]
        public IActionResult HistorialTodos()
        {
            try
            {
                var historial = _comparadorService.ObtenerHistorialTodos();
                var resultados = new List<Models.ComparacionResultado>();

                foreach (var item in historial)
                {
                    var comparaciones = JsonSerializer.Deserialize<List<Models.ComparacionResultado>>(item.Resultados);
                    if (comparaciones != null)
                    {
                        resultados.AddRange(comparaciones);
                    }
                }

                return Ok(new { 
                    success = true,
                    data = resultados,
                    message = "Historial completo obtenido exitosamente"
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener el historial completo");
                return StatusCode(500, new { 
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener el historial completo");
                return StatusCode(500, new { 
                    success = false,
                    message = "Error inesperado al obtener el historial completo"
                });
            }
        }
    }

    public class ComparacionRequest
    {
        [Required(ErrorMessage = "El nombre del producto es requerido")]
        public string Producto { get; set; } = string.Empty;
        public string? Marca { get; set; }
        public decimal PrecioFerremas { get; set; }
    }
} 