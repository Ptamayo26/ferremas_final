using Ferremas.Api.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ferremas.Api.Services.Interfaces
{
    public interface IProveedoresService
    {
        Task<List<ProveedorResponseDTO>> ObtenerTodos();
        Task<ProveedorResponseDTO?> ObtenerPorId(int id);
        Task<ProveedorResponseDTO> Crear(ProveedorCreateDTO dto);
        Task<bool> Eliminar(int id);
    }
} 