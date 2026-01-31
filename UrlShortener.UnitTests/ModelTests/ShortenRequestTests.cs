using UrlShortener.Model.Request;

namespace UrlShortenerTests.ModelTests;

public class ShortenRequestTests
{
    [Fact]
    public void LengthDefaultsTo7()
    {
        var sut = new ShortenRequest()
        {
            Url = "test"
        };

        Assert.Equal(7, sut.Length);
    }
    
    [Fact]
    public void PropertiesRetainStateWhenSet()
    {
        const string url = "https://google.com";
        const int length = 4;
        
        var sut = new ShortenRequest()
        {
            Url = url,
            Length = length
        };

        Assert.Equal(url, sut.Url);
        Assert.Equal(length, sut.Length);
    }
}