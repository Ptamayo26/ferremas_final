using System;
using System.ComponentModel.DataAnnotations;

namespace Ferremas.Api.DTOs
{
    public class ProductoCreateDTO
    {
        public required string Codigo { get; set; }
        public required string Nombre { get; set; }
        public required string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public int CategoriaId { get; set; }
        public int MarcaId { get; set; }
        public string? ImagenUrl { get; set; }
        public string? Especificaciones { get; set; }
    }
}