using System.ComponentModel.DataAnnotations;

namespace Ferremas.Api.DTOs
{
    public class UsuarioUpdateDTO
    {
        [Required]
        [StringLength(100)]
        public string? Nombre { get; set; }

        [StringLength(100)]
        public string? Apellido { get; set; }

        [StringLength(20)]
        public string? Telefono { get; set; }

        public string? Rol { get; set; }

        public bool? Activo { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        public DateTime? FechaRegistro { get; set; }

        public string? Rut { get; set; }

        public DateTime? UltimoAcceso { get; set; }
    }
} 