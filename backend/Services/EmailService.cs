using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Ferremas.Api.Services
{
    public interface IEmailService
    {
        Task<bool> EnviarConfirmacionPedido(string email, string numeroPedido, decimal total, string trackingNumber = null);
    }

    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public async Task<bool> EnviarConfirmacionPedido(string email, string numeroPedido, decimal total, string? trackingNumber = null)
        {
            try
            {
                _logger.LogInformation($"Enviando confirmación de pedido a {email} - Pedido: {numeroPedido}");
                
                // Por ahora solo log, pero aquí se integraría con SendGrid, SMTP, etc.
                var mensaje = $@"
                    ¡Gracias por tu compra en Ferremas!
                    
                    Número de Pedido: {numeroPedido}
                    Total: ${total:N0}
                    {(trackingNumber != null ? $"Número de Seguimiento: {trackingNumber}" : "")}
                    
                    Te notificaremos cuando tu pedido esté listo para envío.
                    
                    Saludos,
                    Equipo Ferremas
                ";

                _logger.LogInformation($"Email de confirmación generado para {email}: {mensaje}");
                
                // TODO: Integrar con servicio de email real
                // await _emailClient.SendAsync(email, "Confirmación de Pedido", mensaje);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error enviando email de confirmación: {ex.Message}");
                return false;
            }
        }
    }
} 