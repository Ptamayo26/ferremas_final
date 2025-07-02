using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferremas.Api.Models
{
    [Table("Pagos")]
    public class Pago
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [Column("pedido_id")]
        public int PedidoId { get; set; }
        
        [Required]
        [Column("monto")]
        public decimal Monto { get; set; }
        
        [Required]
        [Column("metodo_pago")]
        [StringLength(50)]
        public string MetodoPago { get; set; } = string.Empty;
        
        [Column("transaccion_id")]
        [StringLength(100)]
        public string? TransaccionId { get; set; }
        
        [Column("token_pasarela")]
        [StringLength(100)]
        public string? TokenPasarela { get; set; }
        
        [Column("datos_respuesta")]
        public string? DatosRespuesta { get; set; }
        
        [Required]
        [Column("fecha_pago")]
        public DateTime FechaPago { get; set; }
        
        [Required]
        [Column("estado")]
        public string Estado { get; set; } = "PENDIENTE";
        
        // Relaciones
        [ForeignKey("PedidoId")]
        public virtual Pedido? Pedido { get; set; }
    }
}