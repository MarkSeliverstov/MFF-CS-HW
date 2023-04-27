
if (args.Length != 0)
    Address = args[0];

var json = ViaHttpClient();
JsonDeserialize(json);
JsonDeserializeOptions(json);

ViaRefit();

partial class Program
{
    public static string Address { get; set; } = "https://b2c.cpost.cz";
}
