using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferremas.Api.Models
{
    [Table("detallespedido")]
    public class DetallePedido
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        
        [Column("pedido_id")]
        public int? PedidoId { get; set; } // Nullable según tu BD
        
        [Column("producto_id")]
        public int? ProductoId { get; set; } // Nullable según tu BD
        
        [Column("cantidad")]
        public int? Cantidad { get; set; } // Nullable según tu BD
        
        [Column("precio_unitario", TypeName = "decimal(10,2)")]
        public decimal? PrecioUnitario { get; set; } // Nullable según tu BD
        
        [Column("subtotal", TypeName = "decimal(10,2)")]
        public decimal? Subtotal { get; set; } // Nullable según tu BD
        
        [Column("observaciones")]
        [StringLength(255)]
        public string? Observaciones { get; set; }

        // Navigation properties
        [ForeignKey("PedidoId")]
        public virtual Pedido? Pedido { get; set; }
        
        [ForeignKey("ProductoId")]
        public virtual Producto? Producto { get; set; }
    }
} 