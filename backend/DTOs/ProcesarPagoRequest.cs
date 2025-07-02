using System.ComponentModel.DataAnnotations;

namespace Ferremas.Api.DTOs
{
    /// <summary>
    /// Modelo para procesar un pago
    /// </summary>
    public class ProcesarPagoRequest
    {
        /// <summary>
        /// Monto del pago
        /// </summary>
        [Required(ErrorMessage = "El monto es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal Monto { get; set; }

        /// <summary>
        /// Método de pago a utilizar
        /// </summary>
        [Required(ErrorMessage = "El método de pago es requerido")]
        public string MetodoPago { get; set; } = "MERCADOPAGO";

        /// <summary>
        /// Descripción del pago
        /// </summary>
        public string? Descripcion { get; set; }

        /// <summary>
        /// Datos adicionales del pago
        /// </summary>
        public Dictionary<string, string>? DatosAdicionales { get; set; }
    }
} 