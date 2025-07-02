using System.ComponentModel.DataAnnotations;

namespace Ferremas.Api.Models
{
    public class Notificacion
    {
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public string Mensaje { get; set; } = string.Empty;

        public bool Leida { get; set; } = false;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Relación con el usuario
        public Usuario? Usuario { get; set; }
    }
} 