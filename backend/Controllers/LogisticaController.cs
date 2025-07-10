using Microsoft.AspNetCore.Mvc;
using Ferremas.Api.Services;
using Ferremas.Api.DTOs;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Ferremas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogisticaController : ControllerBase
    {
        private readonly ShipitService _shipitService;
        private readonly ILogger<LogisticaController> _logger;

        public LogisticaController(ShipitService shipitService, ILogger<LogisticaController> logger)
        {
            _shipitService = shipitService;
            _logger = logger;
        }

        /// <summary>
        /// Crea un nuevo envío en Shipit
        /// </summary>
        [HttpPost("crear-envio")]
        public async Task<IActionResult> CrearEnvio([FromBody] ShipitEnvioRequestDto envio)
        {
            try
            {
                _logger.LogInformation($"Solicitud de creación de envío recibida para: {envio.NombreDestinatario}");
                
                var resultado = await _shipitService.CrearEnvioAsync(envio);
                
                if (resultado.Success)
                {
                    _logger.LogInformation($"Envío creado exitosamente. Tracking: {resultado.TrackingNumber}");
                    return Ok(new 
                    { 
                        success = true, 
                        message = "Envío creado exitosamente",
                        data = resultado 
                    });
                }
                else
                {
                    _logger.LogWarning($"Error al crear envío: {resultado.Error}");
                    return BadRequest(new 
                    { 
                        success = false, 
                        message = "Error al crear envío", 
                        error = resultado.Error 
                    });
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Error inesperado al crear envío: {ex.Message}");
                return StatusCode(500, new 
                { 
                    success = false, 
                    message = "Error interno del servidor", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// Obtiene cotización de envío
        /// </summary>
        [HttpPost("cotizar-envio")]
        public async Task<IActionResult> CotizarEnvio([FromBody] ShipitCotizacionRequest request)
        {
            try
            {
                _logger.LogInformation("Solicitud de cotización de envío recibida");
                
                var resultado = await _shipitService.CotizarEnvioAsync(request);
                
                return Ok(new 
                { 
                    success = true, 
                    message = "Cotización obtenida exitosamente",
                    data = resultado 
                });
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Error al cotizar envío: {ex.Message}");
                return StatusCode(500, new 
                { 
                    success = false, 
                    message = "Error al obtener cotización", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// Obtiene la lista de couriers disponibles
        /// </summary>
        [HttpGet("couriers")]
        public async Task<IActionResult> ObtenerCouriers()
        {
            try
            {
                _logger.LogInformation("Solicitud de couriers recibida");
                
                var couriers = await _shipitService.ObtenerCouriersAsync();
                
                return Ok(new 
                { 
                    success = true, 
                    message = "Couriers obtenidos exitosamente",
                    data = couriers 
                });
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Error al obtener couriers: {ex.Message}");
                return StatusCode(500, new 
                { 
                    success = false, 
                    message = "Error al obtener couriers", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// Verifica si el servicio de Shipit está disponible
        /// </summary>
        [HttpGet("status")]
        public async Task<IActionResult> VerificarEstado()
        {
            try
            {
                var disponible = await _shipitService.IsServiceAvailableAsync();
                
                return Ok(new 
                { 
                    success = true, 
                    message = "Estado del servicio verificado",
                    data = new { disponible = disponible } 
                });
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Error al verificar estado: {ex.Message}");
                return StatusCode(500, new 
                { 
                    success = false, 
                    message = "Error al verificar estado", 
                    error = ex.Message 
                });
            }
        }
    }
} 