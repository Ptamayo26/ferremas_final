using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferremas.Api.Models
{
    public class LogAuditoria
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; } // ID del usuario que realizó la acción

        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Accion { get; set; } = string.Empty; // Ej: "CrearUsuario", "ActualizarRol", "DesactivarUsuario"

        [Required]
        [StringLength(100)]
        public string Entidad { get; set; } = string.Empty; // Ej: "Usuarios", "Productos"

        public int? EntidadId { get; set; } // ID del registro afectado

        [Column(TypeName = "jsonb")]
        public string? Detalles { get; set; } // JSON con los detalles del cambio (valor antiguo, valor nuevo)

        [Required]
        public DateTime FechaHora { get; set; } = DateTime.UtcNow;
    }
} 