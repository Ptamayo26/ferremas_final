using System.ComponentModel.DataAnnotations;

namespace Ferremas.Api.DTOs
{
    public class PedidoDetalleCreateDTO
    {
        [Required]
        public int ProductoId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }
    }
} 