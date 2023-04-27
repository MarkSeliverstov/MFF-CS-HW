using Refit;

// public record PostCodeItem(string NameCity, string NameStreet, string Name, string PostCode, string Number);

public interface ICeskaPostaWebApi
{
    [Get("/services/PostCode/getDataAsJson?cityOrPart={city}&nameStreet={street}")]
    Task<IReadOnlyList<PostCodeItem>> GetPostCodesByCityAndStreetAsync(string city, string street);
}

partial class Program
{
    public static void ViaRefit()
    {
        var postApi = RestService.For<ICeskaPostaWebApi>(Address);

        var items = postApi.GetPostCodesByCityAndStreetAsync("Praha", "Patkova").Result;
        PrintPostCodeItems(items);
        items = postApi.GetPostCodesByCityAndStreetAsync("Sokolov", "Karla Hynka Machy").Result;
        PrintPostCodeItems(items);
        items = postApi.GetPostCodesByCityAndStreetAsync("Praha", "Malostranske namesti").Result;
        PrintPostCodeItems(items);
    }

    static void PrintPostCodeItems(IEnumerable<PostCodeItem> items)
    {
        foreach (var item in items)
        {
            Console.WriteLine(item);
        }
        Console.WriteLine();
    }
}