using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ferremas.Api.Data;
using Ferremas.Api.Models;
using Ferremas.Api.DTOs;
using Ferremas.Api.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using BCrypt.Net;
using Microsoft.Extensions.Logging;

namespace Ferremas.Api.Services
{
    public class UsuariosService : IUsuariosService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UsuariosService> _logger;

        public UsuariosService(AppDbContext context, IConfiguration configuration, ILogger<UsuariosService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<IEnumerable<UsuarioResponseDTO>> GetAll()
        {
            var usuarios = await _context.Usuarios
                .ToListAsync();

            return usuarios.Select(u => new UsuarioResponseDTO
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Apellido = u.Apellido,
                Email = u.Email,
                Rut = u.Rut,
                Telefono = u.Telefono,
                Rol = u.Rol,
                Activo = u.Activo ?? false,
                FechaRegistro = u.FechaRegistro
            });
        }

        public async Task<UsuarioResponseDTO?> GetById(int id)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
                return null;

            return new UsuarioResponseDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                Rut = usuario.Rut,
                Telefono = usuario.Telefono,
                Rol = usuario.Rol,
                Activo = usuario.Activo ?? false,
                FechaRegistro = usuario.FechaRegistro
            };
        }

        public async Task<UsuarioResponseDTO> Create(UsuarioCreateDTO usuarioDto)
        {
            var now = DateTime.UtcNow;
            var usuario = new Usuario
            {
                Nombre = usuarioDto.Nombre,
                Apellido = usuarioDto.Apellido ?? string.Empty,
                Email = usuarioDto.Email,
                PasswordHash = HashPassword(usuarioDto.Password),
                Rut = usuarioDto.Rut ?? string.Empty,
                Telefono = usuarioDto.Telefono,
                Rol = usuarioDto.Rol,
                Activo = true,
                FechaRegistro = now
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return new UsuarioResponseDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                Rut = usuario.Rut,
                Telefono = usuario.Telefono,
                Rol = usuario.Rol,
                Activo = usuario.Activo ?? false,
                FechaRegistro = usuario.FechaRegistro
            };
        }

        public async Task<UsuarioResponseDTO?> Update(int id, UsuarioUpdateDTO usuarioDto)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
                return null;

            usuario.Nombre = usuarioDto.Nombre;
            usuario.Apellido = usuarioDto.Apellido ?? string.Empty;
            usuario.Telefono = usuarioDto.Telefono;
            usuario.Rol = usuarioDto.Rol;
            usuario.Activo = usuarioDto.Activo ?? usuario.Activo;
            await _context.SaveChangesAsync();

            return new UsuarioResponseDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                Rut = usuario.Rut,
                Telefono = usuario.Telefono,
                Rol = usuario.Rol,
                Activo = usuario.Activo ?? false,
                FechaRegistro = usuario.FechaRegistro
            };
        }

        public async Task<bool> Delete(int id)
        {
            _logger.LogInformation($"Iniciando proceso de eliminación para usuario ID: {id}");

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
            {
                _logger.LogWarning($"Usuario con ID {id} no encontrado en la base de datos");
                return false;
            }

            _logger.LogInformation($"Usuario encontrado: ID={usuario.Id}, Nombre={usuario.Nombre}, Activo={usuario.Activo}");

            usuario.Activo = false;
            _context.Entry(usuario).Property(u => u.Activo).IsModified = true;
            try 
            {
                _logger.LogInformation("Antes de SaveChangesAsync");
                await _context.SaveChangesAsync();
                _logger.LogInformation("Después de SaveChangesAsync");
                _logger.LogInformation($"Usuario {id} marcado como inactivo exitosamente");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al guardar cambios para usuario {id}");
                return false;
            }
        }

        public async Task<string?> Autenticar(string email, string password)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email && u.Activo == true);

            if (usuario == null || !VerifyPassword(password, usuario.PasswordHash))
                return null;

            var token = GenerateJwtToken(usuario);
            return token;
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        private string GenerateJwtToken(Usuario usuario)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey no configurada")));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol.ToLower())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> ChangePasswordAsync(int id, string currentPassword, string newPassword)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return false;
                }

                if (!VerifyPassword(currentPassword, usuario.PasswordHash))
                {
                    return false;
                }

                usuario.PasswordHash = HashPassword(newPassword);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar contraseña");
                throw;
            }
        }
    }
} 