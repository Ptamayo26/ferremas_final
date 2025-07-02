using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferremas.Api.Models
{
    [Table("pedidos")]
    public class Pedido
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        
        [Column("usuario_id")]
        public int? UsuarioId { get; set; } // Nullable según tu BD
        
        [Column("fecha_pedido")]
        public DateTime? FechaPedido { get; set; } // Nullable según tu BD
        
        [Column("estado")]
        [StringLength(20)]
        public string? Estado { get; set; } // Nullable según tu BD
        
        [Required]
        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; }
        
        [Column("fecha_modificacion")]
        public DateTime? FechaModificacion { get; set; }
        
        [Column("activo")]
        public bool Activo { get; set; }
        
        [Column("repartidor_id")]
        public int? RepartidorId { get; set; }
        
        [Column("total", TypeName = "decimal(10,2)")] // Según tu BD
        public decimal? Total { get; set; } // Nullable según tu BD
        
        [Column("observaciones")]
        [StringLength(500)]
        public string? Observaciones { get; set; }
        
        [Column("direccion_entrega")]
        [StringLength(200)]
        public string? DireccionEntrega { get; set; }
        
        [Column("numero_pedido")]
        [StringLength(50)]
        public string? NumeroPedido { get; set; }
        
        [Column("nombre_cliente_temporal")]
        [StringLength(100)]
        public string? NombreClienteTemporal { get; set; }
        
        // Navigation properties
        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }
        
        public virtual ICollection<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
    }
}