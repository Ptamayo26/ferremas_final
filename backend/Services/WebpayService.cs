using Transbank.Webpay.WebpayPlus;
using Transbank.Webpay.WebpayPlus.Responses;
using Transbank.Common;
using Transbank.Webpay.Common;
using Transbank;

namespace Ferremas.Api.Services
{
    public class TestIntegrationType : IIntegrationType
    {
        public string Key => "integration_test";
        public string ApiBase => "https://webpay3gint.transbank.cl";
    }

    public class WebpayService
    {
        private readonly Options _options = new Options(
            "597055555532", // CommerceCode integración
            "579B532A7440BB0C9079DED94D31EA1615BACEB56610332264630D42D0A36B1C", // ApiKey integración
            new TestIntegrationType() // Tipo de integración para pruebas
        );

        public CreateResponse CrearTransaccion(decimal amount, string buyOrder, string sessionId, string returnUrl)
        {
            // Redondear el monto a enteros para evitar errores con decimales en CLP
            var amountRounded = Math.Round(amount, 0);
            Console.WriteLine($"[WebpayService] Creando transacción - Amount original: {amount}, Amount redondeado: {amountRounded}, BuyOrder: {buyOrder}, SessionId: {sessionId}, ReturnUrl: {returnUrl}");
            
            try
            {
                var transaction = new Transaction(_options);
                var response = transaction.Create(buyOrder, sessionId, amountRounded, returnUrl);
                
                Console.WriteLine($"[WebpayService] Transacción creada exitosamente - Token: {response.Token}, URL: {response.Url}");
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WebpayService] Error al crear transacción: {ex.Message}");
                throw;
            }
        }

        public CommitResponse ConfirmarTransaccion(string token)
        {
            var transaction = new Transaction(_options);
            var response = transaction.Commit(token);
            return response;
        }
    }
} 