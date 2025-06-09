using ExchangeRate.Models;
using FluentValidation;

namespace ExchangeRate.Validators;

public class CurrencyRateToValidator : AbstractValidator<CurrencyRateTo>
{
    private CurrencyRateToValidator()
    {
        RuleFor(x => x.BaseCode).NotEmpty();
        RuleFor(x => x.Rates).NotEmpty();
        RuleFor(x => x.TimeLastUpdateUtcInApi).Must(x => x < DateTime.UtcNow).NotEmpty();
        RuleFor(x => x.TimeLastUpdateUtc).Must(x => x == null || x < DateTime.UtcNow);
    }

    public static bool IsValid(CurrencyRateTo value) =>
        new CurrencyRateToValidator().Validate(value).IsValid;
}