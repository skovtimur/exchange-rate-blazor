namespace ExchangeRate.Models;

public struct Rate(string code, decimal value)
{
    public string Code { get; set; } = code;
    public decimal Value { get; set; } = value;
}