using System.ComponentModel.DataAnnotations;

namespace Ferremas.Api.DTOs
{
    public class VerificarTransferenciaDTO
    {
        [Required]
        public string NumeroTransferencia { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal Monto { get; set; }
    }
} 