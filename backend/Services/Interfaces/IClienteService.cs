using System.Collections.Generic;
using System.Threading.Tasks;
using Ferremas.Api.DTOs;

namespace Ferremas.Api.Services.Interfaces
{
    public interface IClienteService
    {
        Task<IEnumerable<ClienteResponseDTO>> GetAll();
        Task<ClienteResponseDTO?> GetById(int id);
        Task<ClienteResponseDTO> Create(ClienteCreateDTO clienteDto);
        Task<ClienteResponseDTO?> Update(int id, ClienteUpdateDTO clienteDto);
        Task<bool> Delete(int id);
    }
} 