using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferremas.Api.Models
{
    [Table("productos")]
    public class Producto
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        [Column("codigo")]
        public required string Codigo { get; set; }
        
        [Required]
        [StringLength(150)]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;
        
        [Column("descripcion")]
        public string? Descripcion { get; set; }
        
        [Required]
        [Column("precio", TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }
        
        [Column("stock")]
        public int Stock { get; set; }
        
        [Column("stock_minimo")]
        public int StockMinimo { get; set; } = 1;
        
        [StringLength(200)]
        [Column("ImagenUrl")]
        public string ImagenUrl { get; set; } = "Sin imagen";
        
        [StringLength(1000)]
        [Column("especificaciones")]
        public string? Especificaciones { get; set; }
        
        [Required]
        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        
        [Column("fecha_modificacion")]
        public DateTime? FechaModificacion { get; set; }
        
        [Column("activo")]
        public bool Activo { get; set; } = true;

        // FOREIGN KEYS - Solo las propiedades, SIN atributos ForeignKey aquí
        [Column("categoria_id")]
        public int? CategoriaId { get; set; }
        
        [Column("marca_id")]
        public int? MarcaId { get; set; }
        
        // NAVIGATION PROPERTIES - SIN atributos ForeignKey
        public virtual Categoria? Categoria { get; set; }
        public virtual Marca? Marca { get; set; }

        public virtual ICollection<DetallePedido> DetallesPedido { get; set; } = new List<DetallePedido>();
    }
}