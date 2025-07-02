using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferremas.Api.Models
{
    public class Tienda
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Descripcion { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Url { get; set; } = string.Empty;

        [Required]
        public DateTime FechaCreacion { get; set; }

        // Navigation properties
        public virtual ICollection<ComparadorPrecio> Precios { get; set; } = new List<ComparadorPrecio>();
    }
} 