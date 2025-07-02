namespace Ferremas.Api.Models
{
    public class MercadoPagoCreateRequest
    {
        public decimal Monto { get; set; }
        public string? MetodoPago { get; set; }
        public string Token { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? Email { get; set; }
        public int? PedidoId { get; set; }
    }
} 