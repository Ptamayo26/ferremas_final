using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ferremas.Api.DTOs;
using Ferremas.Api.Services.Interfaces;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Ferremas.Api.Models;
using Ferremas.Api.Data;
using Ferremas.Api.Constants;

namespace Ferremas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuariosService _usuariosService;
        private readonly AppDbContext _context;

        public UsuariosController(IUsuariosService usuariosService, AppDbContext context)
        {
            _usuariosService = usuariosService;
            _context = context;
        }

        [HttpGet]
        [Authorize(Policy = "RequireAdministrador")]
        public async Task<ActionResult<IEnumerable<UsuarioResponseDTO>>> GetUsuarios()
        {
            Console.WriteLine("=== Claims del usuario ===");
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
            }

            var userRoles = User.Claims.Where(c => c.Type == ClaimTypes.Role || c.Type == "role" || c.Type == "rol").Select(c => c.Value.ToLower()).ToList();
            Console.WriteLine("=== Roles encontrados ===");
            foreach (var role in userRoles)
            {
                Console.WriteLine($"Rol: {role}");
            }
            Console.WriteLine($"Rol esperado: {Roles.Administrador}");
            Console.WriteLine($"¿Contiene rol administrador?: {userRoles.Contains(Roles.Administrador)}");

            try
            {
                Console.WriteLine("Iniciando consulta de usuarios...");
                var usuarios = await _context.Usuarios
                    .Where(u => u.Activo != false)
                    .Select(u => new UsuarioResponseDTO
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
                    })
                    .ToListAsync();
                Console.WriteLine($"Consulta completada. Se encontraron {usuarios.Count} usuarios.");
                return usuarios;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener usuarios: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { message = "Error interno del servidor al obtener usuarios" });
            }
        }

        [HttpGet("perfil")]
        public async Task<ActionResult<UsuarioResponseDTO>> GetMiPerfil()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == userId && u.Activo != false);

            if (usuario == null)
                return NotFound();

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

        [HttpGet("{id}")]
        [Authorize(Policy = "RequireAdministrador")]
        public async Task<ActionResult<UsuarioResponseDTO>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == id && u.Activo != false);

            if (usuario == null)
                return NotFound();

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

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<UsuarioResponseDTO>> Create(UsuarioCreateDTO usuarioDto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Rut == usuarioDto.Rut))
                return BadRequest("El RUT ya está registrado");

            if (await _context.Usuarios.AnyAsync(u => u.Email == usuarioDto.Email))
                return BadRequest("El correo electrónico ya está registrado");

            if (usuarioDto.Rol != "administrador" && usuarioDto.Rol != "vendedor" && usuarioDto.Rol != "bodeguero" && usuarioDto.Rol != "contador")
                return BadRequest("Rol no válido");

            var usuario = await _usuariosService.Create(usuarioDto);
            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, usuario);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "RequireAdministrador")]
        public async Task<IActionResult> PutUsuario(int id, UsuarioUpdateDTO usuarioDto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null || !usuario.Activo.GetValueOrDefault())
                return NotFound();

            if (usuarioDto.Email != usuario.Email && 
                await _context.Usuarios.AnyAsync(u => u.Email == usuarioDto.Email))
                return BadRequest("El correo electrónico ya está registrado");

            if (!string.IsNullOrEmpty(usuarioDto.Rol) &&
                usuarioDto.Rol != "administrador" && usuarioDto.Rol != "vendedor" && usuarioDto.Rol != "bodeguero" && usuarioDto.Rol != "contador")
                return BadRequest("Rol no válido");

            // Solo actualiza si el campo viene en el DTO
            if (usuarioDto.Nombre != null) usuario.Nombre = usuarioDto.Nombre;
            if (usuarioDto.Apellido != null) usuario.Apellido = usuarioDto.Apellido;
            if (usuarioDto.Telefono != null) usuario.Telefono = usuarioDto.Telefono;
            if (usuarioDto.Rol != null) usuario.Rol = usuarioDto.Rol;
            if (usuarioDto.Activo.HasValue) usuario.Activo = usuarioDto.Activo;
            if (usuarioDto.Email != null) usuario.Email = usuarioDto.Email;
            if (usuarioDto.FechaRegistro != null) usuario.FechaRegistro = usuarioDto.FechaRegistro.Value;
            if (usuarioDto.Rut != null) usuario.Rut = usuarioDto.Rut;
            if (usuarioDto.UltimoAcceso.HasValue) usuario.UltimoAcceso = usuarioDto.UltimoAcceso;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                    return NotFound();
                else
                    throw;
            }

            return Ok(new { mensaje = "Actualización realizada" });
        }

        [HttpPut("perfil")]
        public async Task<IActionResult> PutMiPerfil(UsuarioUpdateDTO usuarioDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var usuario = await _context.Usuarios.FindAsync(userId);
            if (usuario == null || !usuario.Activo.GetValueOrDefault())
                return NotFound();

            if (usuarioDto.Email != usuario.Email && 
                await _context.Usuarios.AnyAsync(u => u.Email == usuarioDto.Email))
                return BadRequest("El correo electrónico ya está registrado");

            if (!string.IsNullOrEmpty(usuarioDto.Rol) &&
                usuarioDto.Rol != "administrador" && usuarioDto.Rol != "vendedor" && usuarioDto.Rol != "bodeguero" && usuarioDto.Rol != "contador")
                return BadRequest("Rol no válido");

            // Solo actualiza si el campo viene en el DTO
            if (usuarioDto.Nombre != null) usuario.Nombre = usuarioDto.Nombre;
            if (usuarioDto.Apellido != null) usuario.Apellido = usuarioDto.Apellido;
            if (usuarioDto.Telefono != null) usuario.Telefono = usuarioDto.Telefono;
            if (usuarioDto.Rol != null) usuario.Rol = usuarioDto.Rol;
            if (usuarioDto.Activo.HasValue) usuario.Activo = usuarioDto.Activo;
            if (usuarioDto.Email != null) usuario.Email = usuarioDto.Email;
            if (usuarioDto.FechaRegistro != null) usuario.FechaRegistro = usuarioDto.FechaRegistro.Value;
            if (usuarioDto.Rut != null) usuario.Rut = usuarioDto.Rut;
            if (usuarioDto.UltimoAcceso.HasValue) usuario.UltimoAcceso = usuarioDto.UltimoAcceso;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(userId))
                    return NotFound();
                else
                    throw;
            }

            return Ok(new { mensaje = "Actualización realizada" });
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireAdministrador")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            Console.WriteLine($"Controlador: Iniciando eliminación de usuario ID: {id}");
            
            try
            {
                var result = await _usuariosService.Delete(id);
                if (!result)
                {
                    Console.WriteLine($"Controlador: No se pudo eliminar el usuario {id}");
                    return NotFound(new { message = $"No se encontró el usuario con ID {id}" });
                }

                Console.WriteLine($"Controlador: Usuario {id} eliminado exitosamente");
                return Ok(new { message = $"Usuario {id} eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Controlador: Error al eliminar usuario {id}: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor al eliminar el usuario" });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> Login(LoginDTO loginDto)
        {
            var token = await _usuariosService.Autenticar(loginDto.Email, loginDto.Password);
            if (token == null)
                return Unauthorized(new { message = "Credenciales inválidas" });

            return Ok(new { token });
        }

        [HttpPut("reactivar/{id}")]
        [Authorize(Policy = "RequireAdministrador")]
        public async Task<IActionResult> ReactivarUsuario(int id)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
            if (usuario == null)
                return NotFound(new { message = $"No se encontró el usuario con ID {id}" });
            if (usuario.Activo == true)
                return BadRequest(new { message = $"El usuario con ID {id} ya está activo" });

            usuario.Activo = true;
            _context.Entry(usuario).Property(u => u.Activo).IsModified = true;
            try
            {
                Console.WriteLine("Antes de SaveChangesAsync (reactivar)");
                await _context.SaveChangesAsync();
                Console.WriteLine("Después de SaveChangesAsync (reactivar)");
                return Ok(new { message = $"Usuario {id} reactivado exitosamente" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al reactivar usuario: {ex.Message}");
                return StatusCode(500, new { message = "Error interno al reactivar usuario" });
            }
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id && e.Activo == true);
        }
    }
} 