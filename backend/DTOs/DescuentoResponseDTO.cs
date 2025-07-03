using System;

namespace Ferremas.Api.DTOs
{
    public class DescuentoResponseDTO
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool Activo { get; set; }
        public string Tipo { get; set; } = "porcentaje"; // 'porcentaje' o 'monto'
        public decimal Valor { get; set; } // porcentaje (ej: 10 para 10%) o monto fijo
    }
} 