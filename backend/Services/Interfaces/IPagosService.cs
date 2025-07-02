using System.Collections.Generic;
using System.Threading.Tasks;
using Ferremas.Api.Models;
using Ferremas.Api.DTOs;

namespace Ferremas.Api.Services.Interfaces
{
    public interface IPagosService
    {
        Task<IEnumerable<Pago>> GetAll();
        Task<Pago?> GetById(int id);
        Task<Pago> Create(PagoCreateDTO pagoDto);
        Task<bool> Delete(int id);
        Task<bool> ProcesarPago(int pedidoId, decimal monto);
        Task<bool> VerificarTransferencia(string numeroTransferencia, decimal monto);
        Task<Pago?> GetByTokenPasarela(string tokenPasarela);
    }
} 