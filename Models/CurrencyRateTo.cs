using ExchangeRate.Validators;

namespace ExchangeRate.Models;

public class CurrencyRateTo
{
    public string BaseCode { get; set; }
    public List<Rate> Rates { get; set; }
    public DateTime TimeLastUpdateUtcInApi { get; set; }
    public DateTime? TimeLastUpdateUtc { get; set; }

    public static CurrencyRateTo? Create(string baseCode, Dictionary<string, decimal> rates,
        string timeLastUpdateUtcInApi, DateTime? timeLastUpdateUtc)
    {
        if (string.IsNullOrEmpty(baseCode))
            return null;

        if (DateTime.TryParse(timeLastUpdateUtcInApi,
                out var timeLastUpdateUtcInApiDateTime) == false)
        {
            throw new ApplicationException($"Invalid time last update in API: {timeLastUpdateUtcInApi}");
        }

        var result = new CurrencyRateTo
        {
            TimeLastUpdateUtc = timeLastUpdateUtc,
            TimeLastUpdateUtcInApi = timeLastUpdateUtcInApiDateTime,
            BaseCode = baseCode,
        };
        var list = rates
            .Select(r => new Rate(code: r.Key, value: r.Value))
            .ToList();

        if (list.Count <= 0)
        {
            throw new ApplicationException("Rates can't be empty");
        }

        result.Rates = list;

        return CurrencyRateToValidator.IsValid(result) ? result : null;
    }
}