using Microsoft.AspNetCore.Mvc;
using Ferremas.Api.Services;
using Ferremas.Api.Services.Interfaces;
using System.Text.Json;
using System;
using Ferremas.Api.DTOs;

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

            var detalles = pago.Pedido.Detalles;
            decimal subtotalBruto = detalles.Sum(d => (decimal?)(d.Subtotal ?? ((d.PrecioUnitario ?? 0) * (d.Cantidad ?? 0)))) ?? 0m;
            decimal subtotalNeto = Math.Round(subtotalBruto / 1.19m, 0);
            decimal iva = subtotalBruto - subtotalNeto;
            decimal descuentoBase = 0m;
            decimal descuentoCupon = 0m;
            decimal envio = 0m;
            decimal totalFinal = pago.Pedido.Total ?? 0m;
            // Buscar si hay observaciones con desglose de descuentos
            foreach (var d in detalles)
            {
                if (!string.IsNullOrEmpty(d.Observaciones))
                {
                    var parts = d.Observaciones.Split(',');
                    foreach (var part in parts)
                    {
                        if (part.StartsWith("descuentoBase:"))
                            decimal.TryParse(part.Replace("descuentoBase:", ""), out descuentoBase);
                        if (part.StartsWith("descuentoCupon:"))
                            decimal.TryParse(part.Replace("descuentoCupon:", ""), out descuentoCupon);
                    }
                }
            }
            // Mapear productos
            var productos = detalles.Select(d => {
                decimal precioOriginal = (decimal)(d.PrecioUnitario ?? 0);
                decimal precioConDescuento = 0;
                if (!string.IsNullOrEmpty(d.Observaciones))
                {
                    var parts = d.Observaciones.Split(',');
                    foreach (var part in parts)
                    {
                        if (part.StartsWith("precioOriginal:"))
                            decimal.TryParse(part.Replace("precioOriginal:", ""), out precioOriginal);
                        if (part.StartsWith("precioConDescuento:"))
                            decimal.TryParse(part.Replace("precioConDescuento:", ""), out precioConDescuento);
                    }
                }
                return new ProductoResumenDTO {
                    Nombre = d.Producto?.Nombre ?? "Producto",
                    Cantidad = d.Cantidad ?? 1,
                    Precio = d.PrecioUnitario ?? 0,
                    PrecioOriginal = precioOriginal,
                    PrecioConDescuento = precioConDescuento
                };
            }).ToList();
            var pedido = new CheckoutResponseDTO {
                PedidoId = pago.Pedido.Id,
                NumeroPedido = pago.Pedido.Id.ToString(),
                Total = subtotalBruto,
                Estado = pago.Pedido.Estado,
                FechaCreacion = pago.Pedido.FechaCreacion,
                UrlPago = null,
                CodigoPago = null,
                Productos = productos,
                Subtotal = subtotalNeto,
                DescuentoBase = descuentoBase,
                DescuentoCupon = descuentoCupon,
                Impuestos = iva,
                Envio = envio,
                TotalFinal = totalFinal
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