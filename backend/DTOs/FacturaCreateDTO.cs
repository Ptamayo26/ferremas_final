using System;
using System.ComponentModel.DataAnnotations;

namespace Ferremas.Api.DTOs
{
    public class FacturaCreateDTO
    {
        [Required]
        public int PedidoId { get; set; }

        [Required]
        public decimal MontoTotal { get; set; }
    }
} 