using System.Threading.Tasks;
using Ferremas.Api.DTOs;

namespace Ferremas.Api.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de autenticación
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Registra un nuevo usuario
        /// </summary>
        Task<AuthResponse> RegisterAsync(UsuarioCreateDTO registerDto);

        /// <summary>
        /// Autentica un usuario
        /// </summary>
        Task<AuthResponse> LoginAsync(LoginDTO loginDto);

        /// <summary>
        /// Restablece la contraseña de un usuario
        /// </summary>
        Task<AuthResponse> ResetPasswordAsync(ResetPasswordDTO resetDto);

        /// <summary>
        /// Actualiza el token JWT
        /// </summary>
        Task<AuthResponse> RefreshTokenAsync(RefreshTokenDTO model);

        /// <summary>
        /// Revoca un refresh token
        /// </summary>
        Task<bool> RevokeRefreshTokenAsync(string refreshToken);
    }
} 