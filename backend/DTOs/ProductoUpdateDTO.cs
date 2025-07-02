namespace Ferremas.Api.DTOs
{
    public class ProductoUpdateDTO
    {
        public string? Codigo { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public decimal? Precio { get; set; }
        public int? Stock { get; set; }
        public int? CategoriaId { get; set; }
        public int? MarcaId { get; set; }
        public string? ImagenUrl { get; set; }
        public string? Especificaciones { get; set; }
    }
} 