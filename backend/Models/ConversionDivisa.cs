using System;
using System.ComponentModel.DataAnnotations;

namespace Ferremas.Api.Models
{
    public class ConversionDivisa
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(10)]
        public string MonedaOrigen { get; set; } = string.Empty;
        
        [Required]
        [StringLength(10)]
        public string MonedaDestino { get; set; } = string.Empty;
        
        [Required]
        public decimal TasaCambio { get; set; }
        
        public DateTime FechaActualizacion { get; set; } = DateTime.Now;
        
        public bool Activo { get; set; } = true;
    }
}