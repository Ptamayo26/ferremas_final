using Ferremas.Api.DTOs;
using Ferremas.Api.Models;
using Ferremas.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ferremas.Api.Data;

namespace Ferremas.Api.Services
{
    public class ProveedoresService : IProveedoresService
    {
        private readonly AppDbContext _context;

        public ProveedoresService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProveedorResponseDTO>> ObtenerTodos()
        {
            return await _context.Proveedores
                .Where(p => p.Activo)
                .Select(p => new ProveedorResponseDTO
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Contacto = p.Contacto,
                    Correo = p.Correo,
                    Telefono = p.Telefono,
                    Direccion = p.Direccion
                }).ToListAsync();
        }

        public async Task<ProveedorResponseDTO?> ObtenerPorId(int id)
        {
            var p = await _context.Proveedores.FindAsync(id);
            if (p == null || !p.Activo) return null;

            return new ProveedorResponseDTO
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Contacto = p.Contacto,
                Correo = p.Correo,
                Telefono = p.Telefono,
                Direccion = p.Direccion
            };
        }

        public async Task<ProveedorResponseDTO> Crear(ProveedorCreateDTO dto)
        {
            var proveedor = new Proveedor
            {
                Nombre = dto.Nombre,
                Contacto = dto.Contacto,
                Correo = dto.Correo,
                Telefono = dto.Telefono,
                Direccion = dto.Direccion,
                Activo = true
            };

            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();

            return new ProveedorResponseDTO
            {
                Id = proveedor.Id,
                Nombre = proveedor.Nombre,
                Contacto = proveedor.Contacto,
                Correo = proveedor.Correo,
                Telefono = proveedor.Telefono,
                Direccion = proveedor.Direccion
            };
        }

        public async Task<bool> Eliminar(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null) return false;

            proveedor.Activo = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 