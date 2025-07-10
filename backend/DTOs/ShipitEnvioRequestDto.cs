namespace Ferremas.Api.DTOs
{
    public class ShipitEnvioRequestDto
    {
        public string Courier { get; set; } = string.Empty;              // Ej: "Chilexpress"
        public string NombreDestinatario { get; set; } = string.Empty;   // Ej: "Juan Pérez"
        public string Direccion { get; set; } = string.Empty;            // Ej: "Av. Siempre Viva 123"
        public string ComunaDestino { get; set; } = string.Empty;        // Ej: "Santiago"
        public string Correo { get; set; } = string.Empty;               // Ej: "juan@example.com"
        public string Telefono { get; set; } = string.Empty;             // Ej: "+56912345678"
        public int ItemsCount { get; set; } = 1;                         // Número de ítems
        public int Largo { get; set; }                                   // Largo del paquete en cm
        public int Ancho { get; set; }                                   // Ancho del paquete en cm
        public int Alto { get; set; }                                    // Alto del paquete en cm
        public int Peso { get; set; }                                    // Peso en gramos
        public int ValorDeclarado { get; set; }                          // Valor del envío
        public string? Referencia { get; set; }                          // Referencia opcional, máx. 15 caracteres
        public string? Contenido { get; set; }                           // Descripción del contenido
    }
} 