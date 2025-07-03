using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Ferremas.Api.DTOs;
using Ferremas.Api.Services.Interfaces;
using Ferremas.Api.Services;

namespace Ferremas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnviosController : ControllerBase
    {
        private readonly IEnvioService _envioService;
        private readonly WhatsAppWebService _whatsAppService;
        private readonly ChilexpressCotizadorService _chilexpressService;
        private readonly ShipitService _shipitService;

        public EnviosController(IEnvioService envioService, WhatsAppWebService whatsAppService, 
            ChilexpressCotizadorService chilexpressService, ShipitService shipitService)
        {
            _envioService = envioService;
            _whatsAppService = whatsAppService;
            _chilexpressService = chilexpressService;
            _shipitService = shipitService;
        }

        [HttpGet("{pedidoId}")]
        public async Task<IActionResult> ObtenerEnvioPorPedido(int pedidoId)
        {
            var envio = await _envioService.ObtenerEnvioPorPedidoAsync(pedidoId);
            if (envio == null)
                return NotFound($"No se encontró envío para el pedido {pedidoId}");
            
            return Ok(envio);
        }

        [HttpPost]
        public async Task<IActionResult> CrearEnvio([FromBody] EnvioCreateDTO dto)
        {
            var envio = await _envioService.CrearEnvioAsync(dto);
            
            // Generar link de WhatsApp para notificación
            string numeroCliente = dto.TelefonoContacto;
            string mensaje = $"¡Tu pedido #{dto.PedidoId} ha sido despachado!\n" +
                $"Dirección: {dto.DireccionDestino}\n" +
                $"Comuna: {dto.ComunaDestino}\n" +
                $"Región: {dto.RegionDestino}\n" +
                           "Pronto recibirás tu compra.";
            
            var whatsappUrl = _whatsAppService.GenerarLinkWhatsapp(numeroCliente, mensaje);

            return Ok(new { 
                envio = envio,
                whatsapp_url = whatsappUrl
            });
        }

        [HttpPut("{envioId}/estado")]
        public async Task<IActionResult> ActualizarEstado(int envioId, [FromBody] string nuevoEstado)
        {
            var resultado = await _envioService.ActualizarEstadoEnvioAsync(envioId, nuevoEstado);
            if (!resultado)
                return NotFound($"No se encontró el envío {envioId}");
            
            return Ok(new { mensaje = "Estado actualizado correctamente" });
        }

        [HttpGet("coberturas")]
        public async Task<IActionResult> BuscarCoberturas([FromQuery] string nombre)
        {
            var resultados = await _chilexpressService.BuscarCoberturasAsync(nombre);
            return Ok(resultados);
        }

        [HttpPost("cotizar")]
        public async Task<IActionResult> CotizarEnvio([FromBody] CotizacionRequest request)
        {
            var resultados = await _chilexpressService.CotizarEnvioAsync(request);
            return Ok(resultados);
        }

        // Nuevos endpoints para Shipit
        [HttpGet("shipit/coberturas")]
        public async Task<IActionResult> BuscarCoberturasShipit([FromQuery] string query)
        {
            try
            {
                var resultados = await _shipitService.BuscarCoberturasAsync(query);
                return Ok(resultados);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al buscar coberturas en Shipit: {ex.Message}");
            }
        }

        [HttpPost("shipit/cotizar")]
        public async Task<IActionResult> CotizarEnvioShipit([FromBody] ShipitCotizacionRequest request)
        {
            try
            {
                var resultados = await _shipitService.CotizarEnvioAsync(request);
                return Ok(resultados);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al cotizar envío con Shipit: {ex.Message}");
            }
        }

        [HttpGet("cotizaciones")]
        public async Task<IActionResult> ObtenerCotizaciones([FromQuery] string origen, [FromQuery] string destino, 
            [FromQuery] decimal peso = 1, [FromQuery] decimal alto = 10, [FromQuery] decimal ancho = 10, 
            [FromQuery] decimal largo = 10)
        {
            try
            {
                // Cotización con Shipit
                var requestShipit = new ShipitCotizacionRequest
                {
                    Origen = new ShipitDireccion { Calle = "Av. Providencia", Numero = "123", ComunaId = 13101 }, // Santiago
                    Destino = new ShipitDireccion { Calle = destino, Numero = "456", ComunaId = 13102 }, // Providencia
                    Peso = peso,
                    Alto = alto,
                    Ancho = ancho,
                    Largo = largo,
                    ValorSeguro = 0,
                    Contenido = "Productos Ferremas"
                };

                var cotizacionShipit = await _shipitService.CotizarEnvioAsync(requestShipit);

                // Cotización con Chilexpress
                var requestChilexpress = new CotizacionRequest
                {
                    CodigoCoberturaOrigen = "13101", // Santiago
                    CodigoCoberturaDestino = "13102", // Providencia
                    Peso = (double)peso,
                    Largo = (double)largo,
                    Ancho = (double)ancho,
                    Alto = (double)alto
                };

                var cotizacionChilexpress = await _chilexpressService.CotizarEnvioAsync(requestChilexpress);

                return Ok(new
                {
                    shipit = cotizacionShipit,
                    chilexpress = cotizacionChilexpress
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al obtener cotizaciones: {ex.Message}");
            }
        }
    }
}