using Ferremas.Api.Models;

namespace FerremasBackend.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(Usuario usuario);
    }
} 