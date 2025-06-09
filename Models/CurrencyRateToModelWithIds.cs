namespace ExchangeRate.Models;

public class CurrencyRateToModelWithIds
{
    public string BaseCode { get; set; }
    public List<RateModelWithId> Rates { get; set; }
    public DateTime TimeLastUpdateUtcInApi { get; set; }
    public DateTime TimeLastUpdateUtc { get; set; }
}