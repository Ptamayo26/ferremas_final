using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferremas.Api.Models
{
    [Table("direcciones")]
    public class Direccion
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        
        [Column("cliente_id")]
        public int? ClienteId { get; set; }
        
        [Column("usuario_id")]
        public int? UsuarioId { get; set; }
        
        [Required]
        [StringLength(150)]
        [Column("calle")]
        public string Calle { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        [Column("numero")]
        public string Numero { get; set; } = string.Empty;
        
        [StringLength(50)]
        [Column("departamento")]
        public string? Departamento { get; set; }
        
        [Required]
        [StringLength(100)]
        [Column("comuna")]
        public string Comuna { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        [Column("region")]
        public string Region { get; set; } = string.Empty;
        
        [StringLength(20)]
        [Column("codigo_postal")]
        public string? CodigoPostal { get; set; }
        
        [Column("es_principal")]
        public bool EsPrincipal { get; set; } = false;
        
        [Column("FechaCreacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        
        [Column("FechaModificacion")]
        public DateTime? FechaModificacion { get; set; }
        
        // Relaciones
        [ForeignKey("ClienteId")]
        public virtual Cliente? Cliente { get; set; }
        
        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }
    }
}