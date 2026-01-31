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
        builder.Services.AddScoped<IMongoCollection<BsonDocument>>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var connectionString = configuration["MongoConnectionString"];
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("local");
            return database.GetCollection<BsonDocument>("urls");
        });

        builder.Services.AddScoped<UrlCollection>();
        builder.Services.AddScoped<Redirect>();
        builder.Services.AddScoped<Shorten>();
    }
}