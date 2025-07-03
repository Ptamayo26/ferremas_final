using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ferremas.Api.Services;
using System.Threading.Tasks;

namespace Ferremas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class ChileGeoController : ControllerBase
    {
        private readonly ChileGeoService _chileGeoService;

        public ChileGeoController(ChileGeoService chileGeoService)
        {
            _chileGeoService = chileGeoService;
        }

        /// <summary>
        /// Obtiene todas las regiones de Chile
        /// </summary>
        [HttpGet("regiones")]
        public async Task<IActionResult> GetRegiones()
        {
            try
            {
                var regiones = await _chileGeoService.GetRegionesAsync();
                return Ok(new
                {
                    exito = true,
                    mensaje = $"Se obtuvieron {regiones.Count} regiones de Chile",
                    data = regiones
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Error al obtener las regiones de Chile",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene las comunas de una región específica
        /// </summary>
        [HttpGet("regiones/{codigoRegion}/comunas")]
        public async Task<IActionResult> GetComunasByRegion(string codigoRegion)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(codigoRegion))
                {
                    return BadRequest(new
                    {
                        exito = false,
                        mensaje = "El código de región es requerido"
                    });
                }

                var comunas = await _chileGeoService.GetComunasByRegionAsync(codigoRegion);
                return Ok(new
                {
                    exito = true,
                    mensaje = $"Se obtuvieron {comunas.Count} comunas para la región {codigoRegion}",
                    data = comunas
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Error al obtener las comunas de la región",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene todas las comunas de Chile
        /// </summary>
        [HttpGet("comunas")]
        public async Task<IActionResult> GetTodasLasComunas()
        {
            try
            {
                var comunas = await _chileGeoService.GetTodasLasComunasAsync();
                return Ok(new
                {
                    exito = true,
                    mensaje = $"Se obtuvieron {comunas.Count} comunas de Chile",
                    data = comunas
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Error al obtener todas las comunas de Chile",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Busca comunas por nombre (búsqueda parcial)
        /// </summary>
        [HttpGet("comunas/buscar")]
        public async Task<IActionResult> BuscarComunas([FromQuery] string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return BadRequest(new
                    {
                        exito = false,
                        mensaje = "El parámetro 'query' es requerido"
                    });
                }

                var comunas = await _chileGeoService.BuscarComunasAsync(query);
                return Ok(new
                {
                    exito = true,
                    mensaje = $"Se encontraron {comunas.Count} comunas para la búsqueda '{query}'",
                    data = comunas
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Error al buscar comunas",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Información sobre la API de geografía de Chile
        /// </summary>
        [HttpGet("info")]
        public IActionResult GetInfo()
        {
            return Ok(new
            {
                nombre = "API de Geografía de Chile",
                version = "1.0.0",
                descripcion = "API para obtener regiones y comunas de Chile usando datos oficiales del gobierno",
                fuente = "API Digital del Gobierno de Chile",
                endpoints = new
                {
                    regiones = "/api/ChileGeo/regiones",
                    comunasPorRegion = "/api/ChileGeo/regiones/{codigo}/comunas",
                    todasLasComunas = "/api/ChileGeo/comunas",
                    buscarComunas = "/api/ChileGeo/comunas/buscar?query={nombre}"
                },
                documentacion = "https://apis.digital.gob.cl/dpa/",
                estado = "Operativo",
                ultimaActualizacion = DateTime.UtcNow
            });
        }
    }
} 