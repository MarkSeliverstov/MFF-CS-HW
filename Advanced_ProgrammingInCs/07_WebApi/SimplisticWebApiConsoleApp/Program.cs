using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

class Program
{
    static RequestProcessor requestProcessor = new RequestProcessor();

    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        requestProcessor.RegisterAllRoutes();

        app.Run(ProcessRequest);
        app.UseHttpsRedirection();

        app.Run();
    }

    static Task ProcessRequest(HttpContext context)
    {
        var path = context.Request.Path.Value ?? "";
        var query = context.Request.QueryString.Value ?? "";
        var result = requestProcessor.HandleRequest(path, query);
        return context.Response.WriteAsync(result);
    }
}

class RequestProcessor
{
    private RouteMap _routes = new RouteMap();

    public void RegisterAllRoutes()
    {
        Assembly asm = Assembly.GetExecutingAssembly();
        var test = asm.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(ISimplisticRoutesHandler)));

        var handlers = asm.GetTypes()
            .Where(t => t.GetInterfaces().Contains(typeof(ISimplisticRoutesHandler)))
            .Select(t => Activator.CreateInstance(t) as ISimplisticRoutesHandler);

        foreach (var handler in handlers)
        {
            handler.RegisterRoutes(_routes);
        }
    }

    public string HandleRequest(string path, string query)
    {
        Console.WriteLine($"+++ Thread #{Thread.CurrentThread.ManagedThreadId} processing request:");
        Console.WriteLine($"    Path =\"{path}\"");
        Console.WriteLine($"    Query=\"{query}\" ...");
        if (path == "/")
        {
            return "Home page";
        }

        if (_routes.ContainRoute(path))
        {
            var handler = _routes.GetHandler(path);
            var queryDictionary = QueryHelpers.ParseQuery(query);
            if (queryDictionary.ContainsKey("city"))
            {
                var city = queryDictionary["city"];
                if (queryDictionary.ContainsKey("street"))
                {
                    var street = queryDictionary["street"];
                    var result = handler(city, street);
                    return JsonSerializer.Serialize(result);
                }
                else
                {
                    Console.WriteLine($"!!! parametr street not found !!!");
                    return "Parametr street not found";
                }
            }
            else
            {
                Console.WriteLine($"!!! parametr city not found !!!");
                return "Parametr city not found";
            }
        }
        else
        {
            Console.WriteLine($"!!! Route not found !!!");
            return "Route not found";
        }
    }
}

public class RouteMap
{
    private Dictionary<string, Func<string, string, IReadOnlyList<PostCodeItem>>> _routes = new Dictionary<string, Func<string, string, IReadOnlyList<PostCodeItem>>>();

    public void Map(string path, Func<string, string, IReadOnlyList<PostCodeItem>> handler)
    {
        _routes.Add(path, handler);
    }

    public bool ContainRoute(string path)
    {
        return _routes.ContainsKey(path);
    }

    public Func<string, string, IReadOnlyList<PostCodeItem>> GetHandler(string path)
    {
        return _routes[path];
    }
}

public interface ISimplisticRoutesHandler
{
    public void RegisterRoutes(RouteMap routeMap);
}

