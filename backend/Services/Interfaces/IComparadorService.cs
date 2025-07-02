using System.Collections.Generic;
using System.Threading.Tasks;
using Ferremas.Api.DTOs;
using Ferremas.Api.Models;

namespace Ferremas.Api.Services.Interfaces
{
    public interface IComparadorService
    {
        Task<IEnumerable<ComparadorPrecioDTO>> GetAllPreciosAsync();
        Task<ComparadorPrecioDTO?> GetPrecioByIdAsync(int id);
        Task<ComparadorPrecioDTO> CreatePrecioAsync(ComparadorPrecioCreateDTO dto);
        Task<ComparadorPrecioDTO?> UpdatePrecioAsync(int id, ComparadorPrecioUpdateDTO dto);
        Task<bool> DeletePrecioAsync(int id);
        Task<IEnumerable<ComparadorPrecioDTO>> GetPreciosByProductoAsync(int productoId);
        Task<IEnumerable<ComparadorPrecio>> GetComparacionesByProducto(int productoId);
        Task<ComparadorPrecio?> GetComparacionById(int id);
        Task<IEnumerable<ProductoResponseDTO>> CompararPrecios(int productoId);
        Task<IEnumerable<object>> ObtenerHistorialPrecios(int productoId);
        Task<ComparadorPrecioResponseDTO?> GetComparacionResponseDTO(int id);
        Task GuardarHistorialComparacion(int userId, string producto, decimal precioFerremas, List<ComparacionResultado> resultados);
        Task<List<ComparacionHistorial>> ObtenerHistorialComparacion(int userId);
        Task<List<ComparacionHistorial>> ObtenerHistorialUsuario(int userId);
        List<ComparacionHistorial> ObtenerHistorialTodos();
    }
}
