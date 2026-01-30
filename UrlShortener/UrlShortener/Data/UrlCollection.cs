using MongoDB.Bson;
using MongoDB.Driver;
using UrlShortener.Model;

namespace UrlShortener.Data;

public class UrlCollection
{
    private readonly IMongoCollection<BsonDocument> _collection;

    public UrlCollection(IConfiguration configuration)
    {
        var connectionString = configuration["MongoConnectionString"];
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase("local");
        _collection = database.GetCollection<BsonDocument>("urls");
    }
    
    public void InsertUrl(ShortenResponse shortenedUrlResponse)
    {
        _collection.InsertOne(new BsonDocument()
        {
            { "originalUrl", shortenedUrlResponse.OriginalUrl },
            { "shortenedUrl", shortenedUrlResponse.ShortenedUrl },
            { "numberOfRedirects", 1 },
            { "lastAccessedUTC", "" }
        });
    }

    public bool HasUrlBeenPreviouslyShortened(string url) 
        => _collection.Find(x => x["originalUrl"] == url).Any();
    
    public string GetShortenedUrl(string url) 
        => _collection.Find(x => x["originalUrl"] == url).First()["shortenedUrl"].AsString;
    
    public string GetOriginalUrl(string shortenedUrl)
    {
        var result = _collection.Find(x => x["shortenedUrl"] == shortenedUrl).FirstOrDefault();
        return result?["originalUrl"].AsString ?? string.Empty;
    }
    
    public void IncrementTelemetryForUrl(string shortenedUrl)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("shortenedUrl", shortenedUrl);
        var update = Builders<BsonDocument>.Update
            .Inc("numberOfRedirects", 1)
            .Set("lastAccessedUTC", DateTime.UtcNow);
        
        _collection.UpdateOne(filter, update);
    }
}