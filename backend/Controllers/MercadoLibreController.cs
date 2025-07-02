using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Ferremas.Api.Services;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Ferremas.Api.Models;
using System.Linq;

namespace Ferremas.Api.Controllers
{
    [ApiController]
    [Route("api/mercadolibre")]
    public class MercadoLibreController : ControllerBase
    {
        private readonly MercadoLibreService _mlService;
        private readonly ILogger<MercadoLibreController> _logger;

        public MercadoLibreController(MercadoLibreService mlService, ILogger<MercadoLibreController> logger)
        {
            _mlService = mlService;
            _logger = logger;
        }

        /// <summary>
        /// Endpoint de prueba para verificar conectividad con MercadoLibre
        /// </summary>
        [HttpGet("test")]
        [AllowAnonymous]
        public async Task<IActionResult> TestConexion()
        {
            try
            {
                _logger.LogInformation("=== INICIANDO TEST DE CONEXIÓN MERCADOLIBRE ===");
                
                var resultado = await _mlService.TestearConexion();
                
                return Ok(new {
                    mensaje = "Test de conexión completado",
                    timestamp = DateTime.UtcNow,
                    resultado = resultado,
                    status = "success"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en test de conexión");
                return StatusCode(500, new {
                    error = "Error en test de conexión",
                    detalles = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Busca productos en MercadoLibre con debugging mejorado
        /// </summary>
        /// <param name="query">Término de búsqueda</param>
        /// <returns>Lista de productos encontrados</returns>
        [HttpGet("buscar")]
        [AllowAnonymous]
        public async Task<IActionResult> Buscar([FromQuery] string query)
        {
            try
            {
                _logger.LogInformation("=== INICIANDO BÚSQUEDA EN MERCADOLIBRE ===");
                _logger.LogInformation("Query recibida: '{Query}'", query);

                if (string.IsNullOrWhiteSpace(query))
                {
                    return BadRequest(new { 
                        error = "Debe proporcionar un término de búsqueda válido",
                        ejemplo = "?query=taladro makita",
                        sugerencias = new[] {
                            "taladro",
                            "martillo",
                            "destornillador",
                            "sierra",
                            "herramientas"
                        }
                    });
                }

                // Registrar información de la solicitud
                _logger.LogInformation("User-Agent: {UserAgent}", Request.Headers.UserAgent.FirstOrDefault());
                _logger.LogInformation("Iniciando búsqueda para: '{Query}'", query);

                var resultados = await _mlService.BuscarProductoAsync(query);
                
                _logger.LogInformation("Búsqueda completada. Resultados encontrados: {Count}", resultados.Count);

                if (!resultados.Any())
                {
                    return Ok(new {
                        mensaje = "No se encontraron productos para la búsqueda especificada",
                        total = 0,
                        query = query,
                        sugerencias = new[] {
                            "Intente con términos más simples (ej: 'taladro' en lugar de 'taladro percutor makita')",
                            "Verifique la ortografía",
                            "Pruebe con sinónimos o marcas diferentes"
                        },
                        resultados = new object[0],
                        debug = new {
                            timestamp = DateTime.UtcNow,
                            queryProcessed = query.Trim(),
                            searchAttempted = true
                        }
                    });
                }

                // Calcular estadísticas de precios
                var precios = resultados.Select(r => r.Precio).ToList();
                var precioMinimo = precios.Min();
                var precioMaximo = precios.Max();
                var precioPromedio = precios.Average();

                var response = new {
                    mensaje = "Búsqueda completada exitosamente",
                    total = resultados.Count,
                    query = query,
                    estadisticas = new {
                        precioMinimo = precioMinimo,
                        precioMaximo = precioMaximo,
                        precioPromedio = Math.Round(precioPromedio, 0),
                        diferencia = precioMaximo - precioMinimo,
                        moneda = "CLP"
                    },
                    resultados = resultados.Select(r => new {
                        tienda = r.Tienda,
                        producto = r.NombreProducto,
                        precio = r.Precio,
                        precioFormateado = $"${r.Precio:N0} CLP",
                        enlace = r.Enlace,
                        imagen = r.Imagen.Replace("http://", "https://"), // Forzar HTTPS
                        vendedor = r.Vendedor,
                        reputacion = r.ReputacionVendedor,
                        ventas = r.CantidadVentas,
                        condicion = r.Condicion,
                        envioGratis = r.EnvioGratis ? "Sí" : "No",
                        ubicacion = r.Ubicacion,
                        idProducto = r.IdProducto,
                        // Indicadores de competitividad
                        esMasBarato = r.Precio <= precioPromedio * 0.8m,
                        esMasCaro = r.Precio >= precioPromedio * 1.2m,
                        diferenciaPorcentaje = Math.Round(((r.Precio - precioPromedio) / precioPromedio) * 100, 1),
                        estado = r.EstadoCompetitividad
                    }).OrderBy(r => r.precio),
                    debug = new {
                        timestamp = DateTime.UtcNow,
                        queryOriginal = query,
                        resultadosProcesados = resultados.Count,
                        tiempoRespuesta = DateTime.UtcNow
                    }
                };

                _logger.LogInformation("Respuesta enviada exitosamente con {Count} productos", resultados.Count);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico al buscar productos para query: '{Query}'", query);
                return StatusCode(500, new { 
                    error = "Error interno al buscar productos",
                    mensaje = "Intente nuevamente en unos momentos",
                    query = query,
                    timestamp = DateTime.UtcNow,
                    detalles = ex.Message
                });
            }
        }

        /// <summary>
        /// Compara un producto específico con productos similares en MercadoLibre
        /// </summary>
        [HttpGet("comparar")]
        [AllowAnonymous]
        public async Task<IActionResult> CompararPrecio(
            [FromQuery] string nombreProducto, 
            [FromQuery] decimal precioFerremas)
        {
            try
            {
                _logger.LogInformation("=== INICIANDO COMPARACIÓN DE PRECIOS ===");
                _logger.LogInformation("Producto: '{Producto}' - Precio Ferremas: ${Precio}", 
                    nombreProducto, precioFerremas);

                if (string.IsNullOrWhiteSpace(nombreProducto))
                {
                    return BadRequest(new { 
                        error = "Debe proporcionar el nombre del producto",
                        ejemplo = "?nombreProducto=taladro percutor&precioFerremas=150000"
                    });
                }

                if (precioFerremas <= 0)
                {
                    return BadRequest(new { 
                        error = "El precio de Ferremas debe ser mayor a 0",
                        precioRecibido = precioFerremas
                    });
                }

                var resultados = await _mlService.BuscarProductoAsync(nombreProducto);
                
                if (!resultados.Any())
                {
                    return Ok(new {
                        mensaje = "No se encontraron productos similares para comparar",
                        nombreProducto = nombreProducto,
                        precioFerremas = precioFerremas,
                        precioFerremasFormateado = $"${precioFerremas:N0} CLP",
                        competencia = new object[0],
                        recomendacion = "No hay datos de competencia disponibles. Intente con términos de búsqueda más simples.",
                        debug = new {
                            timestamp = DateTime.UtcNow,
                            busquedaRealizada = true,
                            resultadosEncontrados = 0
                        }
                    });
                }

                // Análisis de competitividad
                var preciosCompetencia = resultados.Select(r => r.Precio).ToList();
                var precioMinimoCompetencia = preciosCompetencia.Min();
                var precioMaximoCompetencia = preciosCompetencia.Max();
                var precioPromedioCompetencia = preciosCompetencia.Average();

                // Determinar posición competitiva
                var posicion = "";
                var recomendacion = "";
                var alertLevel = "info";
                
                if (precioFerremas <= precioMinimoCompetencia)
                {
                    posicion = "🥇 El más económico del mercado";
                    recomendacion = "Excelente posición competitiva. Mantener precio o considerar incremento moderado del 5-10%.";
                    alertLevel = "success";
                }
                else if (precioFerremas <= precioPromedioCompetencia)
                {
                    posicion = "💰 Por debajo del promedio";
                    recomendacion = "Precio competitivo y atractivo para los clientes.";
                    alertLevel = "success";
                }
                else if (precioFerremas <= precioPromedioCompetencia * 1.2m)
                {
                    posicion = "📊 Ligeramente por encima del promedio";
                    recomendacion = "Precio aceptable, pero considerar reducción del 5-15% para ser más competitivo.";
                    alertLevel = "warning";
                }
                else
                {
                    posicion = "⚠️ Significativamente más caro";
                    recomendacion = "Precio alto comparado con la competencia. Evaluar reducción del 15-25% para mejorar competitividad.";
                    alertLevel = "danger";
                }

                var response = new {
                    nombreProducto = nombreProducto,
                    precioFerremas = precioFerremas,
                    precioFerremasFormateado = $"${precioFerremas:N0} CLP",
                    analisis = new {
                        posicion = posicion,
                        recomendacion = recomendacion,
                        alertLevel = alertLevel,
                        diferenciaMinimoCompetencia = precioFerremas - precioMinimoCompetencia,
                        diferenciaPromedioCompetencia = precioFerremas - precioPromedioCompetencia,
                        porcentajeDiferenciaPromedio = Math.Round(((precioFerremas - precioPromedioCompetencia) / precioPromedioCompetencia) * 100, 1),
                        precioSugeridoMin = Math.Round(precioMinimoCompetencia * 0.95m, 0),
                        precioSugeridoMax = Math.Round(precioPromedioCompetencia * 1.05m, 0)
                    },
                    estadisticasCompetencia = new {
                        precioMinimo = precioMinimoCompetencia,
                        precioMinimoFormateado = $"${precioMinimoCompetencia:N0} CLP",
                        precioMaximo = precioMaximoCompetencia,
                        precioMaximoFormateado = $"${precioMaximoCompetencia:N0} CLP",
                        precioPromedio = Math.Round(precioPromedioCompetencia, 0),
                        precioPromedioFormateado = $"${precioPromedioCompetencia:N0} CLP",
                        totalProductos = resultados.Count,
                        rangoPrecios = precioMaximoCompetencia - precioMinimoCompetencia
                    },
                    competencia = resultados.Take(8).Select(r => new {
                        producto = r.NombreProducto,
                        precio = r.Precio,
                        precioFormateado = $"${r.Precio:N0} CLP",
                        diferencia = r.Precio - precioFerremas,
                        diferenciaFormateada = $"{(r.Precio > precioFerremas ? "+" : "")}{r.Precio - precioFerremas:N0}",
                        porcentajeDiferencia = Math.Round(((r.Precio - precioFerremas) / precioFerremas) * 100, 1),
                        vendedor = r.Vendedor,
                        reputacion = r.ReputacionVendedor,
                        enlace = r.Enlace,
                        envioGratis = r.EnvioGratis,
                        condicion = r.Condicion
                    }).OrderBy(r => r.precio),
                    debug = new {
                        timestamp = DateTime.UtcNow,
                        resultadosAnalizados = resultados.Count,
                        queryUtilizada = nombreProducto
                    }
                };

                _logger.LogInformation("Comparación completada. Posición: {Posicion}", posicion);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al comparar precios para: '{Producto}'", nombreProducto);
                return StatusCode(500, new { 
                    error = "Error interno al comparar precios",
                    nombreProducto = nombreProducto,
                    timestamp = DateTime.UtcNow,
                    detalles = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene el detalle de un producto específico de MercadoLibre
        /// </summary>
        [HttpGet("detalle/{itemId}")]
        [AllowAnonymous]
        public async Task<IActionResult> ObtenerDetalle(string itemId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(itemId))
                {
                    return BadRequest(new { 
                        error = "ID del producto requerido",
                        ejemplo = "/detalle/MLC123456789"
                    });
                }

                _logger.LogInformation("Obteniendo detalle para producto: {ItemId}", itemId);

                var detalle = await _mlService.ObtenerDetalleProducto(itemId);
                
                if (string.IsNullOrEmpty(detalle))
                {
                    return NotFound(new { 
                        error = "Producto no encontrado",
                        itemId = itemId,
                        sugerencia = "Verifique que el ID del producto sea correcto"
                    });
                }

                return Ok(new { 
                    itemId = itemId,
                    detalle = detalle,
                    timestamp = DateTime.UtcNow,
                    mensaje = "Detalle obtenido exitosamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalle del producto: {ItemId}", itemId);
                return StatusCode(500, new { 
                    error = "Error interno al obtener detalle del producto",
                    itemId = itemId,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Endpoint para búsquedas populares y sugerencias
        /// </summary>
        [HttpGet("sugerencias")]
        [AllowAnonymous]
        public IActionResult ObtenerSugerencias()
        {
            var sugerencias = new
            {
                herramientas = new[] {
                    "taladro", "martillo", "destornillador", "sierra", "llave inglesa",
                    "alicate", "nivel", "metro", "escalera", "soldadora"
                },
                marcas_populares = new[] {
                    "makita", "bosch", "dewalt", "stanley", "black decker",
                    "truper", "milwaukee", "ryobi", "hitachi", "craftsman"
                },
                categorias = new[] {
                    "herramientas electricas", "herramientas manuales", "ferreteria",
                    "construccion", "jardineria", "automotriz", "plomeria", "electricidad"
                },
                ejemplos_busqueda = new[] {
                    "taladro makita",
                    "martillo stanley",
                    "sierra circular bosch",
                    "destornillador dewalt",
                    "nivel laser"
                }
            };

            return Ok(new {
                mensaje = "Sugerencias de búsqueda para MercadoLibre",
                sugerencias = sugerencias,
                timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Endpoint para obtener estadísticas de uso
        /// </summary>
        [HttpGet("estadisticas")]
        [Authorize(Policy = "RequireAdministrador")]
        public IActionResult ObtenerEstadisticas()
        {
            try
            {
                var stats = new
                {
                    servicio = "MercadoLibre API Integration",
                    version = "1.0.0",
                    endpoints_disponibles = new[] {
                        "/test - Test de conexión",
                        "/buscar?query=... - Búsqueda de productos",
                        "/comparar?nombreProducto=...&precioFerremas=... - Comparación de precios",
                        "/detalle/{itemId} - Detalle de producto específico",
                        "/sugerencias - Sugerencias de búsqueda"
                    },
                    configuracion = new {
                        site_id = "MLC", // Chile
                        base_url = "https://api.mercadolibre.com/",
                        timeout = "30 segundos",
                        reintentos = 3,
                        limite_resultados = 20
                    },
                    estado = "Operativo",
                    timestamp = DateTime.UtcNow
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas");
                return StatusCode(500, new { 
                    error = "Error al obtener estadísticas",
                    timestamp = DateTime.UtcNow 
                });
            }
        }
    }
}