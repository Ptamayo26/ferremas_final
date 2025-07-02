using System.ComponentModel.DataAnnotations;

namespace FerremasBackend.DTOs
{
    public class ComparadorPrecioDTO
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public string NombreTienda { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string UrlProducto { get; set; } = string.Empty;
        public DateTime FechaComparacion { get; set; }
    }

    public class ComparadorPrecioCreateDTO
    {
        [Required]
        public int ProductoId { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreTienda { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Precio { get; set; }

        [Required]
        [StringLength(200)]
        public string UrlProducto { get; set; } = string.Empty;
    }

    public class ComparadorPrecioUpdateDTO
    {
        [Required]
        [StringLength(100)]
        public string NombreTienda { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Precio { get; set; }

        [Required]
        [StringLength(200)]
        public string UrlProducto { get; set; } = string.Empty;
    }

    public class ComparadorPrecioResponseDTO
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public string NombreTienda { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string UrlProducto { get; set; } = string.Empty;
        public DateTime FechaComparacion { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string ImagenUrl { get; set; } = string.Empty;
    }

    public class ProductoResponseDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string Tienda { get; set; } = string.Empty;
        public string UrlProducto { get; set; } = string.Empty;
        public string ImagenUrl { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
    }

    public class ComparacionResultado
    {
        public string Tienda { get; set; }
        public decimal Precio { get; set; }
        public decimal Diferencia { get; set; }
        public decimal PorcentajeAhorro { get; set; }
    }
} 