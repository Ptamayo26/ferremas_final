using System;
using System.Text.Json.Serialization;

namespace Ferremas.Api.Models
{
    public class MercadoPagoPreferenceResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("init_point")]
        public string InitPoint { get; set; } = string.Empty;

        [JsonPropertyName("sandbox_init_point")]
        public string SandboxInitPoint { get; set; } = string.Empty;

        [JsonPropertyName("collector_id")]
        public long CollectorId { get; set; }

        [JsonPropertyName("client_id")]
        public string ClientId { get; set; } = string.Empty;

        [JsonPropertyName("date_created")]
        public DateTime DateCreated { get; set; }

        [JsonPropertyName("expires")]
        public bool Expires { get; set; }

        [JsonPropertyName("expiration_date_from")]
        public DateTime? ExpirationDateFrom { get; set; }

        [JsonPropertyName("expiration_date_to")]
        public DateTime? ExpirationDateTo { get; set; }

        [JsonPropertyName("external_reference")]
        public string ExternalReference { get; set; } = string.Empty;

        [JsonPropertyName("operation_type")]
        public string OperationType { get; set; } = string.Empty;

        [JsonPropertyName("additional_info")]
        public string AdditionalInfo { get; set; } = string.Empty;

        [JsonPropertyName("auto_return")]
        public string AutoReturn { get; set; } = string.Empty;

        [JsonPropertyName("back_urls")]
        public BackUrls BackUrls { get; set; } = new();

        [JsonPropertyName("marketplace")]
        public string Marketplace { get; set; } = string.Empty;

        [JsonPropertyName("marketplace_fee")]
        public decimal MarketplaceFee { get; set; }

        [JsonPropertyName("notification_url")]
        public string NotificationUrl { get; set; } = string.Empty;

        [JsonPropertyName("site_id")]
        public string SiteId { get; set; } = string.Empty;

        [JsonPropertyName("total_amount")]
        public decimal? TotalAmount { get; set; }

        [JsonPropertyName("last_updated")]
        public DateTime? LastUpdated { get; set; }
    }

    public class BackUrls
    {
        [JsonPropertyName("success")]
        public string Success { get; set; } = string.Empty;

        [JsonPropertyName("pending")]
        public string Pending { get; set; } = string.Empty;

        [JsonPropertyName("failure")]
        public string Failure { get; set; } = string.Empty;
    }

    public class MercadoPagoWebhookRequest
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("live_mode")]
        public bool LiveMode { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("date_created")]
        public DateTime DateCreated { get; set; }

        [JsonPropertyName("application_id")]
        public string ApplicationId { get; set; } = string.Empty;

        [JsonPropertyName("user_id")]
        public string UserId { get; set; } = string.Empty;

        [JsonPropertyName("version")]
        public int Version { get; set; }

        [JsonPropertyName("api_version")]
        public string ApiVersion { get; set; } = string.Empty;

        [JsonPropertyName("action")]
        public string Action { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public WebhookData Data { get; set; } = new();

        // Propiedades adicionales para compatibilidad
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("external_reference")]
        public string? ExternalReference { get; set; }
    }

    public class WebhookData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
    }
} 