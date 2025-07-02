using Ferremas.Api.DTOs;
using Ferremas.Api.Models;
using Ferremas.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ferremas.Api.Data;

namespace Ferremas.Api.Services
{
    public class FacturaService : IFacturaService
    {
        private readonly AppDbContext _context;

        public FacturaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<FacturaResponseDTO>> ObtenerTodas()
        {
            return await _context.Facturas
                .Select(f => new FacturaResponseDTO
                {
                    Id = f.Id,
                    PedidoId = f.PedidoId,
                    FechaEmision = f.FechaEmision,
                    MontoTotal = f.MontoTotal,
                    Anulada = f.Anulada
                }).ToListAsync();
        }

        public async Task<FacturaResponseDTO?> ObtenerPorId(int id)
        {
            var f = await _context.Facturas.FindAsync(id);
            if (f == null) return null;

            return new FacturaResponseDTO
            {
                Id = f.Id,
                PedidoId = f.PedidoId,
                FechaEmision = f.FechaEmision,
                MontoTotal = f.MontoTotal,
                Anulada = f.Anulada
            };
        }

        public async Task<FacturaResponseDTO> Crear(FacturaCreateDTO dto)
        {
            var factura = new Factura
            {
                PedidoId = dto.PedidoId,
                MontoTotal = dto.MontoTotal,
                FechaEmision = DateTime.Now,
                Anulada = false
            };

            _context.Facturas.Add(factura);
            await _context.SaveChangesAsync();

            return new FacturaResponseDTO
            {
                Id = factura.Id,
                PedidoId = factura.PedidoId,
                FechaEmision = factura.FechaEmision,
                MontoTotal = factura.MontoTotal,
                Anulada = false
            };
        }

        public async Task<bool> Anular(int id)
        {
            var factura = await _context.Facturas.FindAsync(id);
            if (factura == null) return false;

            factura.Anulada = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 