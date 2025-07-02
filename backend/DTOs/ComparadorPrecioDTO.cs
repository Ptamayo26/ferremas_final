using System;
using System.ComponentModel.DataAnnotations;

namespace Ferremas.Api.DTOs
{
    public class ComparadorPrecioDTO
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        [Required]
        [StringLength(200)]
        public string ProductoNombre { get; set; } = string.Empty;
        public string? Competidor { get; set; }
        public decimal PrecioCompetidor { get; set; }
        public DateTime FechaConsulta { get; set; }
        public decimal PrecioFerremas { get; set; }
    }

    public class ComparadorPrecioCreateDTO
    {
        public int ProductoId { get; set; }
        public string? Competidor { get; set; }
        public decimal PrecioCompetidor { get; set; }
        public string? TipoFuente { get; set; }
        public int? FuenteId { get; set; }
        public string? UrlProducto { get; set; }
    }

    public class ComparadorPrecioUpdateDTO
    {
        public decimal PrecioCompetidor { get; set; }
    }

    public class ComparadorPrecioResponseDTO
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string? TipoFuente { get; set; }
        public int? FuenteId { get; set; }
        public decimal PrecioCompetidor { get; set; }
        public string? Competidor { get; set; }
        public DateTime FechaConsulta { get; set; }
    }
} 