using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using UrlShortener.Data;
using UrlShortener.Model.Request;
using UrlShortener.Model.Response;
using UrlShortener.Url;

namespace UrlShortenerTests.UrlTests.EndpointTests;

public class ShortenTests
{
    [Fact]
    public void Handle_ReturnsSuccess_ForValidUrlThatWasNotShortenedBefore()
    {
        // Arrange
        var mockMongoCollection = new Mock<IMongoCollection<BsonDocument>>();
        var mockUrlCollection = new Mock<UrlCollection>(mockMongoCollection.Object);
        var mockLogger = new Mock<ILogger<Shorten>>();
        mockUrlCollection.Setup(x => x.HasUrlBeenPreviouslyShortened(It.IsAny<string>())).Returns(false);
        var sut = new Shorten(mockLogger.Object);
        
        var request = new ShortenRequest { Url = "http://google.com", Length = 5 };

        // Act
        var result = sut.Handle(request, mockUrlCollection.Object, CancellationToken.None);

        // Assert
        var response = Assert.IsType<Ok<ShortenResponse>>(result);
        Assert.NotNull(response.Value);
        Assert.Equal("Success.", response.Value.Detail);
        Assert.Equal(5, response.Value.ShortenedUrl.Length);
    }

    [Fact]
    public void Handle_Returns400Response_ForInvalidUrlFormat()
    {
        // Arrange
        var mockMongoCollection = new Mock<IMongoCollection<BsonDocument>>();
        var mockUrlCollection = new Mock<UrlCollection>(mockMongoCollection.Object);
        var mockLogger = new Mock<ILogger<Shorten>>();
        var sut = new Shorten(mockLogger.Object);
        var request = new ShortenRequest { Url = "not-a-valid-url", Length = 5 };
        
        // Act
        var result = sut.Handle(request, mockUrlCollection.Object, CancellationToken.None);

        // Assert
        var response = Assert.IsType<BadRequest<ShortenResponse>>(result);
        Assert.NotNull(response.Value);
        Assert.Equal("URL provided was not properly formatted.", response.Value.Detail);
    }
    
    [Fact]
    public void PassingTheSameUrlTwiceShouldReturnTheSameShortenedUrl()
    {
        // Arrange
        var mockMongoCollection = new Mock<IMongoCollection<BsonDocument>>();
        var mockUrlCollection = new Mock<UrlCollection>(mockMongoCollection.Object);
        var mockLogger = new Mock<ILogger<Shorten>>();
        var sut = new Shorten(mockLogger.Object);
        
        var request = new ShortenRequest { Url = "http://google.com", Length = 5 };
        var initialResult = sut.Handle(request, mockUrlCollection.Object, CancellationToken.None);
        var initialResponse = Assert.IsType<Ok<ShortenResponse>>(initialResult);
        Assert.NotNull(initialResponse.Value);
        
        mockUrlCollection.Setup(x => x.HasUrlBeenPreviouslyShortened(It.IsAny<string>())).Returns(true);
        mockUrlCollection.Setup(x => x.GetShortenedUrl(It.IsAny<string>())).Returns(initialResponse.Value.ShortenedUrl);
        
        // Act
        var secondResult = sut.Handle(request, mockUrlCollection.Object, CancellationToken.None);
        var secondResponse = Assert.IsType<Ok<ShortenResponse>>(secondResult);
        
        // Assert
        Assert.NotNull(secondResponse.Value);
        Assert.Equal("Success.", secondResponse.Value.Detail);
        Assert.Equal(initialResponse.Value.ShortenedUrl, secondResponse.Value.ShortenedUrl);
        
    }
}