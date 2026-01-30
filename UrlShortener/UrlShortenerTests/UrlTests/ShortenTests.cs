using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using UrlShortener.Data;
using UrlShortener.Model;
using UrlShortener.Url.Endpoints;

namespace UrlShortenerTests.UrlTests;

public class ShortenTests
{
    private IConfiguration GetConfiguration()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            {"MongoConnectionString", "mongodb://localhost:27017"}
        };
        return new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();
    }

    [Fact]
    public void PassingAValidUrlShouldReturnAValidShortenedUrl()
    {
        // Arrange
        var request = new ShortenRequest { Url = "http://google.com", Length = 5 };
        var collection = new UrlCollection(GetConfiguration());

        // Act
        var result = Shorten.Handle(request, collection, CancellationToken.None);

        // Assert
        var response = Assert.IsType<Ok<ShortenResponse>>(result);
        Assert.NotNull(response.Value);
        Assert.Equal("Success.", response.Value.Detail);
        Assert.Equal(5, response.Value.ShortenedUrl.Length);
    }

    [Fact]
    public void PassingAnInvalidUrlShouldReturnA400Response()
    {
        // Arrange
        var request = new ShortenRequest { Url = "not-a-valid-url", Length = 5 };
        var collection = new UrlCollection(GetConfiguration());

        // Act
        var result = Shorten.Handle(request, collection, CancellationToken.None);

        // Assert
        var response = Assert.IsType<BadRequest<ShortenResponse>>(result);
        Assert.NotNull(response.Value);
        Assert.Equal("URL provided was not properly formatted.", response.Value.Detail);
    }
    
    [Fact]
    public void PassingTheSameUrlTwiceShouldReturnTheSameShortenedUrl()
    {
        // Arrange
        var request = new ShortenRequest { Url = "http://google.com", Length = 5 };
        var collection = new UrlCollection(GetConfiguration());

        var initialResult = Shorten.Handle(request, collection, CancellationToken.None);
        var initialResponse = Assert.IsType<Ok<ShortenResponse>>(initialResult);
        Assert.NotNull(initialResponse.Value);
        
        // Act
        var secondResult = Shorten.Handle(request, collection, CancellationToken.None);
        var secondResponse = Assert.IsType<Ok<ShortenResponse>>(secondResult);
        
        // Assert
        Assert.NotNull(secondResponse.Value);
        Assert.Equal("Success.", secondResponse.Value.Detail);
        Assert.Equal(initialResponse.Value.ShortenedUrl, secondResponse.Value.ShortenedUrl);
        
    }
}