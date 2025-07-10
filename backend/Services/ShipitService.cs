using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Collections.Generic;
using Ferremas.Api.DTOs;

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
                // Usar el endpoint de shipments que funciona
                var url = "https://api.shipit.cl/v/shipments";
                
                // Crear el request body según la documentación oficial
                var requestBody = new
                {
                    kind = 0, // shipit
                    platform = 2, // api
                    reference = $"FERREMAS-{DateTime.Now:yyyyMMdd-HHmmss}",
                    destiny = new
                    {
                        kind = "home_delivery",
                        street = request.Destino.Calle,
                        number = request.Destino.Numero,
                        complement = request.Destino.Complemento,
                        delivery_type = "domicilio",
                        commune_id = request.Destino.ComunaId // Incluir commune_id
                    },
                    sizes = new
                    {
                        length = request.Largo,
                        height = request.Alto,
                        width = request.Ancho,
                        weight = request.Peso
                    },
                    items = 1,
                    sandbox = false,
                    courier = new
                    {
                        payable = _defaultSettings.IsPayable,
                        selected = false,
                        algorithm = 1,
                        algorithm_days = 0,
                        without_courier = false
                    },
                    insurance = new
                    {
                        ticket_amount = request.ValorSeguro,
                        extra = false
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = content
                };
                
                // Headers correctos según la documentación
                httpRequest.Headers.Add("Accept", "application/vnd.shipit.v4");
                httpRequest.Headers.Add("X-Shipit-Email", _email);
                httpRequest.Headers.Add("X-Shipit-Access-Token", _token);
                
                _logger.LogInformation($"Enviando cotización a Shipit: {json}");
                
                var response = await _httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation($"Respuesta de Shipit: {responseContent}");
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Error en cotización Shipit: {responseContent}");
                    // En lugar de lanzar excepción, devolver respuesta vacía
                    return new ShipitCotizacionResponse();
                }

                var result = JsonSerializer.Deserialize<ShipitCotizacionResponse>(responseContent, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                return result ?? new ShipitCotizacionResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en ShipitService.CotizarEnvioAsync: {ex.Message}");
                // En lugar de lanzar excepción, devolver respuesta vacía
                return new ShipitCotizacionResponse();
            }
        }

        public async Task<ShipitCoberturaResponse> BuscarCoberturasAsync(string query)
        {
            try
            {
                // El endpoint de cobertura no funciona, usar couriers en su lugar
                var url = "https://api.shipit.cl/v/couriers";
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                
                // Headers correctos según la documentación
                request.Headers.Add("Accept", "application/vnd.shipit.v4");
                request.Headers.Add("X-Shipit-Email", _email);
                request.Headers.Add("X-Shipit-Access-Token", _token);
                
                var response = await _httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Error al buscar couriers en Shipit: {content}");
                    return new ShipitCoberturaResponse();
                }

                // Convertir la respuesta de couriers a formato de cobertura
                var couriers = JsonSerializer.Deserialize<List<ShipitCourier>>(content, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                var coberturaResponse = new ShipitCoberturaResponse();
                if (couriers != null)
                {
                    foreach (var courier in couriers)
                    {
                        coberturaResponse.CoverageAreas.Add(new ShipitCobertura
                        {
                            Id = courier.Id,
                            Name = courier.Name,
                            Region = "Chile",
                            Country = "Chile"
                        });
                    }
                }
                
                return coberturaResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en ShipitService.BuscarCoberturasAsync: {ex.Message}");
                return new ShipitCoberturaResponse();
            }
        }

        /// <summary>
        /// Obtiene la lista de couriers disponibles
        /// </summary>
        public async Task<List<ShipitCourier>> ObtenerCouriersAsync()
        {
            try
            {
                var url = "https://api.shipit.cl/v/couriers";
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                
                request.Headers.Add("Accept", "application/vnd.shipit.v4");
                request.Headers.Add("X-Shipit-Email", _email);
                request.Headers.Add("X-Shipit-Access-Token", _token);
                
                var response = await _httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Error al obtener couriers de Shipit: {content}");
                    return new List<ShipitCourier>();
                }

                var couriers = JsonSerializer.Deserialize<List<ShipitCourier>>(content, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                return couriers ?? new List<ShipitCourier>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en ShipitService.ObtenerCouriersAsync: {ex.Message}");
                return new List<ShipitCourier>();
            }
        }

        /// <summary>
        /// Verifica si el servicio de Shipit está disponible
        /// </summary>
        public async Task<bool> IsServiceAvailableAsync()
        {
            try
            {
                var url = "https://api.shipit.cl/v/couriers";
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                
                request.Headers.Add("Accept", "application/vnd.shipit.v4");
                request.Headers.Add("X-Shipit-Email", _email);
                request.Headers.Add("X-Shipit-Access-Token", _token);
                
                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Crea un envío en Shipit
        /// </summary>
        public async Task<ShipitEnvioResponse> CrearEnvioAsync(ShipitEnvioRequestDto envio)
        {
            try
            {
                var comunaId = GetCommuneId(envio.ComunaDestino);
                if (comunaId == 0)
                {
                    _logger.LogWarning($"Comuna no válida: {envio.ComunaDestino}");
                    return new ShipitEnvioResponse 
                    { 
                        Success = false, 
                        Error = $"Comuna no válida: {envio.ComunaDestino}" 
                    };
                }

                var url = "https://api.shipit.cl/v/shipments";
                
                var payload = new
                {
                    kind = 0,
                    platform = 2,
                    reference = string.IsNullOrWhiteSpace(envio.Referencia)
                        ? $"FERREMAS-{DateTime.Now:yyyyMMdd-HHmmss}"
                        : envio.Referencia.Length > 15 ? envio.Referencia.Substring(0, 15) : envio.Referencia,
                    destiny = new
                    {
                        kind = "home_delivery",
                        street = envio.Direccion,
                        number = "",
                        complement = "",
                        delivery_type = "domicilio",
                        commune_id = comunaId,
                        full_name = envio.NombreDestinatario ?? "Sin Nombre" // ✅ aquí debe ir
                    },
                    sizes = new
                    {
                        length = envio.Largo,
                        width = envio.Ancho,
                        height = envio.Alto,
                        weight = envio.Peso
                    },
                    items = envio.ItemsCount,
                    sandbox = false,
                    courier = new
                    {
                        payable = _defaultSettings.IsPayable,
                        selected = true,
                        algorithm = 1,
                        algorithm_days = 0,
                        without_courier = false,
                        name = envio.Courier
                    },
                    insurance = new
                    {
                        ticket_amount = envio.ValorDeclarado,
                        extra = false
                    },
                    email = envio.Correo,
                    cellphone = envio.Telefono,
                    content = "Productos de Ferremas"
                };

                var json = JsonSerializer.Serialize(payload);
                _logger.LogInformation("Payload enviado a Shipit: {Payload}", json);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = content
                };
                
                request.Headers.Add("Accept", "application/vnd.shipit.v4");
                request.Headers.Add("X-Shipit-Email", _email);
                request.Headers.Add("X-Shipit-Access-Token", _token);
                
                _logger.LogInformation($"Creando envío en Shipit: {json}");
                
                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation($"Respuesta de Shipit al crear envío: {responseContent}");
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Error al crear envío en Shipit: {responseContent}");
                    return new ShipitEnvioResponse 
                    { 
                        Success = false, 
                        Error = $"Error al crear envío: {responseContent}" 
                    };
                }

                var result = JsonSerializer.Deserialize<ShipitEnvioResponse>(responseContent, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                return result ?? new ShipitEnvioResponse { Success = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en ShipitService.CrearEnvioAsync: {ex.Message}");
                return new ShipitEnvioResponse 
                { 
                    Success = false, 
                    Error = $"Error interno: {ex.Message}" 
                };
            }
        }

        /// <summary>
        /// Obtiene el ID de comuna basado en el nombre
        /// </summary>
        private int GetCommuneId(string comuna)
        {
            // Diccionario ampliado de comunas (ejemplo, puedes seguir ampliando según Shipit)
            var comunas = new Dictionary<string, int>()
            {
                { "Santiago", 1 }, { "Providencia", 2 }, { "Las Condes", 3 },
                { "Ñuñoa", 4 }, { "Maipú", 5 }, { "Puente Alto", 6 },
                { "La Florida", 7 }, { "San Bernardo", 8 }, { "Temuco", 9 },
                { "Concepción", 10 }, { "Valparaíso", 11 }, { "Viña del Mar", 12 },
                { "Antofagasta", 13 }, { "Puerto Montt", 14 }, { "La Serena", 15 },
                { "Rancagua", 16 }, { "Talca", 17 }, { "Chillán", 18 },
                { "Iquique", 19 }, { "Arica", 20 }, { "Los Ángeles", 21 },
                { "Osorno", 22 }, { "Punta Arenas", 23 }, { "Coyhaique", 24 },
                { "Calama", 25 }, { "Quilpué", 26 }, { "Villa Alemana", 27 },
                { "San Felipe", 28 }, { "Quillota", 29 }, { "Melipilla", 30 },
                { "Peñalolén", 31 }, { "Macul", 32 }, { "Recoleta", 33 },
                { "Independencia", 34 }, { "Estación Central", 35 }, { "San Miguel", 36 },
                { "La Cisterna", 37 }, { "El Bosque", 38 }, { "San Ramón", 39 },
                { "Pedro Aguirre Cerda", 40 }, { "Lo Espejo", 41 }, { "Cerrillos", 42 },
                { "Huechuraba", 43 }, { "Conchalí", 44 }, { "Renca", 45 },
                { "Pudahuel", 46 }, { "Cerro Navia", 47 }, { "Lo Prado", 48 },
                { "Quinta Normal", 49 }, { "Lo Barnechea", 50 }, { "Vitacura", 51 },
                { "San Joaquín", 52 }, { "La Granja", 53 }, { "La Pintana", 54 },
                { "San José de Maipo", 55 }, { "Padre Hurtado", 56 }, { "Peñaflor", 57 },
                { "Talagante", 58 }, { "Isla de Maipo", 59 }, { "Buin", 60 },
                { "Paine", 61 }, { "Lampa", 62 }, { "Colina", 63 },
                { "Tiltil", 64 }, { "Curacaví", 65 }, { "El Monte", 66 },
                { "San Pedro", 67 }, { "Pirque", 68 }, { "Alhué", 69 },
                { "María Pinto", 70 }, { "San Fernando", 71 }, { "Rengo", 72 },
                { "Machalí", 73 }, { "Graneros", 74 }, { "Mostazal", 75 },
                { "San Vicente", 76 }, { "Santa Cruz", 77 }, { "Pichilemu", 78 },
                { "Curicó", 79 }, { "Linares", 80 }, { "Parral", 81 },
                { "Cauquenes", 82 }, { "San Javier", 83 }, { "Constitución", 84 },
                { "Maule", 85 }, { "Talcahuano", 86 }, { "Coronel", 87 },
                { "San Pedro de la Paz", 88 }, { "Hualpén", 89 }, { "Chiguayante", 90 },
                { "Tomé", 91 }, { "Cañete", 92 }, { "Lebu", 93 },
                { "Arauco", 94 }, { "Curanilahue", 95 }, { "Lota", 96 },
                { "Angol", 97 }, { "Victoria", 98 }, { "Villarrica", 99 },
                { "Pucón", 100 }, { "Lautaro", 101 }, { "Nueva Imperial", 102 },
                { "Padre Las Casas", 103 }, { "Carahue", 104 }, { "Toltén", 105 },
                { "Valdivia", 106 }, { "La Unión", 107 }, { "Río Bueno", 108 },
                { "Paillaco", 109 }, { "Purranque", 110 }, { "Puerto Varas", 111 },
                { "Castro", 112 }, { "Ancud", 113 }, { "Quellón", 114 },
                { "Chonchi", 115 }, { "Dalcahue", 116 }, { "Futaleufú", 117 },
                { "Chaitén", 118 }, { "Palena", 119 }, { "Aysén", 120 },
                { "Puerto Aysén", 121 }, { "Cisnes", 122 }, { "Porvenir", 123 },
                { "Natales", 124 }, { "Cabo de Hornos", 125 }
            };

            if (string.IsNullOrWhiteSpace(comuna))
                return 0;

            var comunaNormalizada = comuna.Trim().ToLowerInvariant()
                .Replace("á", "a").Replace("é", "e").Replace("í", "i")
                .Replace("ó", "o").Replace("ú", "u").Replace("ñ", "n");

            foreach (var kvp in comunas)
            {
                var keyNorm = kvp.Key.Trim().ToLowerInvariant()
                    .Replace("á", "a").Replace("é", "e").Replace("í", "i")
                    .Replace("ó", "o").Replace("ú", "u").Replace("ñ", "n");
                
                if (keyNorm == comunaNormalizada)
                {
                    return kvp.Value;
                }
            }
            return 0;
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

    public class ShipitCourier
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool AvailableToShip { get; set; }
        public string ImageOriginalUrl { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ShipitCourierServices Services { get; set; } = new();
    }

    public class ShipitCourierServices
    {
        public bool NextDay { get; set; }
        public bool SameDay { get; set; }
        public bool Saturday { get; set; }
        public bool Overnight { get; set; }
    }

    public class ShipitEnvioResponse
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
        public string? TrackingNumber { get; set; }
        public string? LabelUrl { get; set; }
        public decimal? Cost { get; set; }
        public string? Courier { get; set; }
        public string? Service { get; set; }
        public int? DeliveryTime { get; set; }
        public string? DeliveryTimeText { get; set; }
    }
} 