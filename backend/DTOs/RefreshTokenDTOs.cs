namespace Ferremas.Api.DTOs
{
    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class RefreshTokenResponse
    {
        public bool Exito { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public string? Mensaje { get; set; }
    }

    public class TokenPair
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
} 