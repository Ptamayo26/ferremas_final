using System;
using System.Collections.Generic;

namespace Ferremas.Api.DTOs
{
    public class ClienteResponseDTO
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public required string Apellido { get; set; }
        public required string Rut { get; set; }
        public required string CorreoElectronico { get; set; }
        public string? Telefono { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public bool Activo { get; set; }
        public int? UsuarioId { get; set; }
        public ICollection<DireccionDTO>? Direcciones { get; set; }
    }
} 