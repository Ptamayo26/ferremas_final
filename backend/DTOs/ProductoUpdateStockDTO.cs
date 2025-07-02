using System.ComponentModel.DataAnnotations;

namespace Ferremas.Api.DTOs
{
    public class ProductoUpdateStockDTO
    {
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor o igual a 0")]
        public int Cantidad { get; set; }
    }
} 