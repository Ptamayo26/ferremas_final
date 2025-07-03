using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BCrypt.Net;
using Ferremas.Api.Models;
using Ferremas.Api.DTOs;
using Ferremas.Api.Data;
using Ferremas.Api.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;

namespace Ferremas.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private static readonly HashSet<string> RolesPermitidos = new HashSet<string> { "administrador", "vendedor", "bodeguero", "contador", "repartidor", "cliente" };

        public AuthService(
            AppDbContext context,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        [AllowAnonymous]
        public async Task<AuthResponse> RegisterAsync(UsuarioCreateDTO registerDto)
        {
            _logger.LogInformation("Iniciando registro de usuario");
            _logger.LogInformation("Datos recibidos: Nombre={Nombre}, Apellido={Apellido}, Email={Email}, Rut={Rut}, Telefono={Telefono}, Rol={Rol}", 
                registerDto.Nombre, registerDto.Apellido, registerDto.Email, registerDto.Rut, registerDto.Telefono, registerDto.Rol);
            
            if (registerDto.Direccion != null)
            {
                _logger.LogInformation("Dirección recibida: Calle={Calle}, Numero={Numero}, Comuna={Comuna}, Region={Region}", 
                    registerDto.Direccion.Calle, registerDto.Direccion.Numero, registerDto.Direccion.Comuna, registerDto.Direccion.Region);
            }
            
            // Validar rol permitido
            if (string.IsNullOrWhiteSpace(registerDto.Rol) || !RolesPermitidos.Contains(registerDto.Rol.ToLower()))
            {
                _logger.LogWarning("Intento de registro con rol no permitido: {Rol}", registerDto.Rol);
                return new AuthResponse { Exito = false, Mensaje = "Rol no permitido." };
            }

            // Validar duplicado por RUT
            if (!string.IsNullOrWhiteSpace(registerDto.Rut) && await _context.Usuarios.AnyAsync(u => u.Rut == registerDto.Rut))
            {
                _logger.LogWarning("Intento de registro con RUT duplicado: {Rut}", registerDto.Rut);
                return new AuthResponse { Exito = false, Mensaje = "El RUT ya está registrado." };
            }

            // Validar duplicado por Email
            if (!string.IsNullOrWhiteSpace(registerDto.Email) && await _context.Usuarios.AnyAsync(u => u.Email == registerDto.Email))
            {
                _logger.LogWarning("Intento de registro con email duplicado: {Email}", registerDto.Email);
                return new AuthResponse { Exito = false, Mensaje = "El correo electrónico ya está registrado." };
            }

            // Crear nuevo usuario
            var usuario = new Usuario
            {
                Nombre = registerDto.Nombre,
                Apellido = registerDto.Apellido,
                Email = registerDto.Email,
                PasswordHash = HashPassword(registerDto.Password),
                Rut = registerDto.Rut,
                Telefono = registerDto.Telefono,
                Rol = registerDto.Rol.ToLower(),
                FechaRegistro = DateTime.UtcNow,
                Activo = true
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Si el usuario es cliente, crear registro en tabla clientes
            if (registerDto.Rol?.ToLower() == "cliente")
            {
                var cliente = new Cliente
                {
                    UsuarioId = usuario.Id,
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    Rut = usuario.Rut,
                    Telefono = usuario.Telefono,
                    CorreoElectronico = usuario.Email,
                    FechaCreacion = DateTime.UtcNow,
                    Activo = true
                };

                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();

                // Si se proporciona una dirección, crear la dirección
                if (registerDto.Direccion != null)
                {
                    var direccion = new Direccion
                    {
                        ClienteId = cliente.Id, // Usar el ID del cliente, no del usuario
                        UsuarioId = usuario.Id, // Opcional, para referencia
                        Calle = registerDto.Direccion.Calle,
                        Numero = registerDto.Direccion.Numero,
                        Departamento = registerDto.Direccion.Departamento,
                        Comuna = registerDto.Direccion.Comuna,
                        Region = registerDto.Direccion.Region,
                        CodigoPostal = registerDto.Direccion.CodigoPostal,
                        EsPrincipal = registerDto.Direccion.EsPrincipal, // Usar el valor del DTO
                        FechaCreacion = DateTime.UtcNow
                    };

                    _context.Direcciones.Add(direccion);
                    await _context.SaveChangesAsync();
                    
                    _logger.LogInformation("Dirección creada para el cliente: {Email}", usuario.Email);
                }
                
                _logger.LogInformation("Cliente creado para el usuario: {Email}", usuario.Email);
            }
            
            var token = GenerateJwtToken(usuario);
            _logger.LogInformation("Usuario registrado exitosamente: {Email}", usuario.Email);
            
            return new AuthResponse
            {
                Exito = true,
                Mensaje = "Usuario registrado exitosamente",
                Token = token,
                Usuario = new UsuarioResponseDTO
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    Email = usuario.Email,
                    Rut = usuario.Rut,
                    Telefono = usuario.Telefono,
                    Rol = usuario.Rol,
                    Activo = usuario.Activo,
                    FechaRegistro = usuario.FechaRegistro
                }
            };
        }

        [AllowAnonymous]
        public async Task<AuthResponse> LoginAsync(LoginDTO loginDto)
        {
            try
            {
                _logger.LogInformation("Intento de login para: {Email}", loginDto.Email);
                
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.Activo == true);

                if (usuario == null)
                {
                    _logger.LogWarning("Intento de login fallido - Usuario no encontrado: {Email}", loginDto.Email);
                    return new AuthResponse { Exito = false, Mensaje = "Credenciales inválidas" };
                }

                if (!RolesPermitidos.Contains(usuario.Rol.ToLower()))
                {
                    _logger.LogWarning("Intento de login fallido - Rol no permitido: {Rol}", usuario.Rol);
                    return new AuthResponse { Exito = false, Mensaje = "Rol no permitido." };
                }

                bool passwordValida = VerifyPassword(loginDto.Password, usuario.PasswordHash);
                
                if (!passwordValida)
                {
                    _logger.LogWarning("Intento de login fallido - Contraseña incorrecta para: {Email}", loginDto.Email);
                    return new AuthResponse { Exito = false, Mensaje = "Credenciales inválidas" };
                }

                var token = GenerateJwtToken(usuario);
                _logger.LogInformation("Login exitoso para: {Email}", loginDto.Email);

                return new AuthResponse
                {
                    Exito = true,
                    Mensaje = "Login exitoso",
                    Token = token,
                    Usuario = new UsuarioResponseDTO
                    {
                        Id = usuario.Id,
                        Nombre = usuario.Nombre,
                        Apellido = usuario.Apellido,
                        Email = usuario.Email,
                        Rut = usuario.Rut,
                        Telefono = usuario.Telefono,
                        Rol = usuario.Rol,
                        Activo = usuario.Activo,
                        FechaRegistro = usuario.FechaRegistro
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el login para: {Email}", loginDto.Email);
                return new AuthResponse
                {
                    Exito = false,
                    Mensaje = "Error interno del servidor"
                };
            }
        }

        [AllowAnonymous]
        public async Task<AuthResponse> ResetPasswordAsync(ResetPasswordDTO resetDto)
        {
            try
            {
                _logger.LogInformation("Iniciando reset de contraseña para: {Email}", resetDto.Email);
                
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == resetDto.Email && u.Activo == true);

                if (usuario == null)
                {
                    _logger.LogWarning("Intento de reset de contraseña fallido - Usuario no encontrado: {Email}", resetDto.Email);
                    return new AuthResponse { Exito = false, Mensaje = "No se encontró un usuario activo con ese correo electrónico." };
                }

                usuario.PasswordHash = HashPassword(resetDto.NewPassword);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Contraseña actualizada exitosamente para: {Email}", resetDto.Email);
                return new AuthResponse 
                { 
                    Exito = true, 
                    Mensaje = "Contraseña actualizada exitosamente. Por favor, inicie sesión con su nueva contraseña." 
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al resetear contraseña para: {Email}", resetDto.Email);
                return new AuthResponse 
                { 
                    Exito = false, 
                    Mensaje = "Error al restablecer la contraseña. Por favor, intente nuevamente." 
                };
            }
        }

        public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenDTO model)
        {
            try
            {
                _logger.LogInformation("Iniciando actualización de token");
                
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey no configurada"));

                // Validar el token actual
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };

                try
                {
                    var principal = tokenHandler.ValidateToken(model.Token, tokenValidationParameters, out var validatedToken);
                    var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    if (string.IsNullOrEmpty(userId))
                    {
                        return new AuthResponse { Exito = false, Mensaje = "Token inválido" };
                    }

                    var usuario = await _context.Usuarios.FindAsync(int.Parse(userId));
                    if (usuario == null || usuario.Activo != true)
                    {
                        return new AuthResponse { Exito = false, Mensaje = "Usuario no encontrado o inactivo" };
                    }

                    // Generar nuevo token
                    var newToken = GenerateJwtToken(usuario);

                    return new AuthResponse
                    {
                        Exito = true,
                        Mensaje = "Token actualizado exitosamente",
                        Token = newToken,
                        Usuario = new UsuarioResponseDTO
                        {
                            Id = usuario.Id,
                            Nombre = usuario.Nombre,
                            Apellido = usuario.Apellido,
                            Email = usuario.Email,
                            Rut = usuario.Rut,
                            Telefono = usuario.Telefono,
                            Rol = usuario.Rol,
                            Activo = usuario.Activo,
                            FechaRegistro = usuario.FechaRegistro
                        }
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error al validar token");
                    return new AuthResponse { Exito = false, Mensaje = "Token inválido o expirado" };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar token");
                return new AuthResponse { Exito = false, Mensaje = "Error al actualizar el token" };
            }
        }

        private string GenerateJwtToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey no configurada"));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol.ToLower()),
                new Claim("rol", usuario.Rol.ToLower())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }
    }
} 