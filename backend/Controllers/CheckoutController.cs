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

namespace Ferremas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckoutController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CheckoutController(AppDbContext context)
        {
            _context = context;
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
                var envio = 0m; // Por ahora sin costo de envío
                var total = subtotal + impuestos + envio - descuento;

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
                        EsPrincipal = direccionEnvio.EsPrincipal ?? false,
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
        public async Task<ActionResult<CheckoutResponseDTO>> ProcesarCheckout([FromBody] CheckoutRequestDTO dto)
        {
            try
            {
                var usuarioId = GetUsuarioIdFromToken();
                List<CarritoItemDTO> carritoItems;
                Cliente cliente = null;
                Direccion direccion = null;

                if (usuarioId == null) // Pedido anónimo
                {
                    // Crear cliente temporal
                    cliente = new Cliente
                    {
                        Nombre = "Cliente Anónimo",
                        Apellido = "",
                        Rut = "",
                        CorreoElectronico = "anonimo@ferremas.cl",
                        Telefono = "",
                        FechaCreacion = DateTime.UtcNow,
                        Activo = true
                    };
                    _context.Clientes.Add(cliente);
                    await _context.SaveChangesAsync();

                    // Crear dirección temporal
                    direccion = new Direccion
                    {
                        Calle = "Sin dirección",
                        Numero = "S/N",
                        Departamento = "",
                        Comuna = "",
                        Region = "",
                        CodigoPostal = "",
                        EsPrincipal = true,
                        FechaCreacion = DateTime.UtcNow,
                        FechaModificacion = DateTime.UtcNow,
                        ClienteId = cliente.Id, // Asignar el cliente creado
                        UsuarioId = null // Para pedidos anónimos, no hay usuario
                    };
                    _context.Direcciones.Add(direccion);
                    await _context.SaveChangesAsync();

                    // Usar los productos enviados en el DTO
                    if (dto.Items == null || dto.Items.Count == 0)
                        return BadRequest("No se enviaron productos para el pedido anónimo.");
                    carritoItems = dto.Items;
                }
                else // Pedido autenticado
                {
                    // Verificar que el carrito no esté vacío
                    var carrito = await _context.Carrito
                        .Include(c => c.Producto)
                        .Where(c => c.UsuarioId == usuarioId && c.Activo)
                        .ToListAsync();

                    if (!carrito.Any())
                        return BadRequest("El carrito está vacío");

                    carritoItems = carrito.Select(c => new CarritoItemDTO
                    {
                        Id = c.Id,
                        ProductoId = c.ProductoId,
                        ProductoNombre = c.Producto.Nombre,
                        ProductoPrecio = c.Producto.Precio,
                        ProductoImagen = c.Producto.ImagenUrl,
                        Cantidad = c.Cantidad,
                        Subtotal = c.Producto.Precio * c.Cantidad,
                        FechaAgregado = c.FechaAgregado
                    }).ToList();

                    // Verificar stock de todos los productos
                    foreach (var item in carrito)
                    {
                        if (item.Producto.Stock < item.Cantidad)
                            return BadRequest($"Stock insuficiente para {item.Producto.Nombre}. Disponible: {item.Producto.Stock}");
                    }

                    // Verificar que el cliente existe
                    cliente = await _context.Clientes
                        .Include(c => c.Usuario)
                        .ThenInclude(u => u.Direcciones)
                        .FirstOrDefaultAsync(c => c.Id == dto.ClienteId);

                    if (cliente == null)
                        return NotFound("Cliente no encontrado");

                    // Verificar que la dirección existe y pertenece al cliente
                    direccion = cliente.Usuario?.Direcciones?.FirstOrDefault(d => d.Id == dto.DireccionId);
                    if (direccion == null)
                        return NotFound("Dirección de envío no encontrada");
                }

                // Calcular totales
                var subtotal = carritoItems.Sum(c => c.ProductoPrecio * c.Cantidad);
                var descuento = 0m; // Por ahora sin descuentos
                var impuestos = subtotal * 0.19m; // IVA 19%
                var envio = 0m; // Por ahora sin costo de envío
                var total = subtotal + impuestos + envio - descuento;

                // Crear el pedido
                var pedido = new Pedido
                {
                    UsuarioId = usuarioId,
                    FechaPedido = DateTime.UtcNow,
                    Total = total,
                    Estado = "PENDIENTE",
                    Observaciones = dto.Observaciones,
                    DireccionEntrega = direccion != null ? $"{direccion.Calle} {direccion.Numero}, {direccion.Comuna}, {direccion.Region}" : "",
                    FechaCreacion = DateTime.UtcNow,
                    FechaModificacion = DateTime.UtcNow,
                    Activo = true,
                    Detalles = carritoItems.Select(c => new DetallePedido
                    {
                        ProductoId = c.ProductoId,
                        Cantidad = c.Cantidad,
                        PrecioUnitario = c.ProductoPrecio,
                        Subtotal = c.ProductoPrecio * c.Cantidad,
                        Observaciones = null
                    }).ToList()
                };

                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync();

                // Generar número de pedido
                var numeroPedido = $"PED-{pedido.Id:D6}";
                pedido.NumeroPedido = numeroPedido;
                await _context.SaveChangesAsync();

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

                return Ok(new CheckoutResponseDTO
                {
                    PedidoId = pedido.Id,
                    NumeroPedido = numeroPedido,
                    Total = total,
                    Estado = pedido.Estado,
                    FechaCreacion = pedido.FechaCreacion,
                    UrlPago = null, // Por ahora sin integración de pagos
                    CodigoPago = null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private int? GetUsuarioIdFromToken()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (int.TryParse(userIdClaim, out int userId))
                return userId;
            return null;
        }
    }
} 