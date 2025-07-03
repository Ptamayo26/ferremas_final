using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ferremas.Api.Services
{
    public class ChileGeoService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ChileGeoService> _logger;
        private readonly string _baseUrl = "https://apis.digital.gob.cl/dpa/";

        public ChileGeoService(HttpClient httpClient, ILogger<ChileGeoService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las regiones de Chile
        /// </summary>
        public async Task<List<RegionChile>> GetRegionesAsync()
        {
            try
            {
                _logger.LogInformation("Obteniendo regiones de Chile desde API del gobierno");
                
                var response = await _httpClient.GetAsync($"{_baseUrl}regiones");
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                var regiones = JsonSerializer.Deserialize<List<RegionChile>>(content, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });

                _logger.LogInformation("Se obtuvieron {Count} regiones de Chile", regiones?.Count ?? 0);
                return regiones ?? new List<RegionChile>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener regiones de Chile");
                // Retornar lista estática como fallback
                return GetRegionesFallback();
            }
        }

        /// <summary>
        /// Obtiene las comunas de una región específica
        /// </summary>
        public async Task<List<ComunaChile>> GetComunasByRegionAsync(string codigoRegion)
        {
            try
            {
                _logger.LogInformation("Obteniendo comunas para región {CodigoRegion} desde API del gobierno", codigoRegion);
                
                var response = await _httpClient.GetAsync($"{_baseUrl}regiones/{codigoRegion}/comunas");
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                var comunas = JsonSerializer.Deserialize<List<ComunaChile>>(content, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });

                _logger.LogInformation("Se obtuvieron {Count} comunas para la región {CodigoRegion}", comunas?.Count ?? 0, codigoRegion);
                return comunas ?? new List<ComunaChile>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener comunas para región {CodigoRegion}", codigoRegion);
                // Retornar lista estática como fallback
                return GetComunasFallback(codigoRegion);
            }
        }

        /// <summary>
        /// Obtiene todas las comunas de Chile
        /// </summary>
        public async Task<List<ComunaChile>> GetTodasLasComunasAsync()
        {
            try
            {
                _logger.LogInformation("Obteniendo todas las comunas de Chile desde API del gobierno");
                
                var response = await _httpClient.GetAsync($"{_baseUrl}comunas");
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                var comunas = JsonSerializer.Deserialize<List<ComunaChile>>(content, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });

                _logger.LogInformation("Se obtuvieron {Count} comunas de Chile", comunas?.Count ?? 0);
                return comunas ?? new List<ComunaChile>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las comunas de Chile");
                // Retornar lista estática como fallback
                return GetTodasLasComunasFallback();
            }
        }

        /// <summary>
        /// Busca comunas por nombre (búsqueda parcial)
        /// </summary>
        public async Task<List<ComunaChile>> BuscarComunasAsync(string query)
        {
            try
            {
                _logger.LogInformation("Buscando comunas con query: {Query}", query);
                
                var todasLasComunas = await GetTodasLasComunasAsync();
                var comunasFiltradas = todasLasComunas
                    .Where(c => c.Nombre.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .Take(20) // Limitar resultados
                    .ToList();

                _logger.LogInformation("Se encontraron {Count} comunas para la búsqueda '{Query}'", comunasFiltradas.Count, query);
                return comunasFiltradas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar comunas con query: {Query}", query);
                return new List<ComunaChile>();
            }
        }

        #region Fallback Data (datos estáticos como respaldo)

        private List<RegionChile> GetRegionesFallback()
        {
            return new List<RegionChile>
            {
                new RegionChile { Codigo = "01", Nombre = "Tarapacá", NombreOficial = "Región de Tarapacá" },
                new RegionChile { Codigo = "02", Nombre = "Antofagasta", NombreOficial = "Región de Antofagasta" },
                new RegionChile { Codigo = "03", Nombre = "Atacama", NombreOficial = "Región de Atacama" },
                new RegionChile { Codigo = "04", Nombre = "Coquimbo", NombreOficial = "Región de Coquimbo" },
                new RegionChile { Codigo = "05", Nombre = "Valparaíso", NombreOficial = "Región de Valparaíso" },
                new RegionChile { Codigo = "06", Nombre = "O'Higgins", NombreOficial = "Región del Libertador General Bernardo O'Higgins" },
                new RegionChile { Codigo = "07", Nombre = "Maule", NombreOficial = "Región del Maule" },
                new RegionChile { Codigo = "08", Nombre = "Biobío", NombreOficial = "Región del Biobío" },
                new RegionChile { Codigo = "09", Nombre = "La Araucanía", NombreOficial = "Región de La Araucanía" },
                new RegionChile { Codigo = "10", Nombre = "Los Lagos", NombreOficial = "Región de Los Lagos" },
                new RegionChile { Codigo = "11", Nombre = "Aysén", NombreOficial = "Región de Aysén del General Carlos Ibáñez del Campo" },
                new RegionChile { Codigo = "12", Nombre = "Magallanes", NombreOficial = "Región de Magallanes y de la Antártica Chilena" },
                new RegionChile { Codigo = "13", Nombre = "Metropolitana", NombreOficial = "Región Metropolitana de Santiago" },
                new RegionChile { Codigo = "14", Nombre = "Los Ríos", NombreOficial = "Región de Los Ríos" },
                new RegionChile { Codigo = "15", Nombre = "Arica y Parinacota", NombreOficial = "Región de Arica y Parinacota" },
                new RegionChile { Codigo = "16", Nombre = "Ñuble", NombreOficial = "Región de Ñuble" }
            };
        }

        private List<ComunaChile> GetComunasFallback(string codigoRegion)
        {
            // Comunas más comunes por región como fallback
            var comunasPorRegion = new Dictionary<string, List<ComunaChile>>
            {
                ["13"] = new List<ComunaChile> // Metropolitana
                {
                    new ComunaChile { Codigo = "13101", Nombre = "Santiago", CodigoRegion = "13" },
                    new ComunaChile { Codigo = "13102", Nombre = "Providencia", CodigoRegion = "13" },
                    new ComunaChile { Codigo = "13103", Nombre = "Las Condes", CodigoRegion = "13" },
                    new ComunaChile { Codigo = "13104", Nombre = "Ñuñoa", CodigoRegion = "13" },
                    new ComunaChile { Codigo = "13105", Nombre = "Maipú", CodigoRegion = "13" },
                    new ComunaChile { Codigo = "13106", Nombre = "La Florida", CodigoRegion = "13" },
                    new ComunaChile { Codigo = "13107", Nombre = "Puente Alto", CodigoRegion = "13" },
                    new ComunaChile { Codigo = "13108", Nombre = "San Bernardo", CodigoRegion = "13" },
                    new ComunaChile { Codigo = "13109", Nombre = "Peñalolén", CodigoRegion = "13" },
                    new ComunaChile { Codigo = "13110", Nombre = "La Cisterna", CodigoRegion = "13" }
                },
                ["05"] = new List<ComunaChile> // Valparaíso
                {
                    new ComunaChile { Codigo = "05101", Nombre = "Valparaíso", CodigoRegion = "05" },
                    new ComunaChile { Codigo = "05102", Nombre = "Viña del Mar", CodigoRegion = "05" },
                    new ComunaChile { Codigo = "05103", Nombre = "Quilpué", CodigoRegion = "05" },
                    new ComunaChile { Codigo = "05104", Nombre = "Villa Alemana", CodigoRegion = "05" }
                },
                ["08"] = new List<ComunaChile> // Biobío
                {
                    new ComunaChile { Codigo = "08101", Nombre = "Concepción", CodigoRegion = "08" },
                    new ComunaChile { Codigo = "08102", Nombre = "Talcahuano", CodigoRegion = "08" },
                    new ComunaChile { Codigo = "08103", Nombre = "Chillán", CodigoRegion = "08" }
                }
            };

            return comunasPorRegion.GetValueOrDefault(codigoRegion, new List<ComunaChile>());
        }

        private List<ComunaChile> GetTodasLasComunasFallback()
        {
            var todasLasComunas = new List<ComunaChile>();
            
            // Agregar comunas de todas las regiones
            foreach (var region in GetRegionesFallback())
            {
                todasLasComunas.AddRange(GetComunasFallback(region.Codigo));
            }
            
            return todasLasComunas;
        }

        #endregion
    }

    public class RegionChile
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string NombreOficial { get; set; } = string.Empty;
    }

    public class ComunaChile
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string CodigoRegion { get; set; } = string.Empty;
        public string NombreRegion { get; set; } = string.Empty;
    }
} 