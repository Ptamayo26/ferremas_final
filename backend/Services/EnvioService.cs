using System;
using System.Threading.Tasks;
using Ferremas.Api.DTOs;
using Ferremas.Api.Services.Interfaces;
using Ferremas.Api.Data;
using Ferremas.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Ferremas.Api.Services
{
    public class EnvioService : IEnvioService
    {
        private readonly AppDbContext _dbContext;

        public EnvioService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<EnvioDTO> CrearEnvioAsync(EnvioCreateDTO dto)
        {
            var pedido = await _dbContext.Pedidos
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Id == dto.PedidoId) 
                ?? throw new InvalidOperationException("Pedido no encontrado");

            var envio = new Envio
            {
                PedidoId = dto.PedidoId,
                DireccionEnvio = dto.DireccionDestino,
                EstadoEnvio = "Pendiente",
                ProveedorTransporte = "Chilexpress",
                TrackingUrl = "",
                Observaciones = $"Envío a {dto.ComunaDestino}, {dto.RegionDestino}",
                ComunaDestino = dto.ComunaDestino,
                RegionDestino = dto.RegionDestino,
                TelefonoContacto = dto.TelefonoContacto,
                NombreDestinatario = dto.NombreDestinatario,
                Pedido = pedido
            };

            _dbContext.Envios.Add(envio);
            await _dbContext.SaveChangesAsync();

            return new EnvioDTO
            {
                Id = envio.Id,
                PedidoId = envio.PedidoId,
                DireccionDestino = envio.DireccionEnvio,
                Estado = envio.EstadoEnvio,
                NumeroSeguimiento = envio.TrackingUrl,
                FechaCreacion = DateTime.UtcNow
            };
        }

        public async Task<EnvioDTO> ObtenerEnvioPorPedidoAsync(int pedidoId)
        {
            var envio = await _dbContext.Envios
                .FirstOrDefaultAsync(e => e.PedidoId == pedidoId);

            if (envio == null)
                return null;

            return new EnvioDTO
            {
                Id = envio.Id,
                PedidoId = envio.PedidoId,
                DireccionDestino = envio.DireccionEnvio,
                Estado = envio.EstadoEnvio,
                NumeroSeguimiento = envio.TrackingUrl,
                FechaCreacion = DateTime.UtcNow
            };
        }

        public async Task<bool> ActualizarEstadoEnvioAsync(int envioId, string nuevoEstado)
        {
            var envio = await _dbContext.Envios.FindAsync(envioId);
            if (envio == null)
                return false;

            envio.EstadoEnvio = nuevoEstado;
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}