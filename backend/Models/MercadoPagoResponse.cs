using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ferremas.Api.Models
{
    public class MercadoPagoResponse
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("status_detail")]
        public string StatusDetail { get; set; } = string.Empty;

        [JsonPropertyName("operation_type")]
        public string OperationType { get; set; } = string.Empty;

        [JsonPropertyName("date_created")]
        public DateTime DateCreated { get; set; }

        [JsonPropertyName("date_approved")]
        public DateTime? DateApproved { get; set; }

        [JsonPropertyName("date_last_updated")]
        public DateTime DateLastUpdated { get; set; }

        [JsonPropertyName("money_release_date")]
        public DateTime? MoneyReleaseDate { get; set; }

        [JsonPropertyName("currency_id")]
        public string CurrencyId { get; set; } = string.Empty;

        [JsonPropertyName("transaction_amount")]
        public decimal TransactionAmount { get; set; }

        [JsonPropertyName("transaction_amount_refunded")]
        public decimal TransactionAmountRefunded { get; set; }

        [JsonPropertyName("coupon_amount")]
        public decimal CouponAmount { get; set; }

        [JsonPropertyName("differential_pricing_id")]
        public long? DifferentialPricingId { get; set; }

        [JsonPropertyName("deduction_schema")]
        public string? DeductionSchema { get; set; }

        [JsonPropertyName("external_reference")]
        public string? ExternalReference { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("installments")]
        public int Installments { get; set; }

        [JsonPropertyName("payment_method_id")]
        public string PaymentMethodId { get; set; } = string.Empty;

        [JsonPropertyName("payment_type_id")]
        public string PaymentTypeId { get; set; } = string.Empty;

        [JsonPropertyName("live_mode")]
        public bool LiveMode { get; set; }

        [JsonPropertyName("sponsor_id")]
        public long? SponsorId { get; set; }

        [JsonPropertyName("processing_mode")]
        public string ProcessingMode { get; set; } = string.Empty;

        [JsonPropertyName("merchant_account_id")]
        public string? MerchantAccountId { get; set; }

        [JsonPropertyName("acquirer_reconciliation")]
        public List<object> AcquirerReconciliation { get; set; } = new();

        [JsonPropertyName("point_of_interaction")]
        public object? PointOfInteraction { get; set; }
    }

    public class Payer
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
} 