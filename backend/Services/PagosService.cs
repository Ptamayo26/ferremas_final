using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ferremas.Api.Models;
using Ferremas.Api.DTOs;
using Ferremas.Api.Data;
using Ferremas.Api.Services.Interfaces;
using FerremasBackend.Services;

namespace Ferremas.Api.Services
{
    public class PagosService : IPagosService
    {
        private readonly AppDbContext _context;
        private readonly MercadoPagoService _mercadoPagoService;

        public PagosService(AppDbContext context, MercadoPagoService mercadoPagoService)
        {
            _context = context;
            _mercadoPagoService = mercadoPagoService;
        }

        public async Task<IEnumerable<Pago>> GetAll()
        {
            return await _context.Pagos
                .Include(p => p.Pedido)
                    .ThenInclude(p => p.Usuario)
                .Include(p => p.Pedido)
                    .ThenInclude(p => p.Detalles)
                        .ThenInclude(d => d.Producto)
                .OrderByDescending(p => p.FechaPago)
                .Select(p => new Pago
                {
                    Id = p.Id,
                    PedidoId = p.PedidoId,
                    Monto = p.Monto,
                    MetodoPago = p.MetodoPago,
                    TransaccionId = p.TransaccionId,
                    TokenPasarela = p.TokenPasarela,
                    DatosRespuesta = p.DatosRespuesta,
                    FechaPago = p.FechaPago,
                    Estado = p.Estado,
                    Pedido = new Pedido
                    {
                        Id = p.Pedido.Id,
                        UsuarioId = p.Pedido.UsuarioId,
                        Total = p.Pedido.Total,
                        Estado = p.Pedido.Estado,
                        FechaPedido = p.Pedido.FechaPedido,
                        Usuario = p.Pedido.Usuario != null ? new Usuario
                        {
                            Id = p.Pedido.Usuario.Id,
                            Nombre = p.Pedido.Usuario.Nombre,
                            Email = p.Pedido.Usuario.Email
                        } : null
                    }
                })
                .ToListAsync();
        }

        public async Task<Pago?> GetById(int id)
        {
            return await _context.Pagos
                .Include(p => p.Pedido)
                    .ThenInclude(p => p.Usuario)
                .Include(p => p.Pedido)
                    .ThenInclude(p => p.Detalles)
                        .ThenInclude(d => d.Producto)
                .Select(p => new Pago
                {
                    Id = p.Id,
                    PedidoId = p.PedidoId,
                    Monto = p.Monto,
                    MetodoPago = p.MetodoPago,
                    TransaccionId = p.TransaccionId,
                    TokenPasarela = p.TokenPasarela,
                    DatosRespuesta = p.DatosRespuesta,
                    FechaPago = p.FechaPago,
                    Estado = p.Estado,
                    Pedido = new Pedido
                    {
                        Id = p.Pedido.Id,
                        UsuarioId = p.Pedido.UsuarioId,
                        Total = p.Pedido.Total,
                        Estado = p.Pedido.Estado,
                        FechaPedido = p.Pedido.FechaPedido,
                        Usuario = p.Pedido.Usuario != null ? new Usuario
                        {
                            Id = p.Pedido.Usuario.Id,
                            Nombre = p.Pedido.Usuario.Nombre,
                            Email = p.Pedido.Usuario.Email
                        } : null
                    }
                })
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Pago> Create(PagoCreateDTO pagoDto)
        {
            var pedido = await _context.Pedidos.FindAsync(pagoDto.PedidoId);
            if (pedido == null)
                throw new Exception("Pedido no encontrado");

            // Si el método de pago es Mercado Pago, procesamos el pago
            if (pagoDto.MetodoPago.ToUpper() == "MERCADOPAGO")
            {
                var mercadoPagoResponse = await _mercadoPagoService.CrearPago(pagoDto);
                
                var pago = new Pago
                {
                    PedidoId = pagoDto.PedidoId,
                    Monto = pagoDto.Monto,
                    MetodoPago = pagoDto.MetodoPago,
                    TransaccionId = mercadoPagoResponse.Id.ToString(),
                    TokenPasarela = mercadoPagoResponse.Status,
                    DatosRespuesta = System.Text.Json.JsonSerializer.Serialize(mercadoPagoResponse),
                    FechaPago = DateTime.Now,
                    Estado = mercadoPagoResponse.Status
                };

                _context.Pagos.Add(pago);
                await _context.SaveChangesAsync();

                return pago;
            }
            else
            {
                // Para otros métodos de pago
                var pago = new Pago
                {
                    PedidoId = pagoDto.PedidoId,
                    Monto = pagoDto.Monto,
                    MetodoPago = pagoDto.MetodoPago,
                    TransaccionId = pagoDto.TransaccionId,
                    TokenPasarela = pagoDto.TokenPasarela,
                    DatosRespuesta = pagoDto.DatosRespuesta,
                    FechaPago = DateTime.Now,
                    Estado = "PENDIENTE"
                };

                _context.Pagos.Add(pago);
                await _context.SaveChangesAsync();

                return pago;
            }
        }

        public async Task<bool> Delete(int id)
        {
            var pago = await _context.Pagos.FindAsync(id);
            if (pago == null)
                return false;

            _context.Pagos.Remove(pago);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ProcesarPago(int pedidoId, decimal monto)
        {
            var pedido = await _context.Pedidos
                .FirstOrDefaultAsync(p => p.Id == pedidoId && p.Estado == "Pendiente");

            if (pedido == null)
                return false;

            var pago = new Pago
            {
                PedidoId = pedidoId,
                Monto = monto,
                FechaPago = DateTime.Now,
                Estado = "PENDIENTE",
                MetodoPago = "MERCADOPAGO"
            };

            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();

            // Crear el pago en Mercado Pago
            var pagoDto = new PagoCreateDTO
            {
                PedidoId = pedidoId,
                Monto = monto,
                MetodoPago = "MERCADOPAGO"
            };

            try
            {
                var mercadoPagoResponse = await _mercadoPagoService.CrearPago(pagoDto);
                pago.TransaccionId = mercadoPagoResponse.Id.ToString();
                pago.TokenPasarela = mercadoPagoResponse.Status;
                pago.DatosRespuesta = System.Text.Json.JsonSerializer.Serialize(mercadoPagoResponse);
                pago.Estado = mercadoPagoResponse.Status;

                if (mercadoPagoResponse.Status == "approved")
                {
                    pedido.Estado = "Pagado";
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                pago.Estado = "ERROR";
                pago.DatosRespuesta = ex.Message;
                await _context.SaveChangesAsync();
                return false;
            }
        }

        public async Task<bool> VerificarTransferencia(string numeroTransferencia, decimal monto)
        {
            // Aquí iría la lógica de verificación con el banco
            // Por ahora retornamos true como simulación
            await Task.Delay(1000); // Simulamos una llamada asíncrona
            return true;
        }

        public async Task<Pago?> GetByTokenPasarela(string tokenPasarela)
        {
            return await _context.Pagos
                .Include(p => p.Pedido)
                    .ThenInclude(p => p.Usuario)
                .Include(p => p.Pedido)
                    .ThenInclude(p => p.Detalles)
                        .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.TokenPasarela == tokenPasarela);
        }
    }
} 