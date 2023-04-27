using System.Text.Json;

// public record PostCodeItem(string NameCity, string NameStreet, string Name, string PostCode, string Number);

partial class Program
{
    public static void JsonDeserializeOptions(string json)
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var items = JsonSerializer.Deserialize<IReadOnlyList<PostCodeItem>>(json, options);
        Console.WriteLine("Item 0:");
        Console.WriteLine(items![0]);
        Console.WriteLine();

    }
}
