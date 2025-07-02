using System.Threading.Tasks;
using Ferremas.Api.DTOs;

namespace Ferremas.Api.Services.Interfaces
{
    public interface IEnvioService
    {
        Task<EnvioDTO> CrearEnvioAsync(EnvioCreateDTO dto);
        Task<EnvioDTO> ObtenerEnvioPorPedidoAsync(int pedidoId);
        Task<bool> ActualizarEstadoEnvioAsync(int envioId, string nuevoEstado);
    }
}