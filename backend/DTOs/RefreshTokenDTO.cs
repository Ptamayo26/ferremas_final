using System.ComponentModel.DataAnnotations;

namespace Ferremas.Api.DTOs
{
    /// <summary>
    /// Modelo para solicitar la actualización del token JWT
    /// </summary>
    public class RefreshTokenDTO
    {
        /// <summary>
        /// Token de refresco
        /// </summary>
        [Required(ErrorMessage = "El token de refresco es requerido")]
        public string RefreshToken { get; set; } = string.Empty;
    }
} 