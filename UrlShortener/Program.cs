using Microsoft.AspNetCore.Authentication;
using MongoDB.Bson;
using MongoDB.Driver;
using Serilog;
using UrlShortener.Authentication;
using UrlShortener.Data;
using UrlShortener.Url;

namespace UrlShortener;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureLogging(builder);
        AddServices(builder);

        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication("ApiKey")
            .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKey", null);

        var app = builder.Build();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapEndpoints();
        app.Run();
    }

    private static void ConfigureLogging(WebApplicationBuilder builder)
    {
        var logFilePath = builder.Configuration["LogFilePath"];
        
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(logFilePath!, rollingInterval: RollingInterval.Day)
            .CreateLogger();

        builder.Host.UseSerilog();
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
        builder.Services.AddScoped<Purge>();
        builder.Services.AddScoped<Shorten>();
    }
}