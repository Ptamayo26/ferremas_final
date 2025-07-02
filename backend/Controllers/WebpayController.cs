using Microsoft.AspNetCore.Mvc;
using Ferremas.Api.Services;
using Ferremas.Api.Services.Interfaces;
using System.Text.Json;
using System;

namespace Ferremas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebpayController : ControllerBase
    {
        private readonly WebpayService _webpayService;
        private readonly IPagosService _pagosService;
        public WebpayController(WebpayService webpayService, IPagosService pagosService)
        {
            _webpayService = webpayService;
            _pagosService = pagosService;
        }

        [HttpPost("crear-transaccion")]
        public async Task<IActionResult> CrearTransaccion([FromBody] CrearTransaccionRequest request)
        {
            var response = _webpayService.CrearTransaccion(request.Amount, request.BuyOrder, request.SessionId, request.ReturnUrl);

            // Guardar el pago en la base de datos con el token real de Webpay
            string buyOrder = request.BuyOrder;
            int pedidoId = int.Parse(buyOrder.Replace("ORD-", "")); // Extrae el número del formato 'ORD-<número>'
            var pagoDto = new Ferremas.Api.DTOs.PagoCreateDTO
            {
                PedidoId = pedidoId,
                Monto = request.Amount,
                MetodoPago = "WEBPAY",
                TokenPasarela = response.Token,
                TransaccionId = response.Token, // Puedes guardar el mismo token como transacción si no tienes otro dato
                DatosRespuesta = JsonSerializer.Serialize(response)
            };
            await _pagosService.Create(pagoDto);

            return Ok(response);
        }

        [HttpPost("confirmar-transaccion")]
        public async Task<IActionResult> ConfirmarTransaccion([FromBody] ConfirmarTransaccionRequest request)
        {
            // Log del token recibido
            Console.WriteLine($"[Webpay] Token recibido en confirmación: {request.Token}");
            try
            {
                // Intentar confirmar con Transbank
                var response = _webpayService.ConfirmarTransaccion(request.Token);
            }
            catch (Exception ex)
            {
                // Si Transbank devuelve error de transacción ya procesada, continuamos
                if (ex.Message.Contains("Transaction already locked") || ex.Message.Contains("already processed"))
                {
                    // Continuar con la búsqueda en BD
                }
                else
                {
                    // Para otros errores, devolver el error
                    return BadRequest(new { success = false, message = "Error al confirmar transacción: " + ex.Message });
                }
            }

            // Buscar el pago y el pedido asociado
            var pago = await _pagosService.GetByTokenPasarela(request.Token);
            if (pago == null || pago.Pedido == null)
            {
                Console.WriteLine($"[Webpay] No se encontró pago para el token: {request.Token}");
                return Ok(new { success = false, message = "No se encontró el pedido asociado al pago." });
            }

            // Mapear los datos relevantes para el frontend
            var detalles = pago.Pedido.Detalles;
            decimal subtotalBruto = detalles.Sum(d => d.Subtotal ?? ((d.PrecioUnitario ?? 0) * (d.Cantidad ?? 0)));
            decimal subtotalNeto = Math.Round(subtotalBruto / 1.19m, 0);
            decimal iva = subtotalBruto - subtotalNeto;
            decimal descuento = 0m; // Si tienes lógica de descuentos, cámbiala aquí
            decimal envio = 0m; // Si tienes lógica de envío, cámbiala aquí
            var pedido = new {
                numeroPedido = pago.Pedido.Id.ToString(),
                total = subtotalBruto, // El total es el valor con IVA
                estado = pago.Pedido.Estado,
                fechaCreacion = pago.Pedido.FechaCreacion,
                tiempoEnvio = "2-4 días hábiles", // Simulado
                urlBoleta = "/descargas/boleta" + pago.Pedido.Id + ".pdf", // Simulado
                urlFactura = "/descargas/factura" + pago.Pedido.Id + ".pdf", // Simulado
                productos = detalles.Select(d => new {
                    nombre = d.Producto?.Nombre ?? "Producto",
                    cantidad = d.Cantidad,
                    precio = d.PrecioUnitario
                }),
                subtotal = subtotalNeto,
                descuento,
                impuestos = iva,
                envio
            };
            return Ok(new { success = true, pedido });
        }
    }

    public class CrearTransaccionRequest
    {
        public decimal Amount { get; set; }
        public string BuyOrder { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
    }

    public class ConfirmarTransaccionRequest
    {
        public string Token { get; set; } = string.Empty;
    }
} 