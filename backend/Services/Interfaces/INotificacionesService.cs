using Ferremas.Api.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ferremas.Api.Services.Interfaces
{
    public interface INotificacionesService
    {
        Task<List<NotificacionResponseDTO>> ObtenerPorUsuario(int usuarioId);
        Task<NotificacionResponseDTO> Crear(NotificacionCreateDTO dto);
        Task<bool> MarcarComoLeida(int id);
    }
} 