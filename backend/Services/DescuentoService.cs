using Ferremas.Api.DTOs;
using Ferremas.Api.Models;
using Ferremas.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ferremas.Api.Data;

namespace Ferremas.Api.Services
{
    public class DescuentoService : IDescuentoService
    {
        private readonly AppDbContext _context;

        public DescuentoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<DescuentoResponseDTO>> ObtenerTodos()
        {
            return await _context.Descuentos
                .Select(d => new DescuentoResponseDTO
                {
                    Id = d.Id,
                    Codigo = d.Codigo,
                    Porcentaje = d.Porcentaje,
                    FechaInicio = d.FechaInicio,
                    FechaFin = d.FechaFin,
                    Activo = d.Activo
                }).ToListAsync();
        }

        public async Task<DescuentoResponseDTO?> ObtenerPorCodigo(string codigo)
        {
            var d = await _context.Descuentos
                .FirstOrDefaultAsync(x => x.Codigo == codigo && x.Activo);

            if (d == null || DateTime.Now < d.FechaInicio || DateTime.Now > d.FechaFin)
                return null;

            return new DescuentoResponseDTO
            {
                Id = d.Id,
                Codigo = d.Codigo,
                Porcentaje = d.Porcentaje,
                FechaInicio = d.FechaInicio,
                FechaFin = d.FechaFin,
                Activo = d.Activo
            };
        }

        public async Task<DescuentoResponseDTO> Crear(DescuentoCreateDTO dto)
        {
            var descuento = new Descuento
            {
                Codigo = dto.Codigo,
                Porcentaje = dto.Porcentaje,
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin,
                Activo = true
            };

            _context.Descuentos.Add(descuento);
            await _context.SaveChangesAsync();

            return new DescuentoResponseDTO
            {
                Id = descuento.Id,
                Codigo = descuento.Codigo,
                Porcentaje = descuento.Porcentaje,
                FechaInicio = descuento.FechaInicio,
                FechaFin = descuento.FechaFin,
                Activo = descuento.Activo
            };
        }

        public async Task<bool> Desactivar(int id)
        {
            var d = await _context.Descuentos.FindAsync(id);
            if (d == null) return false;

            d.Activo = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<DescuentoResponseDTO?> ObtenerPorId(int id)
        {
            var d = await _context.Descuentos.FindAsync(id);
            if (d == null) return null;
            return new DescuentoResponseDTO
            {
                Id = d.Id,
                Codigo = d.Codigo,
                Porcentaje = d.Porcentaje,
                FechaInicio = d.FechaInicio,
                FechaFin = d.FechaFin,
                Activo = d.Activo
            };
        }

        public async Task<bool> Actualizar(int id, DescuentoCreateDTO dto)
        {
            var d = await _context.Descuentos.FindAsync(id);
            if (d == null) return false;
            d.Codigo = dto.Codigo;
            d.Porcentaje = dto.Porcentaje;
            d.FechaInicio = dto.FechaInicio;
            d.FechaFin = dto.FechaFin;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Eliminar(int id)
        {
            var d = await _context.Descuentos.FindAsync(id);
            if (d == null) return false;
            _context.Descuentos.Remove(d);
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 