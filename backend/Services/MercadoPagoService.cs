using System;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ferremas.Api.Models;
using Ferremas.Api.DTOs;

namespace Ferremas.Api.Services
{
    public class MercadoPagoService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MercadoPagoService> _logger;

        public MercadoPagoService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<MercadoPagoService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        // 🎯 MÉTODO SIMULADO - SIEMPRE FUNCIONA
        public async Task<MercadoPagoPreferenceResponse> CrearPreferenciaPago(PagoCreateDTO pagoDto, string emailCliente = null)
        {
            await Task.Delay(100); // Simular llamada a API

            var preferenciaId = Guid.NewGuid().ToString();
            var fakeUrl = $"http://localhost:5200/api/pagos/simular-pago?pedidoId={pagoDto.PedidoId}&monto={pagoDto.Monto}&preferenciaId={preferenciaId}";

            _logger.LogInformation("🎭 PAGO SIMULADO CREADO");
            _logger.LogInformation("💰 Monto: ${Monto:N0} CLP", pagoDto.Monto);
            _logger.LogInformation("🔗 URL Simulada: {Url}", fakeUrl);

            return new MercadoPagoPreferenceResponse
            {
                Id = preferenciaId,
                InitPoint = fakeUrl,
                SandboxInitPoint = fakeUrl,
                ExternalReference = pagoDto.PedidoId.ToString(),
                DateCreated = DateTime.UtcNow
            };
        }

        // 🎯 MÉTODO SIMULADO - SIEMPRE APRUEBA
        public async Task<MercadoPagoResponse> CrearPago(PagoCreateDTO pagoDto)
        {
            await Task.Delay(500); // Simular procesamiento

            var pagoId = new Random().Next(100000000, 999999999);

            _logger.LogInformation("💳 PAGO DIRECTO SIMULADO");
            _logger.LogInformation("💰 Monto: ${Monto:N0}", pagoDto.Monto);
            _logger.LogInformation("✅ Estado: APROBADO");

            return new MercadoPagoResponse
            {
                Id = pagoId,
                Status = "approved",
                StatusDetail = "accredited",
                TransactionAmount = pagoDto.Monto,
                ExternalReference = pagoDto.PedidoId.ToString(),
                Description = $"Pago simulado - Pedido #{pagoDto.PedidoId}",
                DateCreated = DateTime.UtcNow
            };
        }

        // 🎯 MÉTODO SIMULADO - SIEMPRE ENCUENTRA EL PAGO
        public async Task<MercadoPagoResponse> ObtenerPago(string paymentId)
        {
            await Task.Delay(100);

            _logger.LogInformation("🔍 OBTENIENDO PAGO SIMULADO: {PaymentId}", paymentId);

            return new MercadoPagoResponse
            {
                Id = long.Parse(paymentId),
                Status = "approved",
                StatusDetail = "accredited",
                TransactionAmount = 15000, // Monto por defecto
                ExternalReference = "123",
                Description = "Pago simulado encontrado",
                DateCreated = DateTime.UtcNow.AddMinutes(-5)
            };
        }
    }

    // MODELOS SIMPLIFICADOS
    public class MercadoPagoResponse
    {
        public long Id { get; set; }
        public string Status { get; set; } = "approved";
        public string StatusDetail { get; set; } = "accredited";
        public decimal TransactionAmount { get; set; }
        public string? ExternalReference { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}