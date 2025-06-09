namespace ExchangeRate.Models;

public class CurrencyRateToDto
{
    public string BaseCode { get; set; }
    public List<Rate> Rates { get; set; }
    public DateTime TimeLastUpdateUtcInApi { get; set; }
    public DateTime? TimeLastUpdateUtc { get; set; }
}