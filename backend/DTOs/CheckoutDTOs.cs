using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Ferremas.Api.DTOs
{
    public class CheckoutRequestDTO
    {
        [Required]
        public int ClienteId { get; set; }
        
        [Required]
        public int DireccionId { get; set; }
        
        [Required]
        public string MetodoPago { get; set; } = string.Empty; // "mercadopago", "efectivo", "transferencia"
        
        public string? Observaciones { get; set; }
        
        public string? CodigoDescuento { get; set; }

        public List<CarritoItemDTO> Items { get; set; } = new List<CarritoItemDTO>();
    }

    public class CheckoutResponseDTO
    {
        public int PedidoId { get; set; }
        public string NumeroPedido { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public string? UrlPago { get; set; } // Para MercadoPago
        public string? CodigoPago { get; set; } // Para MercadoPago
    }

    public class CheckoutResumenDTO
    {
        public List<CarritoItemDTO> Items { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal Impuestos { get; set; }
        public decimal Envio { get; set; }
        public decimal Total { get; set; }
        public int TotalItems { get; set; }
        public ClienteResumenDTO Cliente { get; set; } = new();
        public DireccionDTO DireccionEnvio { get; set; } = new();
    }

    public class ClienteResumenDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Rut { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Telefono { get; set; }
    }

    public class ProcesarPagoRequestDTO
    {
        [Required]
        public int PedidoId { get; set; }
        
        [Required]
        public string MetodoPago { get; set; } = string.Empty;
        
        public string? TokenPago { get; set; } // Para MercadoPago
        public string? PayerId { get; set; } // Para MercadoPago
    }

    public class ProcesarPagoResponseDTO
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public string? UrlPago { get; set; }
        public string? CodigoPago { get; set; }
        public string EstadoPago { get; set; } = string.Empty;
    }
} 