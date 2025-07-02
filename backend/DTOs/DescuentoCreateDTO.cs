using System;
using System.ComponentModel.DataAnnotations;

namespace Ferremas.Api.DTOs
{
    public class DescuentoCreateDTO
    {
        [Required]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 100)]
        public decimal Porcentaje { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }
    }
} 