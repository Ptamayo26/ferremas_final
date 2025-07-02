using System;
using Ferremas.Api.DTOs;

namespace Ferremas.Api.DTOs
{
    /// <summary>
    /// Modelo de respuesta para la autenticación
    /// </summary>
    public class AuthResponse : ApiResponse
    {
        /// <summary>
        /// Token JWT de autenticación
        /// </summary>
        public string? Token { get; set; }

        /// <summary>
        /// Datos del usuario autenticado
        /// </summary>
        public UsuarioResponseDTO? Usuario { get; set; }

        /// <summary>
        /// Fecha y hora de la respuesta
        /// </summary>
        public DateTime FechaHora { get; set; } = DateTime.UtcNow;
    }
} 