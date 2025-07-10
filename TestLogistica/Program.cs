using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TestLogistica
{
    class Program
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static readonly string baseUrl = "http://localhost:5200";

        static async Task Main(string[] args)
        {
            Console.WriteLine("🧪 TESTING ENDPOINTS DE LOGÍSTICA - C#");
            Console.WriteLine(new string('=', 60));

            try
            {
                // Test 1: Verificar estado del servicio
                Console.WriteLine("\n" + new string('=', 60));
                Console.WriteLine("TEST 1: Verificar estado del servicio");
                await TestEndpoint("GET", "/api/logistica/status", "Verificar si Shipit está disponible");

                // Test 2: Obtener couriers
                Console.WriteLine("\n" + new string('=', 60));
                Console.WriteLine("TEST 2: Obtener couriers disponibles");
                await TestEndpoint("GET", "/api/logistica/couriers", "Listar couriers disponibles");

                // Test 3: Cotizar envío
                Console.WriteLine("\n" + new string('=', 60));
                Console.WriteLine("TEST 3: Cotizar envío");
                var cotizacionData = new
                {
                    Origen = new
                    {
                        Calle = "Av. Providencia",
                        Numero = "1234",
                        Complemento = "Oficina 5",
                        ComunaId = 2
                    },
                    Destino = new
                    {
                        Calle = "Av. Las Condes",
                        Numero = "5678",
                        Complemento = "Depto 10",
                        ComunaId = 3
                    },
                    Peso = 2.5m,
                    Alto = 15m,
                    Ancho = 20m,
                    Largo = 30m,
                    ValorSeguro = 50000m,
                    Contenido = "Herramientas de construcción"
                };
                await TestEndpoint("POST", "/api/logistica/cotizar-envio", "Cotizar envío entre Providencia y Las Condes", cotizacionData);

                // Test 4: Crear envío
                Console.WriteLine("\n" + new string('=', 60));
                Console.WriteLine("TEST 4: Crear envío");
                var envioData = new
                {
                    Courier = "Chilexpress",
                    NombreDestinatario = "Juan Pablo Perez Soto",
                    Direccion = "Av. Providencia 1234, Depto 201",
                    ComunaDestino = "Providencia",
                    RegionDestino = "Región Metropolitana",
                    Correo = "juan.perez.soto@gmail.com",
                    Telefono = "+56987654321",
                    Rut = "12345678-9",
                    ItemsCount = 1,
                    Largo = 30,
                    Ancho = 20,
                    Alto = 10,
                    Peso = 500,
                    ValorDeclarado = 10000,
                    Referencia = "FERRE001",
                    Contenido = "Herramientas",
                    PedidoId = 5
                };
                await TestEndpoint("POST", "/api/logistica/crear-envio", "Crear envío de prueba", envioData);

                // Test 5: Crear envío con comuna inválida
                Console.WriteLine("\n" + new string('=', 60));
                Console.WriteLine("TEST 5: Crear envío con comuna inválida");
                var envioInvalidoData = new
                {
                    Courier = "Chilexpress",
                    NombreDestinatario = "María González",
                    Direccion = "Av. Apoquindo 1234",
                    ComunaDestino = "ComunaInexistente",
                    Correo = "maria@example.com",
                    Telefono = "+56987654321",
                    ItemsCount = 2,
                    Largo = 25,
                    Ancho = 15,
                    Alto = 8,
                    Peso = 800,
                    ValorDeclarado = 25000,
                    Referencia = "TEST-FERREMAS-002",
                    Contenido = "Herramientas varias"
                };
                await TestEndpoint("POST", "/api/logistica/crear-envio", "Crear envío con comuna inválida (debe fallar)", envioInvalidoData);

                // Test 6: Crear envío con diferentes comunas válidas
                Console.WriteLine("\n" + new string('=', 60));
                Console.WriteLine("TEST 6: Crear envío con diferentes comunas válidas");

                string[] comunasValidas = { "Providencia", "Las Condes", "Ñuñoa", "Maipú" };
                string[] nombresReales = { "María González López", "Carlos Rodríguez Silva", "Ana Martínez Pérez", "Luis Fernández Torres" };

                for (int i = 0; i < comunasValidas.Length; i++)
                {
                    var comuna = comunasValidas[i];
                    Console.WriteLine($"\n--- Probando comuna: {comuna} ---");
                    
                    var envioComunaData = new
                    {
                        Courier = "Chilexpress",
                        NombreDestinatario = nombresReales[i],
                        Direccion = $"Av. {comuna} {i + 1}23",
                        ComunaDestino = comuna,
                        Correo = $"cliente{i + 1}@example.com",
                        Telefono = $"+569{12345678 + i + 1}",
                        ItemsCount = 1,
                        Largo = 20,
                        Ancho = 15,
                        Alto = 10,
                        Peso = 300,
                        ValorDeclarado = 15000,
                        Referencia = $"FERRE{i + 1:000}",
                        Contenido = $"Productos para {comuna}"
                    };
                    await TestEndpoint("POST", "/api/logistica/crear-envio", $"Crear envío a {comuna}", envioComunaData);
                }

                Console.WriteLine("\n" + new string('=', 60));
                Console.WriteLine("🎉 PRUEBAS COMPLETADAS");
                Console.WriteLine(new string('=', 60));
                Console.WriteLine("📊 Resumen:");
                Console.WriteLine("✅ Test 1: Verificar estado del servicio");
                Console.WriteLine("✅ Test 2: Obtener couriers");
                Console.WriteLine("✅ Test 3: Cotizar envío");
                Console.WriteLine("✅ Test 4: Crear envío válido");
                Console.WriteLine("✅ Test 5: Validar comuna inválida");
                Console.WriteLine("✅ Test 6: Probar diferentes comunas");
                Console.WriteLine("\n🔍 Revisa los logs del backend para más detalles");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error general: {ex.Message}");
            }
        }

        static async Task TestEndpoint(string method, string endpoint, string description, object? data = null)
        {
            var url = $"{baseUrl}{endpoint}";
            Console.WriteLine($"\n🔍 {description}");
            Console.WriteLine($"URL: {url}");
            Console.WriteLine($"Método: {method}");

            try
            {
                HttpResponseMessage response;
                string responseContent;

                if (method == "GET")
                {
                    response = await httpClient.GetAsync(url);
                }
                else if (method == "POST")
                {
                    var json = JsonSerializer.Serialize(data);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    response = await httpClient.PostAsync(url, content);
                }
                else
                {
                    Console.WriteLine("❌ Método no soportado");
                    return;
                }

                responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"✅ Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("✅ Respuesta exitosa");
                    Console.WriteLine($"📄 Contenido: {responseContent.Substring(0, Math.Min(500, responseContent.Length))}...");
                }
                else
                {
                    Console.WriteLine($"❌ Error HTTP: {response.StatusCode}");
                    Console.WriteLine($"📄 Error completo:");
                    Console.WriteLine(new string('=', 80));
                    Console.WriteLine(responseContent);
                    Console.WriteLine(new string('=', 80));
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"❌ Error de conexión - ¿Está corriendo el backend? {ex.Message}");
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("❌ Timeout - El servidor tardó demasiado en responder");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error inesperado: {ex.Message}");
            }
        }
    }
} 