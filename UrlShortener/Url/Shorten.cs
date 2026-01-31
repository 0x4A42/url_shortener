using Microsoft.AspNetCore.Mvc;
using UrlShortener.Data;
using UrlShortener.Model.Request;
using UrlShortener.Model.Response;

namespace UrlShortener.Url;

public class Shorten(ILogger<Shorten> logger) : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("",
            ([FromBody] ShortenRequest request,
            [FromServices] UrlCollection collection,
            [FromServices] Shorten shorten,
            CancellationToken cancellationToken) => shorten.Handle(request, collection, cancellationToken))
        .WithSummary("Provide a URL and length in the request body to shorten it!");

    internal IResult Handle([FromBody] ShortenRequest request,
        [FromServices] UrlCollection collection,
        CancellationToken cancellationToken)
    {
        ShortenResponse? shortenedUrlResponse = null;

        try
        {
            if (!CheckUrlIsValid(request.Url))
                return Results.BadRequest(new ShortenResponse
                {
                    OriginalUrl = request.Url,
                    ShortenedUrl = "",
                    Detail = "URL provided was not properly formatted."
                });

            if (collection.HasUrlBeenPreviouslyShortened(request.Url))
                return Results.Ok(new ShortenResponse
                {
                    OriginalUrl = request.Url,
                    ShortenedUrl = collection.GetShortenedUrl(request.Url),
                    Detail = "Success."
                });

            shortenedUrlResponse = ShortenUrl(request);
            collection.InsertUrl(shortenedUrlResponse);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error experienced during execution of Shorten endpoint");
        }
        
        return Results.Ok(shortenedUrlResponse);
    }
    
    private static bool CheckUrlIsValid(string url) 
        => Uri.IsWellFormedUriString(url, UriKind.Absolute);
    
    private static ShortenResponse ShortenUrl(ShortenRequest request)
    {
        return new ShortenResponse
        {
            OriginalUrl = request.Url,
            ShortenedUrl = GenerateShortenedUrlToken(request.Length),
            Detail = "Success."
        };
    }

    private static string GenerateShortenedUrlToken(int length)
    {
        var random = new Random();
        const string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

        return new string(Enumerable.Repeat(allowedChars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
