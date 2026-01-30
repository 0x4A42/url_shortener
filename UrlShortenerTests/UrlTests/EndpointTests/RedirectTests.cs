using Microsoft.AspNetCore.Http.HttpResults;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using UrlShortener.Data;
using UrlShortener.Model;
using UrlShortener.Url.Endpoints;

namespace UrlShortenerTests.UrlTests.EndpointTests;

public class RedirectTests
{
    [Fact]
    public void NavigatingToAValidShortenedUrlShouldRedirect()
    {
        // Arrange
        const string expectedUrl = "http://google.com";
        var mockMongoCollection = new Mock<IMongoCollection<BsonDocument>>();
        var mockUrlCollection = new Mock<UrlCollection>(mockMongoCollection.Object);
        mockUrlCollection.Setup(x => x.GetOriginalUrl(It.IsAny<string>())).Returns(expectedUrl);
        mockUrlCollection.Setup(x => x.IncrementTelemetryForUrl(It.IsAny<string>()));
        
        var sut = new Redirect();

        // Act
        var result = sut.Handle("testShortenedUrl", mockUrlCollection.Object, CancellationToken.None);

        // Assert
        var response = Assert.IsType<RedirectHttpResult>(result);
        Assert.Equal(expectedUrl, response.Url);
    }

    [Fact]
    public void NavigatingToAnInvalidShortenedUrlShouldReturnA400Response()
    {
        // Arrange
        var mockMongoCollection = new Mock<IMongoCollection<BsonDocument>>();
        var mockUrlCollection = new Mock<UrlCollection>(mockMongoCollection.Object);
        mockUrlCollection.Setup(x => x.GetOriginalUrl(It.IsAny<string>())).Returns(string.Empty);
        
        var sut = new Redirect();
        var baseUrl = "http://localhost:5000/";
        var request = $"{baseUrl}/url/not-a-valid-url";

        // Act
        var result = sut.Handle(request, mockUrlCollection.Object, CancellationToken.None);

        // Assert
        var response = Assert.IsType<BadRequest<RedirectResponse>>(result);
        Assert.NotNull(response.Value);
        Assert.Equal("Invalid shortened URL - please call the `/shorten` endpoint to shorten a URL.",
            response.Value.ErrorDetail);
    }
}