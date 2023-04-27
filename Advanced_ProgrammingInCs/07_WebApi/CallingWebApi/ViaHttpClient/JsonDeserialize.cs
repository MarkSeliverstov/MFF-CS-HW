using System.Text.Json;

public record PostCodeItem(string NameCity, string NameStreet, string Name, string PostCode, string Number);

partial class Program
{
    public static void JsonDeserialize(string json)
    {
        var items = JsonSerializer.Deserialize<IReadOnlyList<PostCodeItem>>(json);
        Console.WriteLine("Item 0:");
        Console.WriteLine(items![0]);
        Console.WriteLine();
    }
}
