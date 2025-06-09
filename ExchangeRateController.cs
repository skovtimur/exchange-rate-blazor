using System.ComponentModel.DataAnnotations;
using AutoMapper;
using ExchangeRate.Models;
using ExchangeRate.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace ExchangeRate;

[ApiController, Route("/api")]
public class ExchangeRateController(
    ICurrencyService service,
    IDistributedCache cache,
    IConfiguration conf,
    ILogger<ExchangeRateController> logger,
    IMapper mapper,
    IExcelExporterService excelExporterService) : ControllerBase
{
    [HttpGet("get/{baseCode}")]
    public async Task<IActionResult> Get([Required] string baseCode)
    {
        var rates = await service.GetExchangeRates(baseCode);

        if (rates == null)
            return BadRequest($"{baseCode} isn't supported");

        var response = mapper.Map<CurrencyRateToDto>(rates);
        return Ok(response);
    }

    [HttpGet("rates")]
    public async Task<IActionResult> GetRates()
    {
        var rates = await cache.GetStringAsync(conf["Cache:UsedCurrenciesKey"]);
        return Ok(rates ?? string.Empty);
    }


    [HttpGet("excel/{baseCode}")]
    public async Task<IActionResult> GetExcelFile([Required] string baseCode)
    {
        baseCode = baseCode.ToUpper();
        var currencyRateTo = await service.GetExchangeRates(baseCode);

        if (currencyRateTo == null)
            return BadRequest($"{baseCode} isn't supported");

        await using var file = await excelExporterService.GetExcelFile(currencyRateTo);

        if (file == null)
        {
            logger.LogCritical("The excel file could not be null");
            throw new ApplicationException("The excel file could not be null");
        }
        return File(file.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }
}