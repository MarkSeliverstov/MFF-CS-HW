class Program {
    static RequestProcessor requestProcessor = new RequestProcessor();

    static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        requestProcessor.RegisterAllRoutes();

        app.Run(ProcessRequest);
        app.UseHttpsRedirection();

        app.Run();
    }

    static Task ProcessRequest(HttpContext context) {
        var path = context.Request.Path.Value ?? "";
        var query = context.Request.QueryString.Value ?? "";
        var result = requestProcessor.HandleRequest(path, query);
        return context.Response.WriteAsync(result);
    }
}

class RequestProcessor {
    private RouteMap _routes = new RouteMap();

    public void RegisterAllRoutes()
    {
    }

    public string HandleRequest(string path, string query) {
        Console.WriteLine($"+++ Thread #{Thread.CurrentThread.ManagedThreadId} processing request:");
        Console.WriteLine($"    Path =\"{path}\"");
        Console.WriteLine($"    Query=\"{query}\" ...");
 
        Console.WriteLine($"!!! Parameter ABCDE is missing !!!");
        Console.WriteLine($"!!! Route not found !!!");
        
        return "{}";
    }
}

public class RouteMap {
}

public interface ISimplisticRoutesHandler {
    public void RegisterRoutes(RouteMap routeMap);
} 
