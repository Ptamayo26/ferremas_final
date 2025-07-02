using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Ferremas.Api.DTOs
{
    public class PedidoResponseDTO
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string? UsuarioNombre { get; set; }
        public DateTime FechaPedido { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
        public string? DireccionEntrega { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public bool Activo { get; set; }
        public List<DetallePedidoResponseDTO> Detalles { get; set; } = new();
    }

    public class PedidoCreateDTO
    {
        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public decimal Total { get; set; }

        [Required]
        public string Estado { get; set; } = string.Empty;

        public string? Observaciones { get; set; }
        public string? DireccionEntrega { get; set; }
        public List<DetallePedidoCreateDTO> Detalles { get; set; } = new();
    }

    public class PedidoUpdateDTO
    {
        [Required]
        public string Estado { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
        public string? DireccionEntrega { get; set; }
    }

    public class DetallePedidoResponseDTO
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string? ProductoNombre { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public string? Observaciones { get; set; }
    }
} 