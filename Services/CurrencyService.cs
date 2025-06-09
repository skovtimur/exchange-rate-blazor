using System.Net;
using System.Text.Json;
using ExchangeRate.BackgroundJobs;
using ExchangeRate.Models;
using ExchangeRate.Models.Responses;
using ExchangeRate.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ExchangeRate.Services;

public class CurrencyService : ICurrencyService
{
    public CurrencyService(IDistributedCache cache, HttpClient client,
        IOptions<ApiOptions> options, IConfiguration conf,
        ILogger<CurrencyService> logger)
    {
        _client = client;
        _options = options.Value;
        _cache = cache;
        _usedCurrenciesKey = conf["Cache:UsedCurrenciesKey"];
        _logger = logger;
    }

    private readonly ApiOptions _options;
    private readonly HttpClient _client;
    private readonly IDistributedCache _cache;
    private readonly string _usedCurrenciesKey;
    private readonly ILogger<CurrencyService> _logger;

    public async Task<CurrencyRateTo?> GetExchangeRates(string code)
    {
        code = code.ToUpper();
        var json = await _cache.GetStringAsync(code);

        if (string.IsNullOrEmpty(json))
        {
            var response = await SetCode(code);

            if (string.IsNullOrEmpty(response))
            {
                return null;
            }

            json = response;
            await AddCodeInList(code);
        }

        var jsonData = JsonSerializer.Deserialize<ExchangeRateResponse>(json);

        return CurrencyRateTo.Create(jsonData.BaseCode, jsonData.ConversionRates,
            jsonData.TimeLastUpdateUtcInApi, UpdaterJob.LastUpdateAtUtc);
    }

    private async Task AddCodeInList(string code)
    {
        var listJson = await _cache.GetStringAsync(_usedCurrenciesKey);
        List<string>? list = string.IsNullOrEmpty(listJson)
            ? null
            : JsonSerializer.Deserialize<List<string>>(listJson);

        if (list == null)
        {
            list = new List<string> { code };
        }
        else if (list.Contains(code) == false)
        {
            list.Add(code);
        }

        var newListJson = JsonSerializer.Serialize(list);
        await _cache.SetStringAsync(_usedCurrenciesKey, newListJson);
    }

    public async Task<string?> SetCode(string baseCode)
    {
        baseCode = baseCode.ToUpper();

        var url = _options.Url
            .Replace("{BASE_CODE}", baseCode)
            .Replace("{API_KEY}", _options.Key);

        try
        {
            var responseJson = await _client.GetStringAsync(url);

            if (string.IsNullOrEmpty(responseJson))
            {
                const string criticalText = "API returned empty response";

                _logger.LogCritical(criticalText);
                throw new ApplicationException(criticalText);
            }

            using var doc = JsonDocument.Parse(responseJson);
            var root = doc.RootElement;

            var result = root.GetProperty("result").GetString();

            if (result != "success")
            {
                _logger.LogDebug("This {X} is unsupported", baseCode);
                return null;
            }

            await _cache.SetStringAsync(baseCode, responseJson);
            return responseJson;
        }
        catch (HttpRequestException exception)
        {
            if (exception.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogDebug("This {X} is unsupported", baseCode);
                return null;
            }
            _logger.LogCritical(exception, $"Unknown exception. Message: {exception.Message}");
            throw exception;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, ex.Message);
            throw ex;
        }
    }
}