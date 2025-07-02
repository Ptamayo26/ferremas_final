using System.Collections.Generic;
using System.Threading.Tasks;
using Ferremas.Api.DTOs;

namespace Ferremas.Api.Services.Interfaces
{
    public interface IReportesService
    {
        Task<decimal> ObtenerVentasTotalesMes(int anio, int mes);
        Task<List<ProductoVentaDTO>> ObtenerTopProductosVendidos(int cantidad);
        Task<Dictionary<string, int>> ObtenerPedidosPorCliente();
        Task<List<ProductoDTO>> ObtenerProductosBajoStock();
    }
} 