using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferremas.Api.Models
{
    public class DatosFacturaEmpresa
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PedidoId { get; set; }
        [ForeignKey("PedidoId")]
        public Pedido Pedido { get; set; } = null!;

        [Required]
        public string RazonSocial { get; set; } = string.Empty;
        [Required]
        public string Rut { get; set; } = string.Empty;
        [Required]
        public string Giro { get; set; } = string.Empty;
        [Required]
        public string Direccion { get; set; } = string.Empty;
    }
} 