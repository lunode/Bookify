namespace Bookify.Domain.Apartments;

public record Currency
{
    internal static readonly Currency None = new("");
    public static readonly Currency USD = new("USD");
    public static readonly Currency EUR = new("EUR");
    private Currency(string code) => Code = code;
    public string Code { get; init; }
    public static Currency FromCode(string code)
    {
        return All.FirstOrDefault(c => c.Code == code) ?? throw new ApplicationException($"Invalid currency code: {code}");
    }
    public static readonly IReadOnlyCollection<Currency> All = [USD, EUR];
}
