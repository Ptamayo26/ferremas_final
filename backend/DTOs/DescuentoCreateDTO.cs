using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Ferremas.Api.DTOs
{
    public class DescuentoCreateDTO
    {
        [Required]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        public string Tipo { get; set; } = "porcentaje"; // 'porcentaje' o 'monto'

        [Required]
        public decimal Valor { get; set; } // porcentaje (ej: 10 para 10%) o monto fijo

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }
    }

    public class ValidarDescuentoAnonimoDTO
    {
        public string Codigo { get; set; } = string.Empty;
        public List<CarritoItemDTO> Productos { get; set; } = new List<CarritoItemDTO>();
    }
} 