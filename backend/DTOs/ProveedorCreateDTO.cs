using System;
using System.ComponentModel.DataAnnotations;

namespace Ferremas.Api.DTOs
{
    public class ProveedorCreateDTO
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;

        public string? Contacto { get; set; }

        public string? Correo { get; set; }

        public string? Telefono { get; set; }

        public string? Direccion { get; set; }
    }
} 