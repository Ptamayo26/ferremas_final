using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferremas.Api.Models
{
    [Table("logs")]
    public class Log
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        
        [Required]
        [Column("nivel")]
        [StringLength(50)]
        public string Nivel { get; set; } = string.Empty;
        
        [Required]
        [Column("mensaje")]
        [StringLength(1000)]
        public string Mensaje { get; set; } = string.Empty;
        
        [Column("excepcion")]
        [StringLength(4000)]
        public string Excepcion { get; set; } = string.Empty;
        
        [Required]
        [Column("fecha")]
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        
        public string? UsuarioId { get; set; }
        
        public string? IpAddress { get; set; }
    }
}