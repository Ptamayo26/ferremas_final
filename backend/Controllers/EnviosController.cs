using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Ferremas.Api.DTOs;
using Ferremas.Api.Services.Interfaces;
using Ferremas.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;
using Ferremas.Api.Data;
using Ferremas.Api.Models;

namespace Ferremas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnviosController : ControllerBase
    {
        private readonly IEnvioService _envioService;
        private readonly WhatsAppWebService _whatsAppService;
        private readonly ChilexpressCotizadorService _chilexpressService;
        private readonly ShipitService _shipitService;
        private readonly AppDbContext _context;

        public EnviosController(IEnvioService envioService, WhatsAppWebService whatsAppService, 
            ChilexpressCotizadorService chilexpressService, ShipitService shipitService, AppDbContext context)
        {
            _envioService = envioService;
            _whatsAppService = whatsAppService;
            _chilexpressService = chilexpressService;
            _shipitService = shipitService;
            _context = context;
            Console.WriteLine("[LOG] EnviosController instanciado correctamente");
        }

        [HttpGet("{pedidoId}")]
        public async Task<IActionResult> ObtenerEnvioPorPedido(int pedidoId)
        {
            var envio = await _envioService.ObtenerEnvioPorPedidoAsync(pedidoId);
            if (envio == null)
                return NotFound($"No se encontró envío para el pedido {pedidoId}");
            
            return Ok(envio);
        }

        [HttpPost]
        public async Task<IActionResult> CrearEnvio([FromBody] EnvioCreateDTO dto)
        {
            var envio = await _envioService.CrearEnvioAsync(dto);
            
            // Generar link de WhatsApp para notificación
            string numeroCliente = dto.TelefonoContacto;
            string mensaje = $"¡Tu pedido #{dto.PedidoId} ha sido despachado!\n" +
                $"Dirección: {dto.DireccionDestino}\n" +
                $"Comuna: {dto.ComunaDestino}\n" +
                $"Región: {dto.RegionDestino}\n" +
                           "Pronto recibirás tu compra.";
            
            var whatsappUrl = _whatsAppService.GenerarLinkWhatsapp(numeroCliente, mensaje);

            return Ok(new { 
                envio = envio,
                whatsapp_url = whatsappUrl
            });
        }

        [HttpPut("{envioId}/estado")]
        public async Task<IActionResult> ActualizarEstado(int envioId, [FromBody] string nuevoEstado)
        {
            var resultado = await _envioService.ActualizarEstadoEnvioAsync(envioId, nuevoEstado);
            if (!resultado)
                return NotFound($"No se encontró el envío {envioId}");
            
            return Ok(new { mensaje = "Estado actualizado correctamente" });
        }

        [HttpGet("coberturas")]
        public async Task<IActionResult> BuscarCoberturas([FromQuery] string nombre)
        {
            var resultados = await _chilexpressService.BuscarCoberturasAsync(nombre);
            return Ok(resultados);
        }

        [HttpPost("cotizar")]
        public async Task<IActionResult> CotizarEnvio([FromBody] CotizacionRequest request)
        {
            var resultados = await _chilexpressService.CotizarEnvioAsync(request);
            return Ok(resultados);
        }

        // Endpoints para Shipit
        [HttpGet("shipit/coberturas")]
        public async Task<IActionResult> BuscarCoberturasShipit([FromQuery] string query)
        {
            try
            {
                var resultados = await _shipitService.BuscarCoberturasAsync(query);
                
                if (resultados.CoverageAreas.Count == 0)
                {
                    return Ok(new { 
                        message = "No se encontraron coberturas en Shipit",
                        available = false,
                        data = new List<object>()
                    });
                }
                
                return Ok(new { 
                    message = "Coberturas encontradas en Shipit",
                    available = true,
                    data = resultados.CoverageAreas
                });
            }
            catch (Exception ex)
            {
                return Ok(new { 
                    message = $"Error al buscar coberturas en Shipit: {ex.Message}",
                    available = false,
                    data = new List<object>()
                });
            }
        }

        [HttpGet("shipit/couriers")]
        public async Task<IActionResult> ObtenerCouriersShipit()
        {
            try
            {
                var couriers = await _shipitService.ObtenerCouriersAsync();
                
                if (couriers.Count == 0)
                {
                    return Ok(new { 
                        message = "No se encontraron couriers en Shipit",
                        available = false,
                        data = new List<object>()
                    });
                }
                
                return Ok(new { 
                    message = "Couriers encontrados en Shipit",
                    available = true,
                    data = couriers
                });
            }
            catch (Exception ex)
            {
                return Ok(new { 
                    message = $"Error al obtener couriers de Shipit: {ex.Message}",
                    available = false,
                    data = new List<object>()
                });
            }
        }

        [HttpPost("shipit/cotizar")]
        public async Task<IActionResult> CotizarEnvioShipit([FromBody] ShipitCotizacionRequest request)
        {
            try
            {
                var resultados = await _shipitService.CotizarEnvioAsync(request);
                
                if (resultados.Quotations.Count == 0)
                {
                    return Ok(new { 
                        message = "No se encontraron cotizaciones en Shipit. Verifica que el commune_id sea válido.",
                        available = false,
                        data = new List<object>(),
                        error = "commune_id_invalid"
                    });
                }
                
                return Ok(new { 
                    message = "Cotizaciones encontradas en Shipit",
                    available = true,
                    data = resultados.Quotations
                });
            }
            catch (Exception ex)
            {
                return Ok(new { 
                    message = $"Error al cotizar envío con Shipit: {ex.Message}",
                    available = false,
                    data = new List<object>(),
                    error = "shipit_error"
                });
            }
        }

        [HttpPost("shipit/test-commune")]
        public async Task<IActionResult> ProbarCommuneShipit([FromBody] int communeId)
        {
            try
            {
                var request = new ShipitCotizacionRequest
                {
                    Origen = new ShipitDireccion { Calle = "Av. Providencia", Numero = "123", ComunaId = 13101 },
                    Destino = new ShipitDireccion { Calle = "Av. Las Condes", Numero = "456", ComunaId = communeId },
                    Peso = 1,
                    Alto = 10,
                    Ancho = 10,
                    Largo = 10,
                    ValorSeguro = 0,
                    Contenido = "Test Ferremas"
                };

                var resultados = await _shipitService.CotizarEnvioAsync(request);
                
                return Ok(new { 
                    commune_id = communeId,
                    success = resultados.Quotations.Count > 0,
                    message = resultados.Quotations.Count > 0 ? "Commune ID válido" : "Commune ID inválido",
                    data = resultados.Quotations
                });
            }
            catch (Exception ex)
            {
                return Ok(new { 
                    commune_id = communeId,
                    success = false,
                    message = $"Error: {ex.Message}",
                    data = new List<object>()
                });
            }
        }

        [HttpGet("cotizaciones")]
        public async Task<IActionResult> ObtenerCotizaciones([FromQuery] string origen, [FromQuery] string destino, 
            [FromQuery] decimal peso = 1, [FromQuery] decimal alto = 10, [FromQuery] decimal ancho = 10, 
            [FromQuery] decimal largo = 10)
        {
            try
            {
                var chilexpressResults = new List<object>();
                var shipitResults = new List<object>();
                var messages = new List<string>();

                // Cotización con Chilexpress
                try
                {
                    var requestChilexpress = new CotizacionRequest
                    {
                        CodigoCoberturaOrigen = "13101", // Santiago
                        CodigoCoberturaDestino = "13102", // Providencia
                        Peso = (double)peso,
                        Largo = (double)largo,
                        Ancho = (double)ancho,
                        Alto = (double)alto
                    };

                    var cotizacionChilexpress = await _chilexpressService.CotizarEnvioAsync(requestChilexpress);
                    chilexpressResults = cotizacionChilexpress.Cast<object>().ToList();
                }
                catch (Exception ex)
                {
                    messages.Add($"Error en Chilexpress: {ex.Message}");
                }

                // Cotización con Shipit
                try
                {
                    var requestShipit = new ShipitCotizacionRequest
                    {
                        Origen = new ShipitDireccion { Calle = "Av. Providencia", Numero = "123", ComunaId = 13101 },
                        Destino = new ShipitDireccion { Calle = destino, Numero = "456", ComunaId = 13102 },
                        Peso = peso,
                        Alto = alto,
                        Ancho = ancho,
                        Largo = largo,
                        ValorSeguro = 0,
                        Contenido = "Productos Ferremas"
                    };

                    var cotizacionShipit = await _shipitService.CotizarEnvioAsync(requestShipit);
                    shipitResults = cotizacionShipit.Quotations.Cast<object>().ToList();
                }
                catch (Exception ex)
                {
                    messages.Add($"Error en Shipit: {ex.Message}");
                }

                var resultado = new
                {
                    chilexpress = chilexpressResults,
                    shipit = shipitResults,
                    messages = messages
                };

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al obtener cotizaciones: {ex.Message}");
            }
        }

        [HttpGet("shipit/status")]
        public async Task<IActionResult> VerificarEstadoShipit()
        {
            try
            {
                var isAvailable = await _shipitService.IsServiceAvailableAsync();
                return Ok(new { 
                    available = isAvailable,
                    message = isAvailable ? "Shipit está disponible" : "Shipit no está disponible"
                });
            }
            catch (Exception ex)
            {
                return Ok(new { 
                    available = false,
                    message = $"Error al verificar Shipit: {ex.Message}"
                });
            }
        }

        [HttpGet("entregas")]
        [Authorize(Roles = "contador,administrador,repartidor")]
        public async Task<IActionResult> GetEntregas()
        {
            Console.WriteLine("[LOG] GetEntregas alcanzado");
            var user = HttpContext.User;
            var roles = user.Claims.Where(c => c.Type == "role" || c.Type.Contains("Role")).Select(c => c.Value).ToList();
            var allClaims = user.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();
            Console.WriteLine("ROLES DEL USUARIO: " + string.Join(", ", roles));
            Console.WriteLine("TODOS LOS CLAIMS: " + string.Join(" | ", allClaims));
            var entregas = await _context.Envios
                .Include(e => e.Pedido)
                .ThenInclude(p => p.Usuario)
                .Select(e => new {
                    e.Id,
                    PedidoId = e.PedidoId,
                    Cliente = e.Pedido.Usuario != null ? e.Pedido.Usuario.Nombre : "-",
                    Repartidor = e.RepartidorId != null ? e.RepartidorId.ToString() : "-", // Ajusta si tienes relación con repartidor
                    Fecha = e.FechaCreacion,
                    e.EstadoEnvio
                })
                .ToListAsync();

            return Ok(new { exito = true, datos = entregas });
        }

        [HttpGet("logistica")]
        [Authorize(Roles = "contador,administrador,repartidor")]
        public async Task<IActionResult> GetLogistica()
        {
            var despachos = await _context.Envios
                .Include(e => e.Pedido)
                .ThenInclude(p => p.Usuario)
                .Select(e => new {
                    e.Id,
                    PedidoId = e.PedidoId,
                    Cliente = e.Pedido.Usuario != null ? e.Pedido.Usuario.Nombre : "-",
                    EmpresaTransporte = e.ProveedorTransporte,
                    e.EstadoEnvio,
                    Fecha = e.FechaCreacion
                })
                .ToListAsync();

            return Ok(new { exito = true, datos = despachos });
        }
    }
}