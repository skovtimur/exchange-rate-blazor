using System.Text.Json.Serialization;

namespace ExchangeRate.Models.Responses;

public class ExchangeRateResponse
{
    [JsonPropertyName("result")]
    public string Result { get; set; }

    [JsonPropertyName("time_last_update_utc")]
    public string TimeLastUpdateUtcInApi { get; set; }

    [JsonPropertyName("base_code")]
    public string BaseCode { get; set; }

    [JsonPropertyName("conversion_rates")]
    public Dictionary<string, decimal> ConversionRates { get; set; }
}