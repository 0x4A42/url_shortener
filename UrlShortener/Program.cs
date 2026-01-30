using MongoDB.Bson;
using MongoDB.Driver;
using UrlShortener;
using UrlShortener.Data;

var builder = WebApplication.CreateBuilder(args);

// Register MongoDB collection
builder.Services.AddScoped<IMongoCollection<BsonDocument>>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration["MongoConnectionString"];
    var client = new MongoClient(connectionString);
    var database = client.GetDatabase("local");
    return database.GetCollection<BsonDocument>("urls");
});

builder.Services.AddScoped<UrlCollection>();
var app = builder.Build();
app.MapEndpoints();
app.Run();
