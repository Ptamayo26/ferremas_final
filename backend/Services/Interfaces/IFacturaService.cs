using Ferremas.Api.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ferremas.Api.Services.Interfaces
{
    public interface IFacturaService
    {
        Task<List<FacturaResponseDTO>> ObtenerTodas();
        Task<FacturaResponseDTO?> ObtenerPorId(int id);
        Task<FacturaResponseDTO> Crear(FacturaCreateDTO dto);
        Task<bool> Anular(int id);
    }
} 