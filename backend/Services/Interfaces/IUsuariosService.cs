using System.Collections.Generic;
using System.Threading.Tasks;
using Ferremas.Api.DTOs;

namespace Ferremas.Api.Services.Interfaces
{
    public interface IUsuariosService
    {
        Task<IEnumerable<UsuarioResponseDTO>> GetAll();
        Task<UsuarioResponseDTO?> GetById(int id);
        Task<UsuarioResponseDTO> Create(UsuarioCreateDTO usuarioDto);
        Task<UsuarioResponseDTO?> Update(int id, UsuarioUpdateDTO usuarioDto);
        Task<bool> Delete(int id);
        Task<string?> Autenticar(string email, string password);
    }
} 