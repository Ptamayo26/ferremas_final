using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferremas.Api.Models
{
    public class Carrito
    {
        public int Id { get; set; }
        
        [Required]
        public int UsuarioId { get; set; }
        
        [Required]
        public int ProductoId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }
        
        [Required]
        public DateTime FechaAgregado { get; set; } = DateTime.Now;
        
        public bool Activo { get; set; } = true;

        // Relaciones
        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }
        
        [ForeignKey("ProductoId")]
        public virtual Producto? Producto { get; set; }
    }
}