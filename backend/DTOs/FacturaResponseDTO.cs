using System;
using System.ComponentModel.DataAnnotations;

namespace Ferremas.Api.DTOs
{
    public class FacturaResponseDTO
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public DateTime FechaEmision { get; set; }
        public decimal MontoTotal { get; set; }
        public bool Anulada { get; set; }
    }
} 