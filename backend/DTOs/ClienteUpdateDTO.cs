using System.ComponentModel.DataAnnotations;

namespace Ferremas.Api.DTOs
{
    public class ClienteUpdateDTO
    {
        [StringLength(100)]
        public string? Nombre { get; set; }

        [StringLength(100)]
        public string? Apellido { get; set; }

        [StringLength(20)]
        public string? Rut { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? CorreoElectronico { get; set; }

        [StringLength(20)]
        public string? Telefono { get; set; }
    }
} 