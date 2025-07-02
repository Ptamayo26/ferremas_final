using System;
using System.ComponentModel.DataAnnotations;

namespace Ferremas.Api.DTOs
{
    public class NotificacionCreateDTO
    {
        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public string Mensaje { get; set; } = string.Empty;
    }
} 