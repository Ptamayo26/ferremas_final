using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Ferremas.Api.Services
{
    public class SolotodoService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public SolotodoService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = "https://api.solotodo.com/v1";
        }

        public async Task<string> BuscarProductoAsync(string query)
        {
            try
            {
                var url = $"{_baseUrl}/products/?search={Uri.EscapeDataString(query)}&fields=name,url,active_registry";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al buscar productos en Solotodo: {ex.Message}", ex);
            }
        }
    }
} 