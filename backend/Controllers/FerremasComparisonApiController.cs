using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Ferremas.Api.Services;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Ferremas.Api.Models;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace Ferremas.Api.Controllers
{
    /// <summary>
    /// API Pública de Ferremas - Servicio de Comparación de Precios
    /// Permite a desarrolladores externos acceder a nuestro servicio de comparación de precios
    /// </summary>
    [ApiController]
    [Route("api/v1/ferremas-comparison")]
    public class FerremasComparisonApiController : ControllerBase
    {
        private readonly MercadoLibreService _mlService;
        private readonly ILogger<FerremasComparisonApiController> _logger;

        public FerremasComparisonApiController(
            MercadoLibreService mlService, 
            ILogger<FerremasComparisonApiController> logger)
        {
            _mlService = mlService;
            _logger = logger;
        }

        /// <summary>
        /// API Pública: Buscar productos y precios en el mercado chileno
        /// </summary>
        /// <param name="query">Término de búsqueda del producto</param>
        /// <param name="apiKey">Clave de API (opcional para uso básico)</param>
        /// <returns>Lista de productos con precios comparativos</returns>
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchProducts(
            [FromQuery][Required] string query,
            [FromHeader(Name = "X-Ferremas-API-Key")] string? apiKey = null)
        {
            try
            {
                // Validar entrada
                if (string.IsNullOrWhiteSpace(query))
                {
                    return BadRequest(new FerremasApiResponse
                    {
                        Success = false,
                        Message = "El parámetro 'query' es requerido",
                        Error = new { Code = "MISSING_QUERY", Description = "Debe proporcionar un término de búsqueda" }
                    });
                }

                // Logging para analytics
                _logger.LogInformation("API Pública - Búsqueda: '{Query}' desde IP: {IP}", 
                    query, HttpContext.Connection.RemoteIpAddress);

                // Buscar productos
                var productos = await _mlService.BuscarProductoAsync(query);

                // Transformar a formato API público
                var response = new FerremasApiResponse
                {
                    Success = true,
                    Message = $"Se encontraron {productos.Count} productos",
                    Data = new
                    {
                        Query = query,
                        TotalResults = productos.Count,
                        Timestamp = DateTime.UtcNow,
                        Products = productos.Select(p => new
                        {
                            // Información básica del producto
                            Name = p.NombreProducto,
                            Price = new
                            {
                                Amount = p.Precio,
                                Currency = "CLP",
                                Formatted = $"${p.Precio:N0} CLP"
                            },
                            
                            // Información del vendedor
                            Seller = new
                            {
                                Name = p.Vendedor,
                                Reputation = p.ReputacionVendedor,
                                Sales = p.CantidadVentas
                            },
                            
                            // Detalles del producto
                            Condition = p.Condicion,
                            FreeShipping = p.EnvioGratis,
                            Location = p.Ubicacion,
                            ImageUrl = p.Imagen,
                            ProductUrl = p.Enlace,
                            
                            // Metadatos
                            Source = p.Tienda,
                            LastUpdated = p.FechaConsulta
                        }).OrderBy(p => p.Price.Amount)
                    },
                    Meta = new
                    {
                        ApiVersion = "1.0",
                        Provider = "Ferremas Chile",
                        RateLimit = GetRateLimitInfo(),
                        Documentation = "https://api.ferremas.cl/docs"
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en API pública para query: {Query}", query);
                
                return StatusCode(500, new FerremasApiResponse
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Error = new { Code = "INTERNAL_ERROR", Description = "Intente nuevamente más tarde" }
                });
            }
        }

        /// <summary>
        /// API Pública: Análisis de competitividad de precios
        /// </summary>
        /// <param name="productName">Nombre del producto a analizar</param>
        /// <param name="currentPrice">Precio actual del producto</param>
        /// <param name="apiKey">Clave de API</param>
        [HttpGet("price-analysis")]
        [AllowAnonymous]
        public async Task<IActionResult> PriceAnalysis(
            [FromQuery][Required] string productName,
            [FromQuery][Required] decimal currentPrice,
            [FromHeader(Name = "X-Ferremas-API-Key")] string? apiKey = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productName))
                {
                    return BadRequest(new FerremasApiResponse
                    {
                        Success = false,
                        Message = "El parámetro 'productName' es requerido"
                    });
                }

                if (currentPrice <= 0)
                {
                    return BadRequest(new FerremasApiResponse
                    {
                        Success = false,
                        Message = "El parámetro 'currentPrice' debe ser mayor a 0"
                    });
                }

                _logger.LogInformation("API Pública - Análisis de precio: '{Product}' a ${Price}", 
                    productName, currentPrice);

                var competencia = await _mlService.BuscarProductoAsync(productName);
                
                if (!competencia.Any())
                {
                    return Ok(new FerremasApiResponse
                    {
                        Success = true,
                        Message = "No se encontraron productos competidores",
                        Data = new
                        {
                            ProductName = productName,
                            CurrentPrice = currentPrice,
                            Analysis = new
                            {
                                Position = "No determinada",
                                Recommendation = "Sin datos de competencia disponibles",
                                CompetitorCount = 0
                            }
                        }
                    });
                }

                // Análisis de competitividad
                var precios = competencia.Select(c => c.Precio).ToList();
                var minPrice = precios.Min();
                var maxPrice = precios.Max();
                var avgPrice = precios.Average();

                var position = DetermineMarketPosition(currentPrice, minPrice, avgPrice, maxPrice);
                var recommendation = GenerateRecommendation(currentPrice, avgPrice);

                var response = new FerremasApiResponse
                {
                    Success = true,
                    Message = "Análisis de competitividad completado",
                    Data = new
                    {
                        Product = new
                        {
                            Name = productName,
                            CurrentPrice = new
                            {
                                Amount = currentPrice,
                                Currency = "CLP",
                                Formatted = $"${currentPrice:N0} CLP"
                            }
                        },
                        MarketAnalysis = new
                        {
                            Position = position.Description,
                            PositionLevel = position.Level, // 1-5 (1=más caro, 5=más barato)
                            Recommendation = recommendation.Message,
                            RecommendationType = recommendation.Type,
                            
                            // Estadísticas del mercado
                            Market = new
                            {
                                CompetitorCount = competencia.Count,
                                PriceRange = new
                                {
                                    Min = new { Amount = minPrice, Formatted = $"${minPrice:N0} CLP" },
                                    Max = new { Amount = maxPrice, Formatted = $"${maxPrice:N0} CLP" },
                                    Average = new { Amount = Math.Round(avgPrice, 0), Formatted = $"${avgPrice:N0} CLP" }
                                },
                                YourPosition = new
                                {
                                    PercentileBelowYou = Math.Round((double)precios.Count(p => p < currentPrice) / precios.Count * 100, 1),
                                    DifferenceFromAverage = new
                                    {
                                        Amount = currentPrice - avgPrice,
                                        Percentage = Math.Round(((currentPrice - avgPrice) / avgPrice) * 100, 1)
                                    }
                                }
                            }
                        },
                        TopCompetitors = competencia.Take(5).Select(c => new
                        {
                            Name = c.NombreProducto,
                            Price = new { Amount = c.Precio, Formatted = $"${c.Precio:N0} CLP" },
                            Seller = c.Vendedor,
                            Reputation = c.ReputacionVendedor
                        }).OrderBy(c => c.Price.Amount)
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en análisis de precios");
                return StatusCode(500, new FerremasApiResponse
                {
                    Success = false,
                    Message = "Error interno del servidor"
                });
            }
        }

        /// <summary>
        /// API Pública: Información sobre la API
        /// </summary>
        [HttpGet("info")]
        [AllowAnonymous]
        public IActionResult ApiInfo()
        {
            return Ok(new
            {
                Name = "Ferremas Price Comparison API",
                Version = "1.0.0",
                Description = "API pública para comparación de precios en el mercado chileno",
                Provider = "Ferremas Chile",
                Contact = new
                {
                    Website = "https://ferremas.cl",
                    Email = "api@ferremas.cl",
                    Support = "https://ferremas.cl/api-support"
                },
                Endpoints = new
                {
                    Search = "/api/v1/ferremas-comparison/search?query={product}",
                    PriceAnalysis = "/api/v1/ferremas-comparison/price-analysis?productName={name}&currentPrice={price}",
                    Documentation = "https://api.ferremas.cl/docs"
                },
                Usage = new
                {
                    RateLimit = "100 requests per hour for anonymous users",
                    Authentication = "Optional API key for increased limits",
                    Pricing = "Free tier available, premium plans for high volume"
                },
                Status = "Operational",
                LastUpdated = DateTime.UtcNow
            });
        }

        // Métodos auxiliares
        private (string Description, int Level) DetermineMarketPosition(decimal currentPrice, decimal minPrice, decimal avgPrice, decimal maxPrice)
        {
            if (currentPrice <= minPrice * 1.05m) // Dentro del 5% del más barato
                return ("El más competitivo del mercado", 5);
            else if (currentPrice <= avgPrice * 0.85m) // 15% bajo el promedio
                return ("Muy competitivo", 4);
            else if (currentPrice <= avgPrice * 1.15m) // 15% sobre el promedio
                return ("Competitivo", 3);
            else if (currentPrice <= avgPrice * 1.3m) // 30% sobre el promedio
                return ("Por encima del promedio", 2);
            else
                return ("Significativamente más caro", 1);
        }

        private (string Message, string Type) GenerateRecommendation(decimal currentPrice, decimal avgPrice)
        {
            var difference = ((currentPrice - avgPrice) / avgPrice) * 100;

            return difference switch
            {
                <= -15 => ("Precio excelente, considerar mantener o incrementar ligeramente", "MAINTAIN"),
                <= 0 => ("Precio competitivo, buen punto para el mercado", "MAINTAIN"),
                <= 15 => ("Precio aceptable, monitorear competencia", "MONITOR"),
                <= 30 => ("Considerar reducción del 5-10% para mejorar competitividad", "REDUCE_MINOR"),
                _ => ("Precio alto, considerar reducción significativa del 15-25%", "REDUCE_MAJOR")
            };
        }

        private object GetRateLimitInfo()
        {
            return new
            {
                Remaining = 95, // Esto vendría del sistema de rate limiting real
                Reset = DateTimeOffset.UtcNow.AddHours(1),
                Limit = 100
            };
        }
    }

    /// <summary>
    /// Formato estándar de respuesta de la API de Ferremas
    /// </summary>
    public class FerremasApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
        public object? Error { get; set; }
        public object? Meta { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}