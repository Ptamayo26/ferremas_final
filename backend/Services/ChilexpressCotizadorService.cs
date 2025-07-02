using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Collections.Generic;

namespace Ferremas.Api.Services
{
    public class ChilexpressCotizadorService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;
        private readonly ILogger<ChilexpressCotizadorService> _logger;

        public ChilexpressCotizadorService(HttpClient httpClient, IConfiguration configuration, ILogger<ChilexpressCotizadorService> logger)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Chilexpress:CotizadorApiKey"] ?? "";
            _baseUrl = configuration["Chilexpress:BaseUrl"] ?? "https://api.chilexpress.cl/v1/";
            _logger = logger;
        }

        public async Task<List<CoberturaResult>> BuscarCoberturasAsync(string nombre)
        {
            var url = _baseUrl + $"coverage-areas?RegionCode=R01&type=1";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Respuesta cruda de Chilexpress: " + content);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Error al buscar coberturas: {content}");
                throw new Exception($"Error al buscar coberturas: {content}");
            }
            var result = JsonSerializer.Deserialize<CoberturaResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result?.CoverageAreas ?? new List<CoberturaResult>();
        }

        public async Task<List<CotizacionResult>> CotizarEnvioAsync(CotizacionRequest requestDto)
        {
            var url = _baseUrl + "tarifas/cotizador";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Ocp-Apim-Subscription-Key", _apiKey);
            request.Content = new StringContent(JsonSerializer.Serialize(requestDto), Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Error al cotizar envío: {content}");
                throw new Exception($"Error al cotizar envío: {content}");
            }
            var result = JsonSerializer.Deserialize<CotizacionResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result?.Tarifas ?? new List<CotizacionResult>();
        }
    }

    public class CoberturaResponse
    {
        public List<CoberturaResult> CoverageAreas { get; set; } = new();
    }
    public class CoberturaResult
    {
        public string CountyCode { get; set; } = string.Empty;
        public string CountyName { get; set; } = string.Empty;
        public string RegionCode { get; set; } = string.Empty;
        public int IneCountyCode { get; set; }
        public int QueryMode { get; set; }
        public string CoverageName { get; set; } = string.Empty;
    }
    public class CotizacionRequest
    {
        public string CodigoCoberturaOrigen { get; set; } = string.Empty;
        public string CodigoCoberturaDestino { get; set; } = string.Empty;
        public double Peso { get; set; }
        public double Largo { get; set; }
        public double Ancho { get; set; }
        public double Alto { get; set; }
    }
    public class CotizacionResponse
    {
        public List<CotizacionResult> Tarifas { get; set; } = new();
    }
    public class CotizacionResult
    {
        public string Servicio { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string PlazoEntrega { get; set; } = string.Empty;
    }
} 