using ExchangeRate.Models;
using OfficeOpenXml;

namespace ExchangeRate.Services;

public class ExcelExporterService : IExcelExporterService
{
    public ExcelExporterService(ILogger<ExcelExporterService> logger)
    {
        ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");
        _logger = logger;
    }

    private readonly ILogger<ExcelExporterService> _logger;

    public async Task<MemoryStream> GetExcelFile(CurrencyRateTo value)
    {
        var stream = new MemoryStream();

        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Sheet1");

            _logger.LogCritical(value.Rates.Count.ToString());

            worksheet.Cells[1, 1].Value = "Code";
            worksheet.Cells[1, 2].Value = $"Currency Rate to {value.BaseCode}";

            for (int i = 0; i < value.Rates.Count; i++)
            {
                worksheet.Cells[i + 2, 1].Value = value.Rates[i].Code;
                worksheet.Cells[i + 2, 2].Value = value.Rates[i].Value;
            }

            await package.SaveAsAsync(stream);
        }

        stream.Position = 0;
        return stream;
    }
}