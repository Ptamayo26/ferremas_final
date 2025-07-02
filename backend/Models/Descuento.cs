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

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public bool Activo { get; set; } = true;
    }
} 