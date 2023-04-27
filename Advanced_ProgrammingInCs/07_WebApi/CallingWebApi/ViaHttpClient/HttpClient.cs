partial class Program
{
    public static string ViaHttpClient()
    {
        Console.WriteLine("+++ via HttpClient & JsonSerializer:");

        var httpClient = new HttpClient();
        var json = httpClient.GetStringAsync(Address + "/services/PostCode/getDataAsJson?cityOrPart=Sokolov&nameStreet=Karla+Hynka+Machy").Result;
        Console.WriteLine(json);
        Console.WriteLine();
        return json;
    }
}
