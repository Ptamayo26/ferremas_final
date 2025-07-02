using System;

namespace Ferremas.Api.DTOs
{
    public class ProductoVentaDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public int CantidadVendida { get; set; }
        public decimal TotalVendido { get; set; }
    }

    public class ProductoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public int StockMinimo { get; set; }
        public string? ImagenUrl { get; set; }
        public bool Activo { get; set; }
        public int CategoriaId { get; set; }
        public int MarcaId { get; set; }
    }
} 