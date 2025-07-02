using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferremas.Api.Models
{
    [Table("categorias")]
    public class Categoria
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;
        
        [StringLength(500)]
        [Column("descripcion")]
        public string? Descripcion { get; set; }
        
        [Column("categoria_padre_id")]
        public int? CategoriaPadreId { get; set; }
        
        [StringLength(10)]
        [Column("codigo")]
        public string Codigo { get; set; } = string.Empty;
        
        [Column("activo")]
        public bool Activo { get; set; } = true;
        
        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        
        [Column("fecha_modificacion")]
        public DateTime? FechaModificacion { get; set; }

        // Navigation properties
        [ForeignKey("CategoriaPadreId")]
        public virtual Categoria? CategoriaPadre { get; set; }
        
        public virtual ICollection<Categoria> Subcategorias { get; set; } = new List<Categoria>();
        public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}