using System.ComponentModel.DataAnnotations;

namespace Ferremas.Api.Models
{
    public class Descuento
    {
        public int Id { get; set; }

        [Required]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        public decimal Porcentaje { get; set; }

        [Required]
        public string Tipo { get; set; } = "porcentaje"; // 'porcentaje' o 'monto'

        [Required]
        public decimal Valor { get; set; } // porcentaje (ej: 10 para 10%) o monto fijo

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public bool Activo { get; set; } = true;
    }
} 