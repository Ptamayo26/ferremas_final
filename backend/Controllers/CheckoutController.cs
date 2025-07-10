using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ferremas.Api.DTOs;
using Ferremas.Api.Services.Interfaces;
using Ferremas.Api.Constants;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Ferremas.Api.Data;
using Ferremas.Api.Models;
using System.Security.Claims;
using Ferremas.Api.Services;

namespace Ferremas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckoutController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IDescuentoService _descuentoService;
        private readonly WebpayService _webpayService;
        private readonly ShipitService _shipitService;
        private readonly IEmailService _emailService;

        public CheckoutController(AppDbContext context, IDescuentoService descuentoService, WebpayService webpayService, ShipitService shipitService, IEmailService emailService)
        {
            _context = context;
            _descuentoService = descuentoService;
            _webpayService = webpayService;
            _shipitService = shipitService;
            _emailService = emailService;
        }

        [HttpGet("resumen")]
        public async Task<ActionResult<CheckoutResumenDTO>> GetResumenCheckout()
        {
            try
            {
                var usuarioId = GetUsuarioIdFromToken();
                if (usuarioId == null)
                    return Unauthorized("Usuario no autenticado");

                // Obtener items del carrito
                var carritoItems = await _context.Carrito
                    .Include(c => c.Producto)
                    .Where(c => c.UsuarioId == usuarioId && c.Activo)
                    .Select(c => new CarritoItemDTO
                    {
                        Id = c.Id,
                        ProductoId = c.ProductoId,
                        ProductoNombre = c.Producto.Nombre,
                        ProductoPrecio = c.Producto.Precio,
                        ProductoImagen = c.Producto.ImagenUrl,
                        Cantidad = c.Cantidad,
                        Subtotal = c.Producto.Precio * c.Cantidad,
                        FechaAgregado = c.FechaAgregado
                    })
                    .ToListAsync();

                if (!carritoItems.Any())
                    return BadRequest("El carrito está vacío");

                // Obtener información del cliente
                var cliente = await _context.Clientes
                    .Include(c => c.Usuario)
                    .ThenInclude(u => u.Direcciones)
                    .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

                if (cliente == null)
                    return NotFound("Cliente no encontrado");

                // Calcular totales
                var subtotal = carritoItems.Sum(c => c.Subtotal);
                var descuento = 0m; // Por ahora sin descuentos
                var impuestos = subtotal * 0.19m; // IVA 19%
                var envio = 0m; // Costo de envío se calculará en el frontend
                var total = Math.Round(subtotal + impuestos + envio - descuento, 0); // Redondear a enteros para Webpay CLP

                // Obtener dirección principal
                var direccionEnvio = cliente.Usuario?.Direcciones?.FirstOrDefault(d => d.EsPrincipal == true);

                return Ok(new CheckoutResumenDTO
                {
                    Items = carritoItems,
                    Subtotal = subtotal,
                    Descuento = descuento,
                    Impuestos = impuestos,
                    Envio = envio,
                    Total = total,
                    TotalItems = carritoItems.Sum(c => c.Cantidad),
                    Cliente = new ClienteResumenDTO
                    {
                        Id = cliente.Id,
                        Nombre = cliente.Nombre,
                        Apellido = cliente.Apellido,
                        Rut = cliente.Rut,
                        Email = cliente.CorreoElectronico,
                        Telefono = cliente.Telefono
                    },
                    DireccionEnvio = direccionEnvio != null ? new DireccionDTO
                    {
                        Id = direccionEnvio.Id,
                        Calle = direccionEnvio.Calle,
                        Numero = direccionEnvio.Numero,
                        Departamento = direccionEnvio.Departamento,
                        Comuna = direccionEnvio.Comuna,
                        Region = direccionEnvio.Region,
                        CodigoPostal = direccionEnvio.CodigoPostal,
                        EsPrincipal = direccionEnvio.EsPrincipal,
                        FechaModificacion = direccionEnvio.FechaModificacion
                    } : new DireccionDTO()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<CheckoutResponseDTO>> ProcesarCheckout([FromBody] CheckoutRequestDTO? dto)
        {
            if (dto == null)
            {
                Console.WriteLine("[Checkout] Error: DTO es null");
                return BadRequest("Datos de checkout requeridos");
            }

            Console.WriteLine($"[Checkout] Iniciando procesamiento de checkout - Usuario: {GetUsuarioIdFromToken()?.ToString() ?? "Anónimo"}");
            Console.WriteLine($"[Checkout] ClienteId recibido: {dto.ClienteId?.ToString() ?? "NULL"}");
            Console.WriteLine($"[Checkout] MetodoPago: {dto.MetodoPago}");
            Console.WriteLine($"[Checkout] Items count: {dto.Items?.Count ?? 0}");
            Console.WriteLine($"[Checkout] Datos completos: {System.Text.Json.JsonSerializer.Serialize(dto)}");
            
            try
            {
                var usuarioId = GetUsuarioIdFromToken();
                List<CarritoItemDTO> carritoItems;
                Cliente cliente = null;
                Direccion direccion = null;

                // Usar SIEMPRE los productos enviados en dto.Items
                if (dto.Items == null || dto.Items.Count == 0)
                    return BadRequest("No se enviaron productos para el pedido.");
                carritoItems = dto.Items;
                Console.WriteLine($"[Checkout] carritoItems recibidos: {System.Text.Json.JsonSerializer.Serialize(carritoItems)}");
                Console.WriteLine($"[Checkout] Código de descuento recibido: {dto.CodigoDescuento}");

                if (usuarioId == null) // Pedido anónimo
                {
                    // Crear cliente temporal
                    cliente = new Cliente
                    {
                        Nombre = dto.Rut ?? "Cliente Anónimo", // Usar RUT como nombre si está disponible
                        Apellido = "",
                        Rut = dto.Rut ?? "",
                        CorreoElectronico = dto.Correo ?? "anonimo@ferremas.cl",
                        Telefono = "",
                        FechaCreacion = DateTime.UtcNow,
                        Activo = true
                    };
                    _context.Clientes.Add(cliente);
                    await _context.SaveChangesAsync();

                    // Crear dirección temporal con datos del DTO
                    direccion = new Direccion
                    {
                        Calle = dto.Calle ?? "Sin dirección",
                        Numero = dto.Numero ?? "S/N",
                        Departamento = dto.Departamento ?? "",
                        Comuna = dto.Comuna ?? "",
                        Region = dto.Region ?? "",
                        CodigoPostal = dto.CodigoPostal ?? "",
                        EsPrincipal = true,
                        FechaCreacion = DateTime.UtcNow,
                        FechaModificacion = DateTime.UtcNow,
                        ClienteId = cliente.Id, // Asignar el cliente creado
                        UsuarioId = null // Para pedidos anónimos, no hay usuario
                    };
                    _context.Direcciones.Add(direccion);
                    await _context.SaveChangesAsync();
                }
                else // Pedido autenticado
                {
                    // SIEMPRE buscar el cliente por usuarioId
                    cliente = await _context.Clientes
                        .Include(c => c.Usuario)
                        .ThenInclude(u => u.Direcciones)
                        .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

                    if (cliente == null)
                        return NotFound("Cliente no encontrado para el usuario autenticado");

                    // Si se envía DireccionId, usar la lógica actual
                    if (dto.DireccionId != null && dto.DireccionId > 0)
                    {
                        direccion = cliente.Usuario?.Direcciones?.FirstOrDefault(d => d.Id == dto.DireccionId);
                        if (direccion == null)
                            return NotFound("Dirección de envío no encontrada");
                    }
                    else
                    {
                        // Buscar si ya existe una dirección igual para este usuario (normalizando)
                        direccion = cliente.Usuario?.Direcciones?.FirstOrDefault(d =>
                            (d.Calle ?? "").Trim().ToLower() == (dto.Calle ?? "Sin dirección").Trim().ToLower() &&
                            (d.Numero ?? "").Trim().ToLower() == (dto.Numero ?? "S/N").Trim().ToLower() &&
                            ((d.Departamento ?? "").Trim().ToLower() == (dto.Departamento ?? "").Trim().ToLower()) &&
                            (d.Comuna ?? "").Trim().ToLower() == (dto.Comuna ?? "").Trim().ToLower() &&
                            (d.Region ?? "").Trim().ToLower() == (dto.Region ?? "").Trim().ToLower()
                        );
                        if (direccion == null)
                        {
                            // Si no existe, crearla
                            direccion = new Direccion
                            {
                                Calle = dto.Calle ?? "Sin dirección",
                                Numero = dto.Numero ?? "S/N",
                                Departamento = dto.Departamento ?? "",
                                Comuna = dto.Comuna ?? "",
                                Region = dto.Region ?? "",
                                CodigoPostal = dto.CodigoPostal ?? "",
                                EsPrincipal = false,
                                FechaCreacion = DateTime.UtcNow,
                                FechaModificacion = DateTime.UtcNow,
                                ClienteId = cliente.Id,
                                UsuarioId = usuarioId
                            };
                            _context.Direcciones.Add(direccion);
                            await _context.SaveChangesAsync();
                        }
                    }
                }

                // Calcular subtotal base (sin descuentos)
                var subtotalBase = carritoItems.Sum(c => (c.PrecioOriginal > 0 ? c.PrecioOriginal : c.ProductoPrecio) * c.Cantidad);
                // El subtotal con descuentos ya incluye IVA
                var totalConIVA = carritoItems.Sum(c => (c.PrecioConDescuento > 0 ? c.PrecioConDescuento : c.PrecioOriginal > 0 ? c.PrecioOriginal : c.ProductoPrecio) * c.Cantidad);
                // Descuento total aplicado (base + cupón)
                var descuentoTotal = subtotalBase - totalConIVA;
                decimal descuentoCupon = 0m;
                if (!string.IsNullOrEmpty(dto.CodigoDescuento))
                {
                    var desc = await _descuentoService.ObtenerPorCodigo(dto.CodigoDescuento);
                    if (desc != null && desc.Activo)
                    {
                        if (desc.Tipo == "porcentaje")
                        {
                            descuentoCupon = Math.Round(totalConIVA * (desc.Valor / 100m), 0);
                        }
                        else if (desc.Tipo == "monto")
                        {
                            descuentoCupon = Math.Min(desc.Valor, totalConIVA);
                        }
                        else
                        {
                            descuentoCupon = 0m;
                        }
                    }
                }
                var costoEnvio = dto.CostoEnvio ?? 0m;
                var totalFinal = totalConIVA - descuentoCupon + costoEnvio;
                // Calcular IVA sobre el total final (aprox)
                var iva = Math.Round(totalFinal / 1.19m * 0.19m, 0);
                // Guardar estos valores en el pedido y retornarlos en la respuesta

                // LOGS DETALLADOS DE TOTALES
                Console.WriteLine($"[CHECKOUT] subtotalBase (sin descuentos): {subtotalBase}");
                Console.WriteLine($"[CHECKOUT] totalConIVA (con descuentos base): {totalConIVA}");
                Console.WriteLine($"[CHECKOUT] descuentoTotal (base): {descuentoTotal}");
                Console.WriteLine($"[CHECKOUT] descuentoCupon: {descuentoCupon}");
                Console.WriteLine($"[CHECKOUT] costoEnvio: {costoEnvio}");
                Console.WriteLine($"[CHECKOUT] totalFinal (total pagado): {totalFinal}");

                // Crear el pedido
                var pedido = new Pedido
                {
                    UsuarioId = usuarioId,
                    ClienteId = cliente.Id,
                    FechaPedido = DateTime.UtcNow,
                    Total = totalFinal,
                    Estado = "PENDIENTE",
                    Observaciones = (dto.Observaciones ?? "") + $" | RUT: {dto.Rut ?? ""} | Correo: {dto.Correo ?? ""}",
                    DireccionEntrega = direccion != null ? $"{direccion.Calle} {direccion.Numero}, {direccion.Comuna}, {direccion.Region}" : "",
                    FechaCreacion = DateTime.UtcNow,
                    FechaModificacion = DateTime.UtcNow,
                    Activo = true,
                    Detalles = carritoItems.Select(c => new DetallePedido
                    {
                        ProductoId = c.ProductoId,
                        Cantidad = c.Cantidad,
                        PrecioUnitario = (c.PrecioConDescuento > 0 ? c.PrecioConDescuento : c.PrecioOriginal > 0 ? c.PrecioOriginal : c.ProductoPrecio),
                        Subtotal = (c.PrecioConDescuento > 0 ? c.PrecioConDescuento : c.PrecioOriginal > 0 ? c.PrecioOriginal : c.ProductoPrecio) * c.Cantidad,
                        Observaciones = $"precioOriginal:{(c.PrecioOriginal > 0 ? c.PrecioOriginal : c.ProductoPrecio)},precioConDescuento:{(c.PrecioConDescuento > 0 ? c.PrecioConDescuento : 0)}"
                    }).ToList()
                };

                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync();

                // Guardar datos de empresa si corresponde
                if (dto.TipoDocumento == "factura" && dto.DatosEmpresa != null)
                {
                    var datosEmpresa = new DatosFacturaEmpresa
                    {
                        PedidoId = pedido.Id,
                        RazonSocial = dto.DatosEmpresa.RazonSocial,
                        Rut = dto.DatosEmpresa.Rut,
                        Giro = dto.DatosEmpresa.Giro,
                        Direccion = dto.DatosEmpresa.Direccion
                    };
                    _context.Add(datosEmpresa);
                    await _context.SaveChangesAsync();
                }

                // Generar número de pedido
                var numeroPedido = $"PED-{pedido.Id:D6}";
                pedido.NumeroPedido = numeroPedido;
                await _context.SaveChangesAsync();

                // Crear el envío en Shipit
                var shipitRequest = new ShipitEnvioRequestDto
                {
                    Courier = "chilexpress",
                    NombreDestinatario = dto.Rut ?? "Cliente Ferremas",
                    Direccion = direccion != null ? $"{direccion.Calle} {direccion.Numero}" : dto.Calle ?? "",
                    ComunaDestino = direccion?.Comuna ?? dto.Comuna ?? "Santiago",
                    Correo = dto.Correo ?? "cliente@ferremas.cl",
                    Telefono = dto.Telefono ?? "+56912345678",
                    ItemsCount = carritoItems.Sum(c => c.Cantidad),
                    Largo = 20,
                    Ancho = 20,
                    Alto = 20,
                    Peso = 2000, // 2kg por defecto
                    ValorDeclarado = (int)totalFinal,
                    Referencia = numeroPedido,
                    Contenido = "Productos Ferremas"
                };

                Console.WriteLine($"[CHECKOUT] Creando envío en Shipit: {System.Text.Json.JsonSerializer.Serialize(shipitRequest)}");
                
                var shipitResponse = await _shipitService.CrearEnvioAsync(shipitRequest);
                Console.WriteLine($"[CHECKOUT] Respuesta de Shipit: {System.Text.Json.JsonSerializer.Serialize(shipitResponse)}");
                
                if (!shipitResponse.Success)
                {
                    Console.WriteLine($"[CHECKOUT] Advertencia: Shipit falló pero continuando con el proceso. Error: {shipitResponse.Error}");
                }

                // Crear el envío local
                var envio = new Envio
                {
                    PedidoId = pedido.Id,
                    DireccionEnvio = direccion != null ? $"{direccion.Calle} {direccion.Numero}, {direccion.Comuna}, {direccion.Region}" : "",
                    ProveedorTransporte = "Chilexpress",
                    EstadoEnvio = shipitResponse.Success ? "EN_PREPARACION" : "PENDIENTE",
                    ComunaDestino = direccion?.Comuna ?? "",
                    RegionDestino = direccion?.Region ?? "",
                    NombreDestinatario = dto.Rut ?? "",
                    Rut = dto.Rut ?? "",
                    Correo = dto.Correo ?? "",
                    FechaEnvio = DateTime.UtcNow,
                    FechaCreacion = DateTime.UtcNow,
                    Pedido = pedido,
                    TrackingNumber = shipitResponse.TrackingNumber,
                    CostoEnvio = shipitResponse.Cost ?? 0
                };
                _context.Envios.Add(envio);
                await _context.SaveChangesAsync();

                Console.WriteLine($"[CHECKOUT] Envío creado - Tracking: {shipitResponse.TrackingNumber}, Costo: {shipitResponse.Cost}");

                // Enviar email de confirmación
                var emailEnviado = await _emailService.EnviarConfirmacionPedido(
                    dto.Correo ?? cliente?.CorreoElectronico ?? "cliente@ferremas.cl",
                    numeroPedido,
                    totalFinal,
                    shipitResponse.TrackingNumber
                );
                Console.WriteLine($"[CHECKOUT] Email de confirmación enviado: {emailEnviado}");

                // Limpiar carrito solo si es autenticado
                if (usuarioId != null)
                {
                    var carrito = await _context.Carrito.Where(c => c.UsuarioId == usuarioId && c.Activo).ToListAsync();
                    foreach (var item in carrito)
                    {
                        item.Activo = false;
                    }
                    await _context.SaveChangesAsync();
                }

                // Generar transacción Webpay
                var buyOrder = pedido.Id.ToString();
                var sessionId = (usuarioId?.ToString() ?? "anon") + "-" + DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var returnUrl = "http://localhost:3000/confirmacion-pago";
                
                try
                {
                    Console.WriteLine($"[Checkout] Generando transacción Webpay - Monto: {pedido.Total}, BuyOrder: {buyOrder}, SessionId: {sessionId}");
                    var webpayResp = _webpayService.CrearTransaccion((decimal)pedido.Total, buyOrder, sessionId, returnUrl);
                    Console.WriteLine($"[Checkout] Transacción Webpay generada - URL: {webpayResp.Url}, Token: {webpayResp.Token}");
                    
                    // Guardar el pago en la base de datos
                    var pago = new Pago
                    {
                        PedidoId = pedido.Id,
                        Monto = (decimal)pedido.Total,
                        FechaPago = DateTime.UtcNow,
                        Estado = "PENDIENTE",
                        MetodoPago = "WEBPAY",
                        TransaccionId = webpayResp.Token,
                        TokenPasarela = webpayResp.Token,
                        DatosRespuesta = System.Text.Json.JsonSerializer.Serialize(webpayResp)
                    };
                    _context.Pagos.Add(pago);
                    await _context.SaveChangesAsync();

                    // Log de productos y precios enviados al frontend
                    foreach (var c in carritoItems)
                    {
                        var precioFinal = (c.PrecioConDescuento > 0 ? c.PrecioConDescuento : c.PrecioOriginal > 0 ? c.PrecioOriginal : c.ProductoPrecio);
                        Console.WriteLine($"[CHECKOUT] Producto: {c.ProductoNombre}, Precio enviado: {precioFinal}");
                    }
                    // Justo antes del return Ok(new CheckoutResponseDTO ...)
                    Console.WriteLine($"[CHECKOUT] Retornando DTO con Total: {totalFinal}");
                    var productosDTO = carritoItems.Select(c => new ProductoResumenDTO {
                        Nombre = c.ProductoNombre,
                        Cantidad = c.Cantidad,
                        Precio = (c.PrecioConDescuento > 0 ? c.PrecioConDescuento : c.PrecioOriginal > 0 ? c.PrecioOriginal : c.ProductoPrecio),
                        PrecioOriginal = (c.PrecioOriginal > 0 ? c.PrecioOriginal : c.ProductoPrecio),
                        PrecioConDescuento = (c.PrecioConDescuento > 0 ? c.PrecioConDescuento : 0)
                    }).ToList();

                    foreach (var p in productosDTO)
                    {
                        Console.WriteLine($"[DEBUG DTO SERIALIZADO] {p.Nombre} - PrecioOriginal: {p.PrecioOriginal}, PrecioConDescuento: {p.PrecioConDescuento}");
                    }
                    Console.WriteLine("[DEBUG JSON] " + Newtonsoft.Json.JsonConvert.SerializeObject(productosDTO));

                    return Ok(new CheckoutResponseDTO
                    {
                        PedidoId = pedido.Id,
                        NumeroPedido = numeroPedido,
                        Total = totalFinal,
                        Estado = pedido.Estado,
                        FechaCreacion = pedido.FechaCreacion,
                        UrlPago = webpayResp.Url,
                        CodigoPago = webpayResp.Token,
                        Productos = productosDTO,
                        Subtotal = subtotalBase,
                        DescuentoBase = descuentoTotal,
                        DescuentoCupon = descuentoCupon,
                        Impuestos = iva,
                        Envio = costoEnvio,
                        TotalFinal = totalFinal
                    });
                }
                catch (Exception webpayEx)
                {
                    // Si falla Webpay, marcar el pedido como fallido pero no fallar completamente
                    pedido.Estado = "FALLIDO";
                    await _context.SaveChangesAsync();
                    
                    return BadRequest($"Error al generar transacción Webpay: {webpayEx.Message}");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("test")]
        [AllowAnonymous]
        public async Task<ActionResult> TestCheckout()
        {
            try
            {
                Console.WriteLine("[TEST CHECKOUT] Iniciando prueba de checkout");
                
                // Crear datos de prueba
                var testDto = new CheckoutRequestDTO
                {
                    ClienteId = 1,
                    DireccionId = 1,
                    MetodoPago = "efectivo",
                    Observaciones = "Prueba de checkout",
                    TipoDocumento = "boleta",
                    Items = new List<CarritoItemDTO>
                    {
                        new CarritoItemDTO
                        {
                            ProductoId = 1,
                            ProductoNombre = "Martillo",
                            ProductoPrecio = 15000,
                            Cantidad = 1,
                            Subtotal = 15000,
                            PrecioOriginal = 15000,
                            PrecioConDescuento = 0
                        }
                    },
                    Rut = "12345678-9",
                    Correo = "test@ferremas.cl",
                    Calle = "Av. Test 123",
                    Numero = "456",
                    Comuna = "Santiago",
                    Region = "Metropolitana",
                    CostoEnvio = 5000
                };

                Console.WriteLine("[TEST CHECKOUT] DTO creado, procesando...");
                
                // Llamar al método de procesamiento
                var result = await ProcesarCheckout(testDto);
                
                Console.WriteLine("[TEST CHECKOUT] Procesamiento completado");
                return Ok(new { message = "Test completado", result = result });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TEST CHECKOUT] Error: {ex.Message}");
                return BadRequest($"Error en test: {ex.Message}");
            }
        }

        private int? GetUsuarioIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
                return userId;
            return null;
        }
    }
} 