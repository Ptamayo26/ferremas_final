using Ferremas.Api.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ferremas.Api.Services.Interfaces
{
    public interface IDescuentoService
    {
        Task<List<DescuentoResponseDTO>> ObtenerTodos();
        Task<DescuentoResponseDTO?> ObtenerPorCodigo(string codigo);
        Task<DescuentoResponseDTO> Crear(DescuentoCreateDTO dto);
        Task<bool> Desactivar(int id);
        Task<DescuentoResponseDTO?> ObtenerPorId(int id);
        Task<bool> Actualizar(int id, DescuentoCreateDTO dto);
        Task<bool> Eliminar(int id);
    }
} 