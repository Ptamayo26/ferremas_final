using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Collections.Generic;

namespace Ferremas.Api.Services
{
    public class ShipitService
    {
        private readonly HttpClient _httpClient;
        private readonly string _email;
        private readonly string _token;
        private readonly ILogger<ShipitService> _logger;
        private readonly ShipitDefaultSettings _defaultSettings;

        public ShipitService(HttpClient httpClient, IConfiguration configuration, ILogger<ShipitService> logger)
        {
            _httpClient = httpClient;
            _email = configuration["Shipit:Email"] ?? "";
            _token = configuration["Shipit:Token"] ?? "";
            _logger = logger;
            
            _defaultSettings = new ShipitDefaultSettings
            {
                IsPayable = configuration.GetValue<bool>("Shipit:DefaultSettings:IsPayable"),
                Packing = configuration["Shipit:DefaultSettings:Packing"] ?? "Sin empaque",
                ShippingType = configuration["Shipit:DefaultSettings:ShippingType"] ?? "Normal",
                Destiny = configuration["Shipit:DefaultSettings:Destiny"] ?? "domicilio",
                CourierForClient = configuration["Shipit:DefaultSettings:CourierForClient"] ?? "chilexpress"
            };
        }

        public async Task<ShipitCotizacionResponse> CotizarEnvioAsync(ShipitCotizacionRequest request)
        {
            try
            {
                var url = "https://api.shipit.cl/v/3/quotations";
                var requestBody = new
                {
                    email = _email,
                    token = _token,
                    origin = new
                    {
                        street = request.Origen.Calle,
                        number = request.Origen.Numero,
                        complement = request.Origen.Complemento,
                        commune_id = request.Origen.ComunaId,
                        delivery_type = "domicilio"
                    },
                    destiny = new
                    {
                        street = request.Destino.Calle,
                        number = request.Destino.Numero,
                        complement = request.Destino.Complemento,
                        commune_id = request.Destino.ComunaId,
                        delivery_type = "domicilio"
                    },
                    packages = new[]
                    {
                        new
                        {
                            weight = request.Peso,
                            height = request.Alto,
                            width = request.Ancho,
                            length = request.Largo,
                            insurance = request.ValorSeguro,
                            content = request.Contenido
                        }
                    },
                    is_payable = _defaultSettings.IsPayable,
                    packing = _defaultSettings.Packing,
                    shipping_type = _defaultSettings.ShippingType,
                    destiny = _defaultSettings.Destiny,
                    courier_for_client = _defaultSettings.CourierForClient
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                _logger.LogInformation($"Enviando cotización a Shipit: {json}");
                
                var response = await _httpClient.PostAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation($"Respuesta de Shipit: {responseContent}");
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error en cotización Shipit: {responseContent}");
                    throw new Exception($"Error al cotizar envío con Shipit: {responseContent}");
                }

                var result = JsonSerializer.Deserialize<ShipitCotizacionResponse>(responseContent, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                return result ?? new ShipitCotizacionResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en ShipitService.CotizarEnvioAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<ShipitCoberturaResponse> BuscarCoberturasAsync(string query)
        {
            try
            {
                var url = $"https://api.shipit.cl/v/3/coverage_areas?q={Uri.EscapeDataString(query)}";
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("X-Shipit-Email", _email);
                request.Headers.Add("X-Shipit-Token", _token);
                
                var response = await _httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error al buscar coberturas en Shipit: {content}");
                    throw new Exception($"Error al buscar coberturas: {content}");
                }

                var result = JsonSerializer.Deserialize<ShipitCoberturaResponse>(content, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                return result ?? new ShipitCoberturaResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en ShipitService.BuscarCoberturasAsync: {ex.Message}");
                throw;
            }
        }
    }

    public class ShipitDefaultSettings
    {
        public bool IsPayable { get; set; }
        public string Packing { get; set; } = string.Empty;
        public string ShippingType { get; set; } = string.Empty;
        public string Destiny { get; set; } = string.Empty;
        public string CourierForClient { get; set; } = string.Empty;
    }

    public class ShipitCotizacionRequest
    {
        public ShipitDireccion Origen { get; set; } = new();
        public ShipitDireccion Destino { get; set; } = new();
        public decimal Peso { get; set; }
        public decimal Alto { get; set; }
        public decimal Ancho { get; set; }
        public decimal Largo { get; set; }
        public decimal ValorSeguro { get; set; }
        public string Contenido { get; set; } = string.Empty;
    }

    public class ShipitDireccion
    {
        public string Calle { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Complemento { get; set; } = string.Empty;
        public int ComunaId { get; set; }
    }

    public class ShipitCotizacionResponse
    {
        public List<ShipitCotizacion> Quotations { get; set; } = new();
    }

    public class ShipitCotizacion
    {
        public int Id { get; set; }
        public string Courier { get; set; } = string.Empty;
        public string Service { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DeliveryTime { get; set; }
        public string DeliveryTimeText { get; set; } = string.Empty;
    }

    public class ShipitCoberturaResponse
    {
        public List<ShipitCobertura> CoverageAreas { get; set; } = new();
    }

    public class ShipitCobertura
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
} 