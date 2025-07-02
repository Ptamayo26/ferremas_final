using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ferremas.Api.Models;
using Ferremas.Api.DTOs;
using Ferremas.Api.Data;
using Ferremas.Api.Services.Interfaces;

namespace Ferremas.Api.Services
{
    public class PedidosService : IPedidosService
    {
        private readonly AppDbContext _context;

        public PedidosService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PedidoResponseDTO>> GetAll()
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Usuario)
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Producto)
                .ToListAsync();

            return pedidos.Select(p => new PedidoResponseDTO
            {
                Id = p.Id,
                UsuarioId = p.UsuarioId ?? 0,
                UsuarioNombre = p.Usuario?.Nombre ?? "Desconocido",
                FechaPedido = p.FechaPedido ?? DateTime.MinValue,
                Total = p.Total ?? 0,
                Estado = p.Estado ?? "Pendiente",
                Observaciones = p.Observaciones,
                DireccionEntrega = p.DireccionEntrega,
                FechaCreacion = p.FechaCreacion,
                FechaModificacion = p.FechaModificacion,
                Activo = p.Activo,
                Detalles = p.Detalles.Select(d => new DetallePedidoResponseDTO
                {
                    Id = d.Id,
                    ProductoId = d.ProductoId ?? 0,
                    ProductoNombre = d.Producto?.Nombre ?? "Desconocido",
                    Cantidad = d.Cantidad ?? 0,
                    PrecioUnitario = d.PrecioUnitario ?? 0,
                    Subtotal = d.Subtotal ?? 0,
                    Observaciones = d.Observaciones
                }).ToList()
            });
        }

        public async Task<PedidoResponseDTO?> GetById(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Usuario)
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
                return null;

            return new PedidoResponseDTO
            {
                Id = pedido.Id,
                UsuarioId = pedido.UsuarioId ?? 0,
                UsuarioNombre = pedido.Usuario?.Nombre ?? "Desconocido",
                FechaPedido = pedido.FechaPedido ?? DateTime.MinValue,
                Total = pedido.Total ?? 0,
                Estado = pedido.Estado ?? "Pendiente",
                Observaciones = pedido.Observaciones,
                DireccionEntrega = pedido.DireccionEntrega,
                FechaCreacion = pedido.FechaCreacion,
                FechaModificacion = pedido.FechaModificacion,
                Activo = pedido.Activo,
                Detalles = pedido.Detalles.Select(d => new DetallePedidoResponseDTO
                {
                    Id = d.Id,
                    ProductoId = d.ProductoId ?? 0,
                    ProductoNombre = d.Producto?.Nombre ?? "Desconocido",
                    Cantidad = d.Cantidad ?? 0,
                    PrecioUnitario = d.PrecioUnitario ?? 0,
                    Subtotal = d.Subtotal ?? 0,
                    Observaciones = d.Observaciones
                }).ToList()
            };
        }

        public async Task<PedidoResponseDTO> Create(PedidoCreateDTO pedidoDto)
        {
            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var pedido = new Pedido
                    {
                        UsuarioId = pedidoDto.UsuarioId,
                        FechaPedido = DateTime.Now,
                        Total = pedidoDto.Total,
                        Estado = pedidoDto.Estado,
                        Observaciones = pedidoDto.Observaciones,
                        DireccionEntrega = pedidoDto.DireccionEntrega,
                        FechaCreacion = DateTime.Now,
                        Activo = true
                    };

                    _context.Pedidos.Add(pedido);
                    await _context.SaveChangesAsync();

                    // Add order details
                    if (pedidoDto.Detalles != null && pedidoDto.Detalles.Any())
                    {
                        foreach (var detalle in pedidoDto.Detalles)
                        {
                            var detallePedido = new DetallePedido
                            {
                                PedidoId = pedido.Id,
                                ProductoId = detalle.ProductoId,
                                Cantidad = detalle.Cantidad,
                                PrecioUnitario = detalle.PrecioUnitario,
                                Subtotal = detalle.Cantidad * detalle.PrecioUnitario,
                                Observaciones = detalle.Observaciones
                            };

                            _context.DetallesPedido.Add(detallePedido);
                        }

                        await _context.SaveChangesAsync();
                    }

                    await transaction.CommitAsync();

                    return await GetById(pedido.Id) ?? throw new Exception("Error al crear el pedido");
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear el pedido: {ex.Message}", ex);
            }
        }

        public async Task<PedidoResponseDTO?> Update(int id, PedidoUpdateDTO pedidoDto)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
                return null;

            pedido.Estado = pedidoDto.Estado;
            pedido.Observaciones = pedidoDto.Observaciones;
            pedido.DireccionEntrega = pedidoDto.DireccionEntrega;
            pedido.FechaModificacion = DateTime.Now;

            await _context.SaveChangesAsync();

            return await GetById(id);
        }

        public async Task<IEnumerable<PedidoResponseDTO>> GetByUsuario(int usuarioId)
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Usuario)
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Producto)
                .Where(p => p.UsuarioId == usuarioId)
                .ToListAsync();

            return pedidos.Select(p => new PedidoResponseDTO
            {
                Id = p.Id,
                UsuarioId = p.UsuarioId ?? 0,
                UsuarioNombre = p.Usuario?.Nombre ?? "Desconocido",
                FechaPedido = p.FechaPedido ?? DateTime.MinValue,
                Total = p.Total ?? 0,
                Estado = p.Estado ?? "Pendiente",
                Observaciones = p.Observaciones,
                DireccionEntrega = p.DireccionEntrega,
                FechaCreacion = p.FechaCreacion,
                FechaModificacion = p.FechaModificacion,
                Activo = p.Activo,
                Detalles = p.Detalles.Select(d => new DetallePedidoResponseDTO
                {
                    Id = d.Id,
                    ProductoId = d.ProductoId ?? 0,
                    ProductoNombre = d.Producto?.Nombre ?? "Desconocido",
                    Cantidad = d.Cantidad ?? 0,
                    PrecioUnitario = d.PrecioUnitario ?? 0,
                    Subtotal = d.Subtotal ?? 0,
                    Observaciones = d.Observaciones
                }).ToList()
            });
        }

        public async Task<bool> Delete(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
                return false;

            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
