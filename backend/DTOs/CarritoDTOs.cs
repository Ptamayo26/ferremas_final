using System;
using System.ComponentModel.DataAnnotations;

namespace Ferremas.Api.DTOs
{
    public class CarritoItemDTO
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public decimal ProductoPrecio { get; set; }
        public string? ProductoImagen { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; }
        public DateTime FechaAgregado { get; set; }
    }

    public class AgregarAlCarritoDTO
    {
        [Required]
        public int ProductoId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }
    }

    public class ActualizarCantidadDTO
    {
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad no puede ser negativa")]
        public int Cantidad { get; set; }
    }

    public class CarritoResumenDTO
    {
        public int TotalItems { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public int CantidadProductos { get; set; }
    }
} 