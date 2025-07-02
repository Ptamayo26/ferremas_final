using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferremas.Api.Models
{
    public class ComparacionHistorial
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(200)]
        [Column("nombre_producto")]
        public string Producto { get; set; } = string.Empty;

        [Required]
        [Column("precio_ferremas", TypeName = "decimal(18,2)")]
        public decimal PrecioFerremas { get; set; }

        [Required]
        [Column("resultado_json")]
        public string Resultados { get; set; } = string.Empty;

        [Required]
        [Column("fecha")]
        public DateTime FechaComparacion { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }
    }
} 