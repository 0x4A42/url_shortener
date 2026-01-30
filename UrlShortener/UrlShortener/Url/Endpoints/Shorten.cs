using Microsoft.AspNetCore.Mvc;
using UrlShortener.Data;
using UrlShortener.Interface;
using UrlShortener.Model;

namespace UrlShortener.Url.Endpoints;

public class Shorten : IEndpoint
{
    
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("shorten", Handle)
        .WithSummary("Provide a URL to shorten it!");
    
    internal static IResult Handle([FromBody] ShortenRequest request, [FromServices] UrlRepository repository, CancellationToken cancellationToken)
    {
        if (!CheckUrlIsValid(request.Url))
            return Results.BadRequest(new ShortenResponse
            {
                OriginalUrl = request.Url,
                ShortenedUrl = "",
                Detail = "URL provided was not properly formatted."
            });

        if (repository.HasUrlBeenPreviouslyShortened(request.Url))
            return Results.Ok(new ShortenResponse()
            {
                OriginalUrl = request.Url,
                ShortenedUrl = repository.GetPreviouslyShortenedUrl(request.Url),
                Detail = "Success."
            });

        var shortenedUrlResponse = ShortenUrl(request);
        repository.InsertUrl(shortenedUrlResponse);
        return Results.Ok(shortenedUrlResponse);
    }
    
    private static bool CheckUrlIsValid(string url)
    {
        return Uri.IsWellFormedUriString(url, UriKind.Absolute);
    }
    
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
