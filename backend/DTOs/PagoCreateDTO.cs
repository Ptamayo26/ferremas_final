using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Ferremas.Api.DTOs
{
    public class PagoCreateDTO
    {
        [Required]
        public int PedidoId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal Monto { get; set; }

        [Required]
        public string MetodoPago { get; set; } = string.Empty;

        public string? TransaccionId { get; set; }

        public string? TokenPasarela { get; set; }

        public string? DatosRespuesta { get; set; }

        public string Descripcion { get; set; }

        public Dictionary<string, string> DatosAdicionales { get; set; }
    }
} 