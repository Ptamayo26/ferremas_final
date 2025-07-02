using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferremas.Api.Models
{
    [Table("listas_deseos")]
    public class ListaDeseos
    {
        [Key]
        public int Id { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Column("producto_id")]
        public int ProductoId { get; set; }

        [Column("fecha_agregado")]
        public DateTime FechaAgregado { get; set; } = DateTime.Now;

        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }

        [ForeignKey("ProductoId")]
        public virtual Producto Producto { get; set; }
    }
} 