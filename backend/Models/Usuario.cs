using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferremas.Api.Models
{
    [Table("usuarios")]
    public class Usuario
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        
        [Required]
        [Column("nombre")]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;
        
        [Column("apellido")]
        [StringLength(50)]
        public string? Apellido { get; set; }
        
        [Required]
        [Column("email")]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [Column("password")]
        [StringLength(100)]
        public string PasswordHash { get; set; } = string.Empty;
        
        [Column("rut")]
        [StringLength(20)]
        public string? Rut { get; set; }
        
        [Column("telefono")]
        [StringLength(20)]
        public string? Telefono { get; set; }
        
        [Column("rol")]
        [Required]
        [StringLength(20)]
        public string Rol { get; set; } = "Cliente";
        
        [Column("activo")]
        public bool? Activo { get; set; } = true;
        
        [Column("fecha_registro")]
        public DateTime? FechaRegistro { get; set; }
        
        [Column("ultimo_acceso")]
        public DateTime? UltimoAcceso { get; set; }

        public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
        public virtual ICollection<Direccion> Direcciones { get; set; } = new List<Direccion>();
    }
}