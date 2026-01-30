using Microsoft.AspNetCore.Mvc;
using UrlShortener.Data;
using UrlShortener.Model;

namespace UrlShortener.Url.Endpoints;

public class Shorten : IEndpoint
{
    
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("", Handle)
        .WithSummary("Provide a URL and length in the request body to shorten it!");
    
    internal static IResult Handle([FromBody] ShortenRequest request,
        [FromServices] UrlCollection collection,
        CancellationToken cancellationToken)
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

        var shortenedUrlResponse = ShortenUrl(request);
        collection.InsertUrl(shortenedUrlResponse);
        
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
        var allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

        return new string(Enumerable.Repeat(allowedChars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
