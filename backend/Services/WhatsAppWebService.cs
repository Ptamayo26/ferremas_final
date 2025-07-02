using System;

namespace Ferremas.Api.Services
{
    public class WhatsAppWebService
    {
        public string GenerarLinkWhatsapp(string telefono, string mensaje)
        {
            if (string.IsNullOrWhiteSpace(telefono) || string.IsNullOrWhiteSpace(mensaje))
                throw new ArgumentException("El teléfono y el mensaje son requeridos");

            var telefonoLimpio = telefono.Replace("+", "").Replace(" ", "").Trim();
            var mensajeEncoded = Uri.EscapeDataString(mensaje);

            return $"https://wa.me/{telefonoLimpio}?text={mensajeEncoded}";
        }
    }
} 