using MongoDB.Bson;
using MongoDB.Driver;
using UrlShortener.Model;

namespace UrlShortener.Data;

public class UrlRepository
{
    private IMongoCollection<BsonDocument> _collection;

    public UrlRepository(IConfiguration configuration)
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
            { "accessCount", 1 },
            { "lastAccessed", "" }
        });
    }

    public bool HasUrlBeenPreviouslyShortened(string url)
    {
        return _collection.Find(x => x["originalUrl"] == url).Any();
    }

    public string GetPreviouslyShortenedUrl(string url)
    {
        return _collection.Find(x => x["originalUrl"] == url).First()["shortenedUrl"].AsString;
    }
}