using Ferremas.Api.DTOs;
using Ferremas.Api.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Ferremas.Api.Services
{
    public class FakePaymentService
    {
        private readonly ILogger<FakePaymentService> _logger;

        public FakePaymentService(ILogger<FakePaymentService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Crea una preferencia de pago FAKE que siempre funciona
        /// </summary>
        public async Task<FakePaymentPreferenceResponse> CrearPreferenciaPago(PagoCreateDTO pagoDto)
        {
            await Task.Delay(500); // Simular demora de API real

            var preferenceId = $"FAKE_{DateTime.Now.Ticks}";
            var checkoutUrl = $"http://localhost:5200/api/pagos/fake-checkout/{preferenceId}?monto={pagoDto.Monto}&pedido={pagoDto.PedidoId}";

            _logger.LogInformation("🎭 PAGO FAKE CREADO");
            _logger.LogInformation("💰 Monto: ${Monto:N0}", pagoDto.Monto);
            _logger.LogInformation("🆔 Pedido: {PedidoId}", pagoDto.PedidoId);
            _logger.LogInformation("🔗 URL: {Url}", checkoutUrl);

            return new FakePaymentPreferenceResponse
            {
                Id = preferenceId,
                InitPoint = checkoutUrl,
                SandboxInitPoint = checkoutUrl
            };
        }

        /// <summary>
        /// Procesa un pago FAKE - siempre aprobado
        /// </summary>
        public async Task<FakePaymentResponse> ProcesarPago(string preferenceId, decimal monto, int pedidoId)
        {
            await Task.Delay(1000); // Simular procesamiento

            var paymentId = new Random().Next(100000, 999999);
            
            _logger.LogInformation("✅ PAGO FAKE PROCESADO");
            _logger.LogInformation("🆔 Payment ID: {PaymentId}", paymentId);
            _logger.LogInformation("📦 Pedido: {PedidoId}", pedidoId);
            _logger.LogInformation("💳 Estado: APROBADO");

            return new FakePaymentResponse
            {
                Id = paymentId,
                Status = "approved",
                TransactionAmount = monto,
                ExternalReference = pedidoId.ToString(),
                DateCreated = DateTime.UtcNow,
                PaymentMethodId = "visa",
                Description = $"Pago FAKE del pedido #{pedidoId}"
            };
        }
    }

    // Modelos para el sistema FAKE
    public class FakePaymentPreferenceResponse
    {
        public string Id { get; set; } = string.Empty;
        public string InitPoint { get; set; } = string.Empty;
        public string SandboxInitPoint { get; set; } = string.Empty;
    }

    public class FakePaymentResponse
    {
        public long Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TransactionAmount { get; set; }
        public string ExternalReference { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
        public string PaymentMethodId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
} 