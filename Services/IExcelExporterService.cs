using ExchangeRate.Models;

namespace ExchangeRate.Services;

public interface IExcelExporterService
{
    public Task<MemoryStream> GetExcelFile(CurrencyRateTo value);
}