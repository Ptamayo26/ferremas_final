using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferremas.Api.Models
{
    [Table("marcas")]
    public class Marca
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        [Column("nombre")]
        public required string Nombre { get; set; }
        
        [Column("descripcion")]
        public string? Descripcion { get; set; }
        
        [StringLength(255)]
        [Column("logo_url")]
        public string? LogoUrl { get; set; }
        
        [Column("activo")]
        public bool Activo { get; set; } = true;

        // Navigation property
        public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}