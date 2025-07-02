using Ferremas.Api.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ferremas.Api.Services.Interfaces
{
    public interface IProductosService
    {
        Task<IEnumerable<ProductoResponseDTO>> GetAll();
        Task<ProductoResponseDTO?> GetById(int id);
        Task<ProductoResponseDTO> Create(ProductoCreateDTO productoDto);
        Task<ProductoResponseDTO?> Update(int id, ProductoUpdateDTO productoDto);
        Task<ProductoResponseDTO?> UpdateStock(int id, int cantidad);
        Task<bool> Delete(int id);
        Task<IEnumerable<ProductoResponseDTO>> GetByCategoria(int categoriaId);
        Task<IEnumerable<ProductoResponseDTO>> GetByMarca(int marcaId);
        Task<IEnumerable<ProductoResponseDTO>> Search(string termino);
    }
}
