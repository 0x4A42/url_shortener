using UrlShortener.Model;

namespace UrlShortenerTests.ModelTests;

public class ShortenResponseTests
{
    [Fact]
    public void PropertiesRetainStateWhenSet()
    {
        const string originalUrl = "https://google.com";
        const string shortenedUrl = "abc123";
        const string detail = "Success.";
        
        var sut = new ShortenResponse()
        {
            OriginalUrl = originalUrl,
            ShortenedUrl = shortenedUrl,
            Detail = detail
        };

        Assert.Equal(originalUrl, sut.OriginalUrl);
        Assert.Equal(shortenedUrl, sut.ShortenedUrl);
        Assert.Equal(detail, sut.Detail);
    }
}