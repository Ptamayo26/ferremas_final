using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferremas.Api.Models
{
    [Table("comparadorprecios")]
    public class ComparadorPrecio
    {
        [Key]
        public int Id { get; set; }
        public int ProductoId { get; set; }
        [MaxLength(50)]
        public string? TipoFuente { get; set; }
        public int? FuenteId { get; set; }
        [MaxLength(100)]
        public string? Competidor { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal PrecioCompetidor { get; set; }
        public DateTime FechaConsulta { get; set; } = DateTime.UtcNow;
        [MaxLength(500)]
        public string? UrlProducto { get; set; }
        public virtual Producto Producto { get; set; } = null!;
    }
}