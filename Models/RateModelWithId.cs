namespace ExchangeRate.Models;

public class RateModelWithId
{
    public int Id { get; set; }
    public string Code { get; set; }
    public decimal Value { get; set; }
}