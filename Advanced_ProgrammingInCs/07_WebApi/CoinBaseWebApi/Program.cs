using Refit;

// ToDo: Create similar app as CallingWebApi using Refit, but for https://api.coinbase.com
// For these WebAPIs:
// https://api.coinbase.com/v2/currencies
// https://api.coinbase.com/v2/exchange-rates?currency=EUR 
// (or https://api.coinbase.com/v2/exchange-rates?currency=CZK)
public record CurrencyItem(string id, string name, string min_size);
public record Currencies(CurrencyItem[] data);

public record ExchangeRateItem(string currency, Dictionary<string, string> rates);
public record ExchangeRates(ExchangeRateItem data);

public interface ICoinbaseApi
{
    [Get("/v2/currencies")]
    Task<Currencies> GetCurrenciesAsync();

    [Get("/v2/exchange-rates?currency={currency}")]
    Task<ExchangeRates> GetExchangeRatesAsync(string currency);
}

partial class Program
{
    public static string Address { get; set; } = "https://api.coinbase.com";

    public static void Main()
    {
        var coinbaseApi = RestService.For<ICoinbaseApi>(Address);

        Console.WriteLine("=== Currencies ===" + "\n");

        var currencies = coinbaseApi.GetCurrenciesAsync().Result;
        foreach (var currency in currencies.data)
            Console.WriteLine(currency);

        Console.WriteLine("\n" + "=== Exchange rates (EUR) ===" + "\n");

        var exchangeRates = coinbaseApi.GetExchangeRatesAsync("EUR").Result;
        foreach (var rate in exchangeRates.data.rates)
            Console.WriteLine($"{rate.Key}: {rate.Value}");

        Console.WriteLine("\n" + "=== Exchange rates (CZK) ===" + "\n");

        exchangeRates = coinbaseApi.GetExchangeRatesAsync("CZK").Result;
        foreach (var rate in exchangeRates.data.rates)
            Console.WriteLine($"{rate.Key}: {rate.Value}");

    }
}