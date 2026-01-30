using MongoDB.Bson;
using MongoDB.Driver;
using UrlShortener.Model;

namespace UrlShortener.Data;

public class UrlCollection(IMongoCollection<BsonDocument> collection)
{
    public void InsertUrl(ShortenResponse shortenedUrlResponse)
    {
        collection.InsertOne(new BsonDocument()
        {
            { "originalUrl", shortenedUrlResponse.OriginalUrl },
            { "shortenedUrl", shortenedUrlResponse.ShortenedUrl },
            { "numberOfRedirects", 1 },
            { "lastAccessedUTC", "" }
        });
    }

    public virtual bool HasUrlBeenPreviouslyShortened(string url) 
        => collection.Find(x => x["originalUrl"] == url).Any();
    
    public virtual string GetShortenedUrl(string url) 
        => collection.Find(x => x["originalUrl"] == url).First()["shortenedUrl"].AsString;
    
    public virtual string GetOriginalUrl(string shortenedUrl)
    {
        var result = collection.Find(x => x["shortenedUrl"] == shortenedUrl).FirstOrDefault();
        return result?["originalUrl"].AsString ?? string.Empty;
    }
    
    public virtual void IncrementTelemetryForUrl(string shortenedUrl)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("shortenedUrl", shortenedUrl);
        var update = Builders<BsonDocument>.Update
            .Inc("numberOfRedirects", 1)
            .Set("lastAccessedUTC", DateTime.UtcNow);
        
        collection.UpdateOne(filter, update);
    }
}