using System.ComponentModel.DataAnnotations;

namespace Ferremas.Api.DTOs
{
    public class ClienteCreateDTO
    {
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Rut { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string CorreoElectronico { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Telefono { get; set; } = string.Empty;

        [Required]
        public List<DireccionDTO> Direcciones { get; set; } = new();
    }
}