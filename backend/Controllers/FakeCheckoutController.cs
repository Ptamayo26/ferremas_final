using Microsoft.AspNetCore.Mvc;
using Ferremas.Api.Services;
using Ferremas.Api.Data;
using Ferremas.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Ferremas.Api.Controllers
{
    [ApiController]
    [Route("api/pagos")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class FakeCheckoutController : ControllerBase
    {
        private readonly FakePaymentService _fakePaymentService;
        private readonly AppDbContext _context;
        private readonly ILogger<FakeCheckoutController> _logger;

        public FakeCheckoutController(
            FakePaymentService fakePaymentService,
            AppDbContext context,
            ILogger<FakeCheckoutController> logger)
        {
            _fakePaymentService = fakePaymentService;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Checkout FAKE - Página HTML que simula MercadoPago
        /// </summary>
        [HttpGet("fake-checkout/{preferenceId}")]
        public IActionResult FakeCheckout(string preferenceId, [FromQuery] decimal monto, [FromQuery] int pedido)
        {
            var html = $@"
<!DOCTYPE html>
<html>
<head>
    <title>Ferremas - Pago Simulado</title>
    <style>
        body {{ font-family: Arial, sans-serif; background: #f5f5f5; margin: 0; padding: 20px; }}
        .container {{ max-width: 500px; margin: 0 auto; background: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        .logo {{ text-align: center; margin-bottom: 30px; }}
        .amount {{ font-size: 24px; font-weight: bold; color: #009ee3; text-align: center; margin: 20px 0; }}
        .form-group {{ margin-bottom: 20px; }}
        label {{ display: block; margin-bottom: 5px; font-weight: bold; }}
        input {{ width: 100%; padding: 12px; border: 1px solid #ddd; border-radius: 5px; font-size: 16px; }}
        .btn {{ background: #009ee3; color: white; padding: 15px 30px; border: none; border-radius: 5px; font-size: 16px; cursor: pointer; width: 100%; }}
        .btn:hover {{ background: #0078a3; }}
        .success {{ background: #d4edda; border: 1px solid #c3e6cb; color: #155724; padding: 15px; border-radius: 5px; margin: 20px 0; }}
        .loading {{ text-align: center; display: none; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='logo'>
            <h2>🔧 Ferremas - Pago Simulado</h2>
            <p>Pedido #{pedido}</p>
        </div>
        
        <div class='amount'>Total: ${monto:N0} CLP</div>
        
        <form id='paymentForm'>
            <div class='form-group'>
                <label>Número de Tarjeta</label>
                <input type='text' value='4111 1111 1111 1111' readonly style='background: #f8f9fa;'>
            </div>
            
            <div class='form-group'>
                <label>Titular</label>
                <input type='text' value='CLIENTE PRUEBA' readonly style='background: #f8f9fa;'>
            </div>
            
            <div style='display: flex; gap: 15px;'>
                <div class='form-group' style='flex: 1;'>
                    <label>Vencimiento</label>
                    <input type='text' value='12/25' readonly style='background: #f8f9fa;'>
                </div>
                <div class='form-group' style='flex: 1;'>
                    <label>CVV</label>
                    <input type='text' value='123' readonly style='background: #f8f9fa;'>
                </div>
            </div>
            
            <button type='submit' class='btn'>💳 PAGAR AHORA</button>
        </form>
        
        <div id='loading' class='loading'>
            <p>🔄 Procesando pago...</p>
        </div>
        
        <div id='success' class='success' style='display: none;'>
            <h3>✅ ¡Pago Exitoso!</h3>
            <p>Tu pago ha sido procesado correctamente.</p>
            <p><strong>ID de Transacción:</strong> <span id='transactionId'></span></p>
        </div>
    </div>

    <script>
        document.getElementById('paymentForm').addEventListener('submit', async function(e) {{
            e.preventDefault();
            
            // Mostrar loading
            document.getElementById('paymentForm').style.display = 'none';
            document.getElementById('loading').style.display = 'block';
            
            // Simular procesamiento
            setTimeout(async () => {{
                try {{
                    const response = await fetch('/api/pagos/fake-process', {{
                        method: 'POST',
                        headers: {{ 'Content-Type': 'application/json' }},
                        body: JSON.stringify({{
                            preferenceId: '{preferenceId}',
                            monto: {monto},
                            pedidoId: {pedido}
                        }})
                    }});
                    
                    const result = await response.json();
                    
                    if (result.success) {{
                        document.getElementById('loading').style.display = 'none';
                        document.getElementById('success').style.display = 'block';
                        document.getElementById('transactionId').textContent = result.transactionId;
                        
                        // Redirigir después de 3 segundos
                        setTimeout(() => {{
                            window.location.href = '/api/pagos/fake-success?pedido={pedido}&transaction=' + result.transactionId;
                        }}, 3000);
                    }}
                }} catch (error) {{
                    alert('Error al procesar el pago');
                }}
            }}, 2000);
        }});
    </script>
</body>
</html>";

            return Content(html, "text/html");
        }

        /// <summary>
        /// Procesar pago FAKE
        /// </summary>
        [HttpPost("fake-process")]
        public async Task<IActionResult> ProcessFakePayment([FromBody] FakePaymentRequest request)
        {
            try
            {
                // Procesar pago FAKE
                var paymentResponse = await _fakePaymentService.ProcesarPago(
                    request.PreferenceId, 
                    request.Monto, 
                    request.PedidoId);

                // Guardar en base de datos
                var pago = new Pago
                {
                    PedidoId = request.PedidoId,
                    Monto = request.Monto,
                    MetodoPago = "FAKE_PAYMENT",
                    TransaccionId = paymentResponse.Id.ToString(),
                    Estado = "approved",
                    FechaPago = DateTime.UtcNow,
                    DatosRespuesta = System.Text.Json.JsonSerializer.Serialize(paymentResponse)
                };

                _context.Pagos.Add(pago);
                await _context.SaveChangesAsync();

                // Actualizar estado del pedido
                var pedido = await _context.Pedidos.FindAsync(request.PedidoId);
                if (pedido != null)
                {
                    pedido.Estado = "Pagado";
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation("✅ PAGO FAKE COMPLETADO - Pedido: {PedidoId}, Transacción: {TransactionId}", 
                    request.PedidoId, paymentResponse.Id);

                return Ok(new { success = true, transactionId = paymentResponse.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando pago fake");
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Página de éxito
        /// </summary>
        [HttpGet("fake-success")]
        public IActionResult FakeSuccess([FromQuery] int pedido, [FromQuery] string transaction)
        {
            var html = $@"
<!DOCTYPE html>
<html>
<head>
    <title>Pago Exitoso - Ferremas</title>
    <style>
        body {{ font-family: Arial, sans-serif; background: #f5f5f5; margin: 0; padding: 20px; text-align: center; }}
        .container {{ max-width: 500px; margin: 50px auto; background: white; padding: 40px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        .success-icon {{ font-size: 64px; color: #28a745; margin-bottom: 20px; }}
        h1 {{ color: #28a745; margin-bottom: 30px; }}
        .details {{ background: #f8f9fa; padding: 20px; border-radius: 5px; margin: 20px 0; text-align: left; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='success-icon'>✅</div>
        <h1>¡Pago Exitoso!</h1>
        <p>Tu compra ha sido procesada correctamente.</p>
        
        <div class='details'>
            <p><strong>Pedido:</strong> #{pedido}</p>
            <p><strong>Transacción:</strong> {transaction}</p>
            <p><strong>Estado:</strong> APROBADO</p>
            <p><strong>Fecha:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</p>
        </div>
        
        <p>Gracias por tu compra en Ferremas 🔧</p>
    </div>
</body>
</html>";

            return Content(html, "text/html");
        }
    }

    public class FakePaymentRequest
    {
        public string PreferenceId { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public int PedidoId { get; set; }
    }
} 