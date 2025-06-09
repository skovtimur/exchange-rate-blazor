using ExchangeRate.Models;

namespace ExchangeRate.Services;

public interface ICurrencyService
{
    public Task<CurrencyRateTo?> GetExchangeRates(string code);
    public Task<string?> SetCode(string baseCode);
}