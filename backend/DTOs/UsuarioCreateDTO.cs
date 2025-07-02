// UsuarioCreateDTO.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace Ferremas.Api.DTOs
{
    public class UsuarioCreateDTO
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;

        public string? Apellido { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string? Rut { get; set; }
        
        public string? Telefono { get; set; }

        public string? Rol { get; set; }

        public DateTime? FechaRegistro { get; set; }
    }
}