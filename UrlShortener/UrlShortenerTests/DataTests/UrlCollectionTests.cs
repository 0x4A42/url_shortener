using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using Moq.AutoMock;
using UrlShortener.Data;

namespace UrlShortenerTests.DataTests;

public class UrlCollectionTests
{
    [Fact]
    public void HasUrlBeenPreviouslyShortened_ReturnsTrue_WhenProvidedUrlThatHasBeenShortenedBefore()
    {
        // Arrange
        var autoMocker = new AutoMocker();

        // Mock cursor to return documents
        var mockCursor = new Mock<IAsyncCursor<BsonDocument>>();
        var documents = new List<BsonDocument> { new() { { "originalUrl", "http://google.com" } } };
        mockCursor.Setup(x => x.Current).Returns(documents);
        mockCursor.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);

        // Mock IFindFluent to return cursor
        var mockFindFluent = new Mock<IFindFluent<BsonDocument, BsonDocument>>();
        mockFindFluent
            .Setup(x => x.ToCursor(It.IsAny<CancellationToken>()))
            .Returns(mockCursor.Object);

        // Mock IMongoCollection
        var mockCollection = autoMocker.GetMock<IMongoCollection<BsonDocument>>();
        mockCollection
            .Setup(x => x.FindSync(
                It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<FindOptions<BsonDocument, BsonDocument>>(),
                It.IsAny<CancellationToken>()))
            .Returns(mockCursor.Object);

        var sut = new UrlCollection(mockCollection.Object);

        // Act
        var result = sut.HasUrlBeenPreviouslyShortened("http://google.com");

        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void HasUrlBeenPreviouslyShortened_ReturnsFalse_WhenProvidedUrlThatHasNotBeenShortenedBefore()
    {
        // Arrange
        var autoMocker = new AutoMocker();

        // Mock cursor to return documents
        var mockCursor = new Mock<IAsyncCursor<BsonDocument>>();
        var documents = new List<BsonDocument> { new BsonDocument { { "originalUrl", "http://google.com" } } };
        mockCursor.Setup(x => x.Current).Returns(documents);
        mockCursor.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(false)
            .Returns(false);

        // Mock IFindFluent to return cursor
        var mockFindFluent = new Mock<IFindFluent<BsonDocument, BsonDocument>>();
        mockFindFluent
            .Setup(x => x.ToCursor(It.IsAny<CancellationToken>()))
            .Returns(mockCursor.Object);

        // Mock IMongoCollection
        var mockCollection = autoMocker.GetMock<IMongoCollection<BsonDocument>>();
        mockCollection
            .Setup(x => x.FindSync(
                It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<FindOptions<BsonDocument, BsonDocument>>(),
                It.IsAny<CancellationToken>()))
            .Returns(mockCursor.Object);

        var sut = new UrlCollection(mockCollection.Object);

        // Act
        var result = sut.HasUrlBeenPreviouslyShortened("http://google.com");

        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void GetShortenedUrl_ReturnsShortenedUrl_WhenFound()
    {
        // Arrange
        var expectedShortenedUrl = "abcdef";
        var autoMocker = new AutoMocker();

        // Mock cursor to return documents with shortenedUrl
        var mockCursor = new Mock<IAsyncCursor<BsonDocument>>();
        var documents = new List<BsonDocument>
        {
            new BsonDocument
            {
                { "originalUrl", "http://google.com" },
                { "shortenedUrl", expectedShortenedUrl }
            }
        };
        mockCursor.Setup(x => x.Current).Returns(documents);
        mockCursor.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);

        // Mock IMongoCollection
        var mockCollection = autoMocker.GetMock<IMongoCollection<BsonDocument>>();
        mockCollection
            .Setup(x => x.FindSync(
                It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<FindOptions<BsonDocument, BsonDocument>>(),
                It.IsAny<CancellationToken>()))
            .Returns(mockCursor.Object);

        var sut = new UrlCollection(mockCollection.Object);

        // Act
        var result = sut.GetShortenedUrl("http://google.com");

        // Assert
        Assert.Equal(expectedShortenedUrl, result);
    }
    
    [Fact]
    public void GetOriginalUrl_ReturnsOriginalUrl_WhenFound()
    {
        // Arrange
        var shortenedUrl = "testurlvalue";
        var expectedOriginalUrl = "http://google.com";
        var autoMocker = new AutoMocker();

        // Mock cursor to return no documents
        var mockCursor = new Mock<IAsyncCursor<BsonDocument>>();
        var documents = new List<BsonDocument>
        {
            new BsonDocument
            {
                { "originalUrl", expectedOriginalUrl },
                { "shortenedUrl", shortenedUrl }
            }
        };
        mockCursor.Setup(x => x.Current).Returns(documents);
        mockCursor.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);

        // Mock IMongoCollection
        var mockCollection = autoMocker.GetMock<IMongoCollection<BsonDocument>>();
        mockCollection
            .Setup(x => x.FindSync(
                It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<FindOptions<BsonDocument, BsonDocument>>(),
                It.IsAny<CancellationToken>()))
            .Returns(mockCursor.Object);

        var sut = new UrlCollection(mockCollection.Object);

        // Act
        var result = sut.GetOriginalUrl(shortenedUrl);

        // Assert
        Assert.Equal(expectedOriginalUrl, result);
    }
    
    [Fact]
    public void GetOriginalUrl_ReturnsEmptyString_WhenNotFound()
    {
        // Arrange
        var autoMocker = new AutoMocker();

        // Mock cursor to return no documents
        var mockCursor = new Mock<IAsyncCursor<BsonDocument>>();
        var documents = new List<BsonDocument>();
        mockCursor.Setup(x => x.Current).Returns(documents);
        mockCursor.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(false);

        // Mock IMongoCollection
        var mockCollection = autoMocker.GetMock<IMongoCollection<BsonDocument>>();
        mockCollection
            .Setup(x => x.FindSync(
                It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<FindOptions<BsonDocument, BsonDocument>>(),
                It.IsAny<CancellationToken>()))
            .Returns(mockCursor.Object);

        var sut = new UrlCollection(mockCollection.Object);

        // Act
        var result = sut.GetOriginalUrl("testurlvalue");

        // Assert
        Assert.Equal(string.Empty, result);
    }
}