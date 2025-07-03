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
    public class ClienteService : IClienteService
    {
        private readonly AppDbContext _context;

        public ClienteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ClienteResponseDTO>> GetAll()
        {
            var clientes = await _context.Clientes
                .Include(c => c.Usuario)
                    .ThenInclude(u => u.Direcciones)
                .Where(c => c.Activo == true)
                .OrderBy(c => c.Apellido)
                .ThenBy(c => c.Nombre)
                .ToListAsync();

            return clientes.Select(c => new ClienteResponseDTO
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Apellido = c.Apellido,
                Rut = c.Rut,
                CorreoElectronico = c.CorreoElectronico,
                Telefono = c.Telefono,
                FechaCreacion = c.FechaCreacion.GetValueOrDefault(),
                FechaModificacion = c.FechaModificacion.GetValueOrDefault(),
                Activo = c.Activo,
                UsuarioId = c.UsuarioId,
                Direcciones = c.Usuario?.Direcciones?.Select(d => new DireccionDTO
                {
                    Id = d.Id,
                    Calle = d.Calle,
                    Numero = d.Numero,
                    Departamento = d.Departamento,
                    Comuna = d.Comuna,
                    Region = d.Region,
                    CodigoPostal = d.CodigoPostal,
                    EsPrincipal = d.EsPrincipal,
                    FechaModificacion = d.FechaModificacion.GetValueOrDefault()
                }).ToList()
            });
        }

        public async Task<ClienteResponseDTO?> GetById(int id)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Usuario)
                    .ThenInclude(u => u.Direcciones)
                .FirstOrDefaultAsync(c => c.Id == id && c.Activo == true);

            if (cliente == null)
                return null;

            return new ClienteResponseDTO
            {
                Id = cliente.Id,
                Nombre = cliente.Nombre,
                Apellido = cliente.Apellido,
                Rut = cliente.Rut,
                CorreoElectronico = cliente.CorreoElectronico,
                Telefono = cliente.Telefono,
                FechaCreacion = cliente.FechaCreacion.GetValueOrDefault(),
                FechaModificacion = cliente.FechaModificacion.GetValueOrDefault(),
                Activo = cliente.Activo,
                UsuarioId = cliente.UsuarioId,
                Direcciones = cliente.Usuario?.Direcciones?.Select(d => new DireccionDTO
                {
                    Id = d.Id,
                    Calle = d.Calle,
                    Numero = d.Numero,
                    Departamento = d.Departamento,
                    Comuna = d.Comuna,
                    Region = d.Region,
                    CodigoPostal = d.CodigoPostal,
                    EsPrincipal = d.EsPrincipal,
                    FechaModificacion = d.FechaModificacion.GetValueOrDefault()
                }).ToList()
            };
        }

        public async Task<ClienteResponseDTO> Create(ClienteCreateDTO clienteDto)
        {
            // Validar RUT único
            if (await _context.Clientes.AnyAsync(c => c.Rut == clienteDto.Rut))
                throw new Exception("El RUT ya está registrado");

            // Validar correo único
            if (await _context.Clientes.AnyAsync(c => c.CorreoElectronico == clienteDto.CorreoElectronico))
                throw new Exception("El correo electrónico ya está registrado");

            // Validar que se envíe al menos una dirección
            if (clienteDto.Direcciones == null || !clienteDto.Direcciones.Any())
                throw new Exception("Debe ingresar al menos una dirección para el cliente.");

            var now = DateTime.UtcNow;
            var direccionPrincipal = clienteDto.Direcciones.FirstOrDefault(d => d.EsPrincipal) ?? clienteDto.Direcciones.First();

            // Crear el usuario primero
            var usuario = new Usuario
            {
                Nombre = clienteDto.Nombre,
                Apellido = clienteDto.Apellido,
                Email = clienteDto.CorreoElectronico,
                PasswordHash = "TEMP_PASSWORD", // Esto debería ser manejado por un servicio de autenticación
                Rol = "Cliente",
                Activo = true,
                FechaRegistro = now,
                Direcciones = clienteDto.Direcciones.Select(d => new Direccion
                {
                    Calle = d.Calle,
                    Numero = d.Numero,
                    Departamento = d.Departamento,
                    Comuna = d.Comuna,
                    Region = d.Region,
                    CodigoPostal = d.CodigoPostal,
                    EsPrincipal = d.EsPrincipal,
                    FechaCreacion = now,
                    FechaModificacion = now
                }).ToList()
            };

            var cliente = new Cliente
            {
                Nombre = clienteDto.Nombre,
                Apellido = clienteDto.Apellido,
                Rut = clienteDto.Rut,
                CorreoElectronico = clienteDto.CorreoElectronico,
                Telefono = clienteDto.Telefono,
                FechaCreacion = now,
                FechaModificacion = now,
                Activo = true,
                Usuario = usuario
            };

            try
            {
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Mostrar la excepción interna si existe
                if (ex.InnerException != null)
                    Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
                else
                    Console.WriteLine("Exception: " + ex.Message);
                throw;
            }

            return new ClienteResponseDTO
            {
                Id = cliente.Id,
                Nombre = cliente.Nombre,
                Apellido = cliente.Apellido,
                Rut = cliente.Rut,
                CorreoElectronico = cliente.CorreoElectronico,
                Telefono = cliente.Telefono,
                FechaCreacion = cliente.FechaCreacion.GetValueOrDefault(),
                FechaModificacion = cliente.FechaModificacion.GetValueOrDefault(),
                Activo = cliente.Activo,
                UsuarioId = cliente.UsuarioId,
                Direcciones = cliente.Usuario?.Direcciones?.Select(d => new DireccionDTO
                {
                    Id = d.Id,
                    Calle = d.Calle,
                    Numero = d.Numero,
                    Departamento = d.Departamento,
                    Comuna = d.Comuna,
                    Region = d.Region,
                    CodigoPostal = d.CodigoPostal,
                    EsPrincipal = d.EsPrincipal,
                    FechaModificacion = d.FechaModificacion.GetValueOrDefault()
                }).ToList()
            };
        }

        public async Task<ClienteResponseDTO?> Update(int id, ClienteUpdateDTO clienteDto)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Usuario)
                    .ThenInclude(u => u.Direcciones)
                .FirstOrDefaultAsync(c => c.Id == id && c.Activo == true);

            if (cliente == null)
                return null;

            // Validar RUT único si se está actualizando
            if (!string.IsNullOrEmpty(clienteDto.Rut) && clienteDto.Rut != cliente.Rut)
            {
                if (await _context.Clientes.AnyAsync(c => c.Rut == clienteDto.Rut))
                    throw new Exception("El RUT ya está registrado");
                cliente.Rut = clienteDto.Rut;
            }

            // Validar correo único si se está actualizando
            if (!string.IsNullOrEmpty(clienteDto.CorreoElectronico) && clienteDto.CorreoElectronico != cliente.CorreoElectronico)
            {
                if (await _context.Clientes.AnyAsync(c => c.CorreoElectronico == clienteDto.CorreoElectronico))
                    throw new Exception("El correo electrónico ya está registrado");
                cliente.CorreoElectronico = clienteDto.CorreoElectronico;
            }

            if (!string.IsNullOrEmpty(clienteDto.Nombre))
                cliente.Nombre = clienteDto.Nombre;

            if (!string.IsNullOrEmpty(clienteDto.Apellido))
                cliente.Apellido = clienteDto.Apellido;

            if (!string.IsNullOrEmpty(clienteDto.Telefono))
                cliente.Telefono = clienteDto.Telefono;

            cliente.FechaModificacion = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
                else
                    Console.WriteLine("Exception: " + ex.Message);
                throw;
            }

            return new ClienteResponseDTO
            {
                Id = cliente.Id,
                Nombre = cliente.Nombre,
                Apellido = cliente.Apellido,
                Rut = cliente.Rut,
                CorreoElectronico = cliente.CorreoElectronico,
                Telefono = cliente.Telefono,
                FechaCreacion = cliente.FechaCreacion.GetValueOrDefault(),
                FechaModificacion = cliente.FechaModificacion.GetValueOrDefault(),
                Activo = cliente.Activo,
                UsuarioId = cliente.UsuarioId,
                Direcciones = cliente.Usuario?.Direcciones?.Select(d => new DireccionDTO
                {
                    Id = d.Id,
                    Calle = d.Calle,
                    Numero = d.Numero,
                    Departamento = d.Departamento,
                    Comuna = d.Comuna,
                    Region = d.Region,
                    CodigoPostal = d.CodigoPostal,
                    EsPrincipal = d.EsPrincipal,
                    FechaModificacion = d.FechaModificacion.GetValueOrDefault()
                }).ToList()
            };
        }

        public async Task<bool> Delete(int id)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.Id == id && c.Activo == true);

            if (cliente == null)
                return false;

            cliente.Activo = false;
            cliente.FechaModificacion = DateTime.UtcNow;

            if (cliente.Usuario != null)
            {
                cliente.Usuario.Activo = false;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
} 