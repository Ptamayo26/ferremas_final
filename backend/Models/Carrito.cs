using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferremas.Api.Models
{
    public class Carrito
    {
        public int Id { get; set; }
        
        [Required]
        [Column("usuario_id")]
        public int UsuarioId { get; set; }
        
        [Required]
        [Column("producto_id")]
        public int ProductoId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }
        
        [Required]
        [Column("fecha_agregado")]
        public DateTime FechaAgregado { get; set; } = DateTime.Now;
        
        [Column("activo")]
        public bool Activo { get; set; } = true;

        // Relaciones
        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }
        
        [ForeignKey("ProductoId")]
        public virtual Producto? Producto { get; set; }
    }
}