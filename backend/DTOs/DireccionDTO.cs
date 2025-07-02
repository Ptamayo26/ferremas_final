using System;
using System.ComponentModel.DataAnnotations;

namespace Ferremas.Api.DTOs
{
    public class DireccionDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Calle { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Numero { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Departamento { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Comuna { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Region { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string CodigoPostal { get; set; } = string.Empty;

        public bool EsPrincipal { get; set; }

        public DateTime? FechaModificacion { get; set; }
    }
} 