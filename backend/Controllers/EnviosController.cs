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

        public EnviosController(IEnvioService envioService, WhatsAppWebService whatsAppService, ChilexpressCotizadorService chilexpressService)
        {
            _envioService = envioService;
            _whatsAppService = whatsAppService;
            _chilexpressService = chilexpressService;
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
    }
}