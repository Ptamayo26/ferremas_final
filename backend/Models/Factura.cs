using System.ComponentModel.DataAnnotations;

namespace Ferremas.Api.Models
{
    public class Factura
    {
        public int Id { get; set; }

        [Required]
        public int PedidoId { get; set; }

        [Required]
        public DateTime FechaEmision { get; set; } = DateTime.Now;

        [Required]
        public decimal MontoTotal { get; set; }

        public bool Anulada { get; set; } = false;

        // Relaciones
        public Pedido? Pedido { get; set; }
    }
} 