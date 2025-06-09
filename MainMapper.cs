using ExchangeRate.Models;

namespace ExchangeRate;

public class MainMapper : AutoMapper.Profile
{
    public MainMapper()
    {
        CreateMap<CurrencyRateTo, CurrencyRateToDto>();
        CreateMap<CurrencyRateTo, CurrencyRateToModelWithIds>()
            .ForMember(x => x.Rates,
                opt => opt.MapFrom(src => GetRatesWithIds(src.Rates)));
    }

    private List<RateModelWithId> GetRatesWithIds(List<Rate> rates)
    {
        return rates
            .Select((t, i) =>
                new RateModelWithId
                {
                    Id = i,
                    Code = t.Code,
                    Value = t.Value,
                })
            .ToList();
    }
}