using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using UrlShortener.Data;
using UrlShortener.Model;
using UrlShortener.Model.Request;
using UrlShortener.Url;

namespace UrlShortenerTests.UrlTests.EndpointTests;

public class PurgeTests
{
    [Fact]
    public void Handle_ReturnsNumberOfPurgedUrls_WhenSuccessfullyPurged()
    {
        // Arrange
        var mockMongoCollection = new Mock<IMongoCollection<BsonDocument>>();
        var mockUrlCollection = new Mock<UrlCollection>(mockMongoCollection.Object);
        var mockLogger = new Mock<ILogger<Purge>>();
        mockUrlCollection.Setup(x => x.PurgeStaleUrls(It.IsAny<int>())).Returns(5);
        var sut = new Purge(mockLogger.Object);
        
        var request = new PurgeRequest { RemoveAfterDays = 7 };

        // Act
        var result = sut.Handle(request, mockUrlCollection.Object, CancellationToken.None);

        // Assert
        var response = Assert.IsType<Ok<PurgeResponse>>(result);
        Assert.NotNull(response.Value);
        Assert.Equal("5 URL(s) successfully purged.", response.Value.Detail);
    }
    
    [Fact]
    public void Handle_ReturnsMessageStatingNone_WhenNoUrlsAreFoundToPurge()
    {
        // Arrange
        var mockMongoCollection = new Mock<IMongoCollection<BsonDocument>>();
        var mockUrlCollection = new Mock<UrlCollection>(mockMongoCollection.Object);
        var mockLogger = new Mock<ILogger<Purge>>();
        mockUrlCollection.Setup(x => x.PurgeStaleUrls(It.IsAny<int>())).Returns(0);
        var sut = new Purge(mockLogger.Object);
        
        var request = new PurgeRequest { RemoveAfterDays = 7 };

        // Act
        var result = sut.Handle(request, mockUrlCollection.Object, CancellationToken.None);

        // Assert
        var response = Assert.IsType<Ok<PurgeResponse>>(result);
        Assert.NotNull(response.Value);
        Assert.Equal("No URLs were eligible for purging.", response.Value.Detail);
    }
}