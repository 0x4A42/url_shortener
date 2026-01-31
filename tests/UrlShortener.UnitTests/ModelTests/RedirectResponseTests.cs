using UrlShortener.Model.Response;

namespace UrlShortenerTests.ModelTests;

public class RedirectResponseTests
{
    [Fact]
    public void PropertiesRetainStateWhenSet()
    {
        const string errorDetail = "An error occurred.";
        
        var sut = new RedirectResponse()
        {
            ErrorDetail = errorDetail
        };

        Assert.Equal(errorDetail, sut.ErrorDetail);
    }
}