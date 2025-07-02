using System.Collections.Generic;
using System.Threading.Tasks;
using Ferremas.Api.DTOs;

namespace Ferremas.Api.Services.Interfaces
{
    public interface IPedidosService
    {
        Task<IEnumerable<PedidoResponseDTO>> GetAll();
        Task<PedidoResponseDTO?> GetById(int id);
        Task<PedidoResponseDTO> Create(PedidoCreateDTO pedidoDto);
        Task<PedidoResponseDTO?> Update(int id, PedidoUpdateDTO pedidoDto);
        Task<bool> Delete(int id);
        Task<IEnumerable<PedidoResponseDTO>> GetByUsuario(int usuarioId);
    }
} 