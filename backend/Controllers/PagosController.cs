using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ferremas.Api.Models;
using Ferremas.Api.DTOs;
using Ferremas.Api.Services.Interfaces;
using Ferremas.Api.Services;
using FerremasBackend.Services;
using System;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Ferremas.Api.Data;
using Ferremas.Api.Constants;

namespace Ferremas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PagosController : ControllerBase
    {
        private readonly IPagosService _pagosService;
        private readonly WhatsAppWebService _whatsAppService;
        private readonly MercadoPagoService _mercadoPagoService;
        private readonly FakePaymentService _fakePaymentService;
        private readonly ILogger<PagosController> _logger;
        private readonly AppDbContext _context;

        public PagosController(
            IPagosService pagosService,
            WhatsAppWebService whatsAppService,
            MercadoPagoService mercadoPagoService,
            FakePaymentService fakePaymentService,
            ILogger<PagosController> logger,
            AppDbContext context)
        {
            _pagosService = pagosService;
            _whatsAppService = whatsAppService;
            _mercadoPagoService = mercadoPagoService;
            _fakePaymentService = fakePaymentService;
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Obtiene todos los pagos registrados
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "RequireRole")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<Pago>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var pagos = await _pagosService.GetAll();
                return Ok(new ApiResponse<IEnumerable<Pago>>
                {
                    Exito = true,
                    Mensaje = "Pagos obtenidos exitosamente",
                    Datos = pagos
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los pagos");
                return StatusCode(500, new ApiResponse
                {
                    Exito = false,
                    Mensaje = "Error al obtener los pagos",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene un pago específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = "RequireRole")]
        [ProducesResponseType(typeof(ApiResponse<Pago>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var pago = await _pagosService.GetById(id);
                if (pago == null)
                    return NotFound(new ApiResponse { Exito = false, Mensaje = $"Pago con ID {id} no encontrado" });

                return Ok(new ApiResponse<Pago>
                {
                    Exito = true,
                    Mensaje = "Pago obtenido exitosamente",
                    Datos = pago
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el pago {id}");
                return StatusCode(500, new ApiResponse
                {
                    Exito = false,
                    Mensaje = "Error al obtener el pago",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Procesa un pago con Mercado Pago
        /// </summary>
        [HttpPost("procesar/{pedidoId}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ProcesarPago(int pedidoId, [FromBody] ProcesarPagoRequest request)
        {
            try
            {
                // Validar request
                if (request == null || request.Monto <= 0)
                {
                    return BadRequest(new ApiResponse
                    {
                        Exito = false,
                        Mensaje = "Datos de pago inválidos. El monto debe ser mayor a 0."
                    });
                }

                // Verificar que el pedido existe
                var pedido = await _context.Pedidos
                    .Include(p => p.Usuario)
                    .FirstOrDefaultAsync(p => p.Id == pedidoId && p.Activo == true);

                if (pedido == null)
                {
                    return NotFound(new ApiResponse
                    {
                        Exito = false,
                        Mensaje = $"Pedido con ID {pedidoId} no encontrado"
                    });
                }

                // Verificar permisos (el usuario puede pagar su propio pedido o un admin/vendedor puede procesar cualquier pago)
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRoles = User.Claims
                    .Where(c => c.Type == ClaimTypes.Role || c.Type == "rol")
                    .Select(c => c.Value.ToLower())
                    .ToList();

                bool isOwner = int.TryParse(userIdClaim, out int userId) && pedido.UsuarioId == userId;
                bool canProcess = isOwner || userRoles.Contains(Roles.Administrador) || userRoles.Contains(Roles.Vendedor);

                if (!canProcess)
                {
                    return Forbid();
                }

                // Verificar que el pedido no tenga ya un pago aprobado
                var pagoExistente = await _context.Pagos
                    .FirstOrDefaultAsync(p => p.PedidoId == pedidoId && p.Estado == "approved");

                if (pagoExistente != null)
                {
                    return BadRequest(new ApiResponse
                    {
                        Exito = false,
                        Mensaje = "Este pedido ya tiene un pago aprobado"
                    });
                }

                // Crear DTO para el servicio de pagos
                var pagoDto = new PagoCreateDTO
                {
                    PedidoId = pedidoId,
                    Monto = request.Monto,
                    MetodoPago = request.MetodoPago ?? "MERCADOPAGO"
                };

                // Procesar el pago
                var pago = await _pagosService.Create(pagoDto);

                if (pago == null)
                {
                    return BadRequest(new ApiResponse
                    {
                        Exito = false,
                        Mensaje = "Error al procesar el pago"
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Exito = true,
                    Mensaje = "Pago procesado exitosamente",
                    Datos = new
                    {
                        pagoId = pago.Id,
                        pedidoId = pedidoId,
                        monto = pago.Monto,
                        estado = pago.Estado,
                        transaccionId = pago.TransaccionId,
                        fechaPago = pago.FechaPago
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al procesar el pago para el pedido {pedidoId}");
                return StatusCode(500, new ApiResponse
                {
                    Exito = false,
                    Mensaje = "Error interno al procesar el pago",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Webhook para recibir notificaciones de Mercado Pago
        /// </summary>
        [HttpPost("webhook")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Webhook([FromBody] MercadoPagoWebhookRequest data)
        {
            try
            {
                _logger.LogInformation("=== WEBHOOK RECIBIDO ===");
                _logger.LogInformation("Datos recibidos: {Data}", JsonSerializer.Serialize(data));

                if (data == null || string.IsNullOrEmpty(data.Id))
                    return BadRequest(new { exito = false, mensaje = "Datos del webhook inválidos - ID requerido" });

                string paymentId = data.Id;
                string? status = data.Status;
                string? externalReference = data.ExternalReference;

                // --- FLUJO FAKE ---
                if (paymentId.StartsWith("FAKE_"))
                {
                    int pedidoId = 0;
                    decimal monto = 1000;
                    if (!string.IsNullOrEmpty(externalReference))
                        int.TryParse(externalReference, out pedidoId);
                    // Puedes ajustar el monto si lo envías en el webhook
                    var fakePayment = await _fakePaymentService.ProcesarPago(paymentId, monto, pedidoId);

                    // Guardar en base de datos
                    var pago = new Pago
                    {
                        PedidoId = pedidoId,
                        Monto = monto,
                        MetodoPago = "FAKE_PAYMENT",
                        TransaccionId = fakePayment.Id.ToString(),
                        Estado = fakePayment.Status,
                        FechaPago = DateTime.UtcNow,
                        DatosRespuesta = JsonSerializer.Serialize(fakePayment)
                    };
                    _context.Pagos.Add(pago);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("✅ WEBHOOK FAKE PROCESADO - Pedido: {PedidoId}, Transacción: {TransactionId}", pedidoId, fakePayment.Id);
                    return Ok(new { exito = true, mensaje = "Webhook FAKE procesado correctamente", pago = fakePayment });
                }

                // --- FLUJO REAL (MercadoPago) ---
                var pagoMercadoPago = await _mercadoPagoService.ObtenerPago(paymentId);
                if (pagoMercadoPago == null)
                    return BadRequest(new { exito = false, mensaje = "Pago no encontrado en Mercado Pago" });

                // ... resto de la lógica real ...
                return Ok(new { exito = true, mensaje = "Webhook procesado correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar webhook");
                var inner = ex.InnerException?.Message;
                return BadRequest(new { exito = false, mensaje = "Error al procesar webhook", error = ex.Message, inner });
            }
        }

        /// <summary>
        /// Elimina un pago específico por su ID
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var resultado = await _pagosService.Delete(id);
                if (!resultado)
                    return NotFound(new ApiResponse { Exito = false, Mensaje = $"Pago con ID {id} no encontrado" });

                return Ok(new ApiResponse { Exito = true, Mensaje = "Pago eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el pago {id}");
                return StatusCode(500, new ApiResponse
                {
                    Exito = false,
                    Mensaje = "Error al eliminar el pago",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Crea una preferencia de pago de Mercado Pago y devuelve la URL de pago
        /// </summary>
        [HttpPost("preferencia")]
        [Authorize]
        public async Task<IActionResult> CrearPreferenciaPago([FromBody] ProcesarPagoRequest request)
        {
            try
            {
                // Usar sistema FAKE en lugar de MercadoPago real
                var fakePaymentService = HttpContext.RequestServices.GetService<FakePaymentService>();
                
                var pagoDto = new PagoCreateDTO
                {
                    PedidoId = int.Parse(request.DatosAdicionales["pedidoId"]),
                    Monto = request.Monto,
                    MetodoPago = "FAKE_PAYMENT"
                };

                var preferencia = await fakePaymentService.CrearPreferenciaPago(pagoDto);

                return Ok(new
                {
                    exito = true,
                    mensaje = "Preferencia de pago FAKE creada exitosamente",
                    url = preferencia.SandboxInitPoint
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { exito = false, mensaje = "Error al crear preferencia FAKE", error = ex.Message });
            }
        }
    }
}
