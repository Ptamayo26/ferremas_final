using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Ferremas.Api.Models;
using Ferremas.Api.Data;
using Ferremas.Api.DTOs;
using Ferremas.Api.Services.Interfaces;
using Ferremas.Api.Constants;

namespace Ferremas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 🔐 Todos los endpoints requieren autenticación
    public class PedidosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPedidosService _pedidosService;

        public PedidosController(AppDbContext context, IPedidosService pedidosService)
        {
            _context = context;
            _pedidosService = pedidosService;
        }

        // GET: api/pedidos
        [HttpGet]
        [Authorize(Roles = "administrador,vendedor,bodeguero")]
        public async Task<ActionResult<IEnumerable<PedidoResponseDTO>>> GetPedidos()
        {
            try
            {
                var pedidos = await _context.Pedidos
                    .Include(p => p.Usuario)
                    .Include(p => p.Detalles)
                        .ThenInclude(d => d.Producto)
                    .Where(p => p.Activo == true)
                    .Select(p => new PedidoResponseDTO
                    {
                        Id = p.Id,
                        UsuarioId = p.UsuarioId ?? 0,
                        UsuarioNombre = p.Usuario != null ? p.Usuario.Nombre : (p.NombreClienteTemporal ?? "Anónimo"),
                        NombreClienteTemporal = p.NombreClienteTemporal,
                        FechaPedido = p.FechaPedido ?? DateTime.MinValue,
                        FechaCreacion = p.FechaCreacion,
                        Estado = p.Estado ?? "Pendiente",
                        Total = p.Total ?? 0,
                        Observaciones = p.Observaciones,
                        DireccionEntrega = p.DireccionEntrega,
                        FechaModificacion = p.FechaModificacion,
                        Activo = p.Activo,
                        Detalles = p.Detalles.Select(d => new DetallePedidoResponseDTO
                        {
                            Id = d.Id,
                            ProductoId = d.ProductoId ?? 0,
                            ProductoNombre = d.Producto != null ? d.Producto.Nombre : "Producto desconocido",
                            Cantidad = d.Cantidad ?? 0,
                            PrecioUnitario = d.PrecioUnitario ?? 0,
                            Subtotal = d.Subtotal ?? 0,
                            Observaciones = d.Observaciones
                        }).ToList()
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<IEnumerable<PedidoResponseDTO>>
                {
                    Exito = true,
                    Mensaje = "Pedidos obtenidos exitosamente",
                    Datos = pedidos
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetPedidos: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return StatusCode(500, new { 
                    exito = false,
                    error = "Error al obtener pedidos",
                    detalles = ex.Message 
                });
            }
        }

        // GET: api/pedidos/mis
        [HttpGet("mis")]
        [Authorize] // Cualquier usuario autenticado puede ver sus propios pedidos
        public async Task<ActionResult<IEnumerable<PedidoResponseDTO>>> GetMisPedidos()
        {
            try
            {
                // Obtener el ID del usuario desde el token JWT
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new ApiResponse
                    { 
                        Exito = false,
                        Mensaje = "Usuario no autenticado o token inválido" 
                    });
                }

                // Buscar el cliente asociado a este usuario
                var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UsuarioId == userId && c.Activo == true);
                if (cliente == null)
                {
                    return Ok(new { exito = true, mensaje = "Sin pedidos", total = 0, datos = new List<PedidoResponseDTO>() });
                }
                
                var pedidos = await _context.Pedidos
                    .Include(p => p.Usuario)
                    .Include(p => p.Detalles)
                        .ThenInclude(d => d.Producto)
                    .Where(p => p.UsuarioId == cliente.Id && p.Activo == true)
                    .Select(p => new PedidoResponseDTO
                    {
                        Id = p.Id,
                        UsuarioId = p.UsuarioId ?? 0,
                        UsuarioNombre = p.Usuario != null ? p.Usuario.Nombre : (p.NombreClienteTemporal ?? "Anónimo"),
                        NombreClienteTemporal = p.NombreClienteTemporal,
                        FechaPedido = p.FechaPedido ?? DateTime.MinValue,
                        FechaCreacion = p.FechaCreacion,
                        Estado = p.Estado ?? "Pendiente",
                        Total = p.Total ?? 0,
                        Observaciones = p.Observaciones,
                        DireccionEntrega = p.DireccionEntrega,
                        FechaModificacion = p.FechaModificacion,
                        Activo = p.Activo,
                        Detalles = p.Detalles.Select(d => new DetallePedidoResponseDTO
                        {
                            Id = d.Id,
                            ProductoId = d.ProductoId ?? 0,
                            ProductoNombre = d.Producto != null ? d.Producto.Nombre : "Producto desconocido",
                            Cantidad = d.Cantidad ?? 0,
                            PrecioUnitario = d.PrecioUnitario ?? 0,
                            Subtotal = d.Subtotal ?? 0,
                            Observaciones = d.Observaciones
                        }).ToList()
                    })
                    .OrderByDescending(p => p.FechaCreacion)
                    .ToListAsync();

                return Ok(new
                {
                    exito = true,
                    mensaje = "Mis pedidos obtenidos exitosamente",
                    total = pedidos.Count,
                    datos = pedidos
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetMisPedidos: {ex.Message}");
                return StatusCode(500, new { 
                    exito = false,
                    error = "Error al obtener mis pedidos",
                    detalles = ex.Message 
                });
            }
        }

        // GET: api/pedidos/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PedidoResponseDTO>> GetPedido(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new ApiResponse
                    { 
                        Exito = false,
                        Mensaje = "Usuario no autenticado o token inválido" 
                    });
                }
                
                var pedido = await _context.Pedidos
                    .Include(p => p.Usuario)
                    .Include(p => p.Detalles)
                        .ThenInclude(d => d.Producto)
                    .FirstOrDefaultAsync(p => p.Id == id && p.Activo == true);

                if (pedido == null)
                {
                    return NotFound(new ApiResponse
                    { 
                        Exito = false,
                        Mensaje = "Pedido no encontrado" 
                    });
                }

                // Verificar permisos
                var userRoles = User.Claims
                    .Where(c => c.Type == ClaimTypes.Role || c.Type == "rol")
                    .Select(c => c.Value.ToLower())
                    .ToList();

                bool isAdmin = userRoles.Contains("administrador");
                bool isVendedor = userRoles.Contains("vendedor");
                bool isRepartidor = userRoles.Contains("repartidor");
                bool isOwner = pedido.UsuarioId == userId;

                // Solo el propietario, admin, vendedor o repartidor asignado pueden ver el pedido
                if (!isOwner && !isAdmin && !isVendedor && !(isRepartidor && pedido.RepartidorId == userId))
                {
                    return Forbid();
                }

                var result = new PedidoResponseDTO
                {
                    Id = pedido.Id,
                    UsuarioId = pedido.UsuarioId ?? 0,
                    UsuarioNombre = pedido.Usuario != null ? pedido.Usuario.Nombre : (pedido.NombreClienteTemporal ?? "Anónimo"),
                    NombreClienteTemporal = pedido.NombreClienteTemporal,
                    FechaPedido = pedido.FechaPedido ?? DateTime.MinValue,
                    FechaCreacion = pedido.FechaCreacion,
                    Estado = pedido.Estado ?? "Pendiente",
                    Total = pedido.Total ?? 0,
                    Observaciones = pedido.Observaciones,
                    DireccionEntrega = pedido.DireccionEntrega,
                    FechaModificacion = pedido.FechaModificacion,
                    Activo = pedido.Activo,
                    Detalles = pedido.Detalles.Select(d => new DetallePedidoResponseDTO
                    {
                        Id = d.Id,
                        ProductoId = d.ProductoId ?? 0,
                        ProductoNombre = d.Producto != null ? d.Producto.Nombre : "Producto desconocido",
                        Cantidad = d.Cantidad ?? 0,
                        PrecioUnitario = d.PrecioUnitario ?? 0,
                        Subtotal = d.Subtotal ?? 0,
                        Observaciones = d.Observaciones
                    }).ToList()
                };

                return Ok(new
                {
                    exito = true,
                    mensaje = "Pedido obtenido exitosamente",
                    datos = result
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetPedido: {ex.Message}");
                return StatusCode(500, new { 
                    exito = false,
                    error = "Error al obtener el pedido",
                    detalles = ex.Message 
                });
            }
        }

        // POST: api/pedidos
        [HttpPost]
        [Authorize(Roles = "administrador,vendedor")]
        public async Task<ActionResult<PedidoResponseDTO>> CrearPedido(PedidoCreateDTO pedidoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse
                    {
                        Exito = false,
                        Mensaje = "Datos de pedido inválidos",
                        Error = string.Join(", ", ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage))
                    });
                }

                // Validar que los productos existan y tengan stock suficiente
                foreach (var detalle in pedidoDto.Detalles)
                {
                    var producto = await _context.Productos.FindAsync(detalle.ProductoId);
                    if (producto == null)
                    {
                        return BadRequest(new ApiResponse
                        {
                            Exito = false,
                            Mensaje = $"El producto con ID {detalle.ProductoId} no existe"
                        });
                    }
                    
                    if (!producto.Activo)
                    {
                        return BadRequest(new ApiResponse
                        {
                            Exito = false,
                            Mensaje = $"El producto {producto.Nombre} no está disponible"
                        });
                    }
                    
                    if (producto.Stock < detalle.Cantidad)
                    {
                        return BadRequest(new ApiResponse
                        {
                            Exito = false,
                            Mensaje = $"Stock insuficiente para el producto {producto.Nombre}. Stock disponible: {producto.Stock}"
                        });
                    }
                }

                var pedido = new Pedido
                {
                    UsuarioId = pedidoDto.UsuarioId,
                    FechaPedido = DateTime.UtcNow,
                    FechaCreacion = DateTime.UtcNow,
                    Estado = pedidoDto.Estado,
                    Total = pedidoDto.Total,
                    Observaciones = pedidoDto.Observaciones,
                    DireccionEntrega = pedidoDto.DireccionEntrega,
                    Activo = true,
                    Detalles = pedidoDto.Detalles.Select(d => new DetallePedido
                    {
                        ProductoId = d.ProductoId,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                        Subtotal = d.Cantidad * d.PrecioUnitario,
                        Observaciones = d.Observaciones
                    }).ToList(),
                    NombreClienteTemporal = pedidoDto.NombreClienteTemporal
                };

                // Actualizar stock de productos
                foreach (var detalle in pedido.Detalles)
                {
                    var producto = await _context.Productos.FindAsync(detalle.ProductoId);
                    if (producto != null)
                    {
                        producto.Stock -= detalle.Cantidad ?? 0;
                        producto.FechaModificacion = DateTime.UtcNow;
                    }
                }

                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetPedido), new { id = pedido.Id }, new
                {
                    exito = true,
                    mensaje = "Pedido creado exitosamente",
                    datos = new { pedidoId = pedido.Id }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CrearPedido: {ex.Message}");
                return StatusCode(500, new { 
                    exito = false,
                    error = "Error al crear el pedido",
                    detalles = ex.Message 
                });
            }
        }

        // PUT: api/pedidos/{id}/estado
        [HttpPut("{id}/estado")]
        [Authorize(Roles = "administrador,vendedor,bodeguero")]
        public async Task<IActionResult> ActualizarEstado(int id, [FromBody] PedidoUpdateEstadoDTO estadoDto)
        {
            try
            {
                if (estadoDto == null || string.IsNullOrWhiteSpace(estadoDto.Estado))
                {
                    return BadRequest(new ApiResponse
                    {
                        Exito = false,
                        Mensaje = "El estado es requerido"
                    });
                }

                var pedido = await _context.Pedidos.FirstOrDefaultAsync(p => p.Id == id);

                if (pedido == null || !pedido.Activo)
                {
                    return NotFound(new ApiResponse
                    { 
                        Exito = false,
                        Mensaje = "Pedido no encontrado" 
                    });
                }

                var userRoles = User.Claims
                    .Where(c => c.Type == ClaimTypes.Role || c.Type == "rol")
                    .Select(c => c.Value.ToLower())
                    .ToList();

                // Validar estados permitidos según el rol
                var estadosValidos = userRoles.Contains("repartidor")
                    ? new[] { "En Proceso", "Entregado" }
                    : new[] { "Pendiente", "En Proceso", "Enviado", "Entregado", "Cancelado" };

                if (!estadosValidos.Contains(estadoDto.Estado))
                {
                    return BadRequest(new ApiResponse
                    { 
                        Exito = false,
                        Mensaje = "Estado no válido para tu rol",
                        Error = $"Estados permitidos: {string.Join(", ", estadosValidos)}"
                    });
                }

                // Si es repartidor, solo puede actualizar sus propios pedidos
                if (userRoles.Contains("repartidor"))
                {
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (int.TryParse(userIdClaim, out int userId) && pedido.RepartidorId != userId)
                    {
                        return Forbid();
                    }
                }

                pedido.Estado = estadoDto.Estado;
                pedido.FechaModificacion = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                { 
                    Exito = true,
                    Mensaje = "Estado actualizado correctamente",
                    Datos = new { 
                        pedidoId = id,
                        nuevoEstado = estadoDto.Estado,
                        fechaActualizacion = pedido.FechaModificacion
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ActualizarEstado: {ex.Message}");
                return StatusCode(500, new { 
                    exito = false,
                    error = "Error al actualizar estado",
                    detalles = ex.Message 
                });
            }
        }

        // DELETE: api/pedidos/{id}
        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireAdministrador")]
        public async Task<IActionResult> DeletePedido(int id)
        {
            try
            {
                var pedido = await _context.Pedidos
                    .Include(p => p.Detalles)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (pedido == null)
                {
                    return NotFound(new ApiResponse
                    { 
                        Exito = false,
                        Mensaje = "Pedido no encontrado" 
                    });
                }

                // Restaurar stock de productos
                foreach (var detalle in pedido.Detalles)
                {
                    var producto = await _context.Productos.FindAsync(detalle.ProductoId);
                    if (producto != null)
                    {
                        producto.Stock += detalle.Cantidad ?? 0;
                        producto.FechaModificacion = DateTime.UtcNow;
                    }
                }

                _context.Pedidos.Remove(pedido);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                { 
                    Exito = true,
                    Mensaje = "Pedido eliminado correctamente",
                    Datos = new { pedidoId = id }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DeletePedido: {ex.Message}");
                return StatusCode(500, new { 
                    exito = false,
                    error = "Error al eliminar pedido",
                    detalles = ex.Message 
                });
            }
        }

        // GET: api/pedidos/usuario/{usuarioId}
        [HttpGet("usuario/{usuarioId}")]
        [Authorize(Roles = "administrador,vendedor")]
        public async Task<ActionResult<IEnumerable<PedidoResponseDTO>>> GetPedidosByUsuario(int usuarioId)
        {
            try
            {
                // Verificar que el usuario existe
                var usuarioExiste = await _context.Usuarios
                    .AnyAsync(u => u.Id == usuarioId && u.Activo == true);

                if (!usuarioExiste)
                {
                    return NotFound(new ApiResponse
                    {
                        Exito = false,
                        Mensaje = $"Usuario con ID {usuarioId} no encontrado"
                    });
                }

                var pedidos = await _pedidosService.GetByUsuario(usuarioId);
                
                return Ok(new ApiResponse<IEnumerable<PedidoResponseDTO>>
                {
                    Exito = true,
                    Mensaje = $"Pedidos del usuario {usuarioId} obtenidos exitosamente",
                    Datos = pedidos
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetPedidosByUsuario: {ex.Message}");
                return StatusCode(500, new { 
                    exito = false,
                    error = "Error al obtener pedidos del usuario",
                    detalles = ex.Message 
                });
            }
        }

        private bool PedidoExists(int id)
        {
            return _context.Pedidos.Any(e => e.Id == id);
        }
    }
}