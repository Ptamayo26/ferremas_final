using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferremas.Api.Models
{
    public class ComparacionResultado
    {
        [Required]
        public string Tienda { get; set; } = string.Empty;

        [Required]
        public string NombreProducto { get; set; } = string.Empty;

        [Required]
        public decimal Precio { get; set; }

        [Required]
        public string Enlace { get; set; } = string.Empty;

        public string? Imagen { get; set; }

        public string? Vendedor { get; set; }

        public decimal? ReputacionVendedor { get; set; }

        public decimal DiferenciaPrecio { get; set; }

        public decimal PorcentajeDiferencia { get; set; }

        public string ReputacionVendedorFormatted => ReputacionVendedor?.ToString("0.0") ?? "N/A";

        public int CantidadVentas { get; set; }
        public string Condicion { get; set; } = ""; // Nuevo, Usado, etc.
        public bool EnvioGratis { get; set; }
        public string Ubicacion { get; set; } = "";
        public string IdProducto { get; set; } = "";
        public string EstadoCompetitividad { get; set; } = "";
        public int CantidadDisponible { get; set; }
        public DateTime FechaConsulta { get; set; } = DateTime.UtcNow;
    }
} 