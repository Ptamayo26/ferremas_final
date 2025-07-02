using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferremas.Api.Models
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        [Column("telefono")]
        public string? Telefono { get; set; }

        [Column("rut")]
        public string? Rut { get; set; }

        [Column("correo_electronico")]
        public string? CorreoElectronico { get; set; }

        [Column("nombre")]
        public string? Nombre { get; set; }

        [Column("apellido")]
        public string? Apellido { get; set; }

        [Column("fecha_creacion")]
        public DateTime? FechaCreacion { get; set; }

        [Column("fecha_modificacion")]
        public DateTime? FechaModificacion { get; set; }

        [Column("activo")]
        public bool? Activo { get; set; }

        [Column("usuario_id")]
        public int? UsuarioId { get; set; }

        // Relaciones
        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }
    }
}