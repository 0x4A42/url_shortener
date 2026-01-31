using MongoDB.Bson;
using MongoDB.Driver;
using UrlShortener.Data;
using UrlShortener.Url.Endpoints;

namespace UrlShortener;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        AddServices(builder);
        
        var app = builder.Build();
        app.MapEndpoints();
        app.Run();
    }

    private static void AddServices(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IMongoClient>(sp => {
            var connectionString = sp.GetRequiredService<IConfiguration>()["MongoConnectionString"];
            return new MongoClient(connectionString);
        });

        builder.Services.AddScoped<IMongoCollection<BsonDocument>>(sp => {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase("local").GetCollection<BsonDocument>("urls");
        });
        
        builder.Services.AddScoped<UrlCollection>();
        builder.Services.AddScoped<Redirect>();
        builder.Services.AddScoped<Shorten>();
    }
}