using ExchangeRate.Services;
using Microsoft.Extensions.Caching.Distributed;
using Quartz;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ExchangeRate.BackgroundJobs;

public class UpdaterJob : IJob
{
    public UpdaterJob(IDistributedCache cashe,
        ILogger<UpdaterJob> logger, IConfiguration conf,
        ICurrencyService service)
    {
        _logger = logger;
        _cache = cashe;
        _usedCurrenciesKey = conf["Cache:UsedCurrenciesKey"];
        _service = service;

        if (string.IsNullOrEmpty(_usedCurrenciesKey))
            throw new NullReferenceException("Used currencies key not set");
    }

    private readonly ILogger<UpdaterJob> _logger;
    private readonly IDistributedCache _cache;
    private readonly string _usedCurrenciesKey;
    private readonly ICurrencyService _service;

    public static DateTime? LastUpdateAtUtc { get; private set; } = null;

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Updating currencies");
        
        var json = await _cache.GetStringAsync(_usedCurrenciesKey);

        if (string.IsNullOrEmpty(json))
            return;

        var codes = JsonSerializer.Deserialize<List<string>>(json);

        foreach (var code in codes)
            await _service.SetCode(code.ToUpper());

        LastUpdateAtUtc = DateTime.UtcNow;
    }
}