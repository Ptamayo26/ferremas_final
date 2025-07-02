using System;

namespace Ferremas.Api.DTOs
{
    public class NotificacionResponseDTO
    {
        public int Id { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public bool Leida { get; set; }
    }
} 