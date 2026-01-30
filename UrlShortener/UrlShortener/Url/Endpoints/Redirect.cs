using Microsoft.AspNetCore.Mvc;
using UrlShortener.Data;
using UrlShortener.Model;

namespace UrlShortener.Url.Endpoints;

public class Redirect: IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("{shortenedUrl}",
            ([FromRoute] string shortenedUrl,
            [FromServices] UrlCollection collection,
            [FromServices] Redirect redirect,
            CancellationToken cancellationToken) => redirect.Handle(shortenedUrl, collection, cancellationToken))
        .WithSummary("Use your shortened URL to be redirected to the original URL!");
    
    internal  IResult? Handle([FromRoute] string shortenedUrl,
        [FromServices] UrlCollection collection,
        CancellationToken cancellationToken)
    {
        var originalUrlResult = collection.GetOriginalUrl(shortenedUrl);
        
        var urlVerification = VerifyUrl(originalUrlResult);
        if (urlVerification is not null)
        {
            return urlVerification;
        }

        collection.IncrementTelemetryForUrl(shortenedUrl);
        return Results.Redirect(originalUrlResult);
    }

    private static IResult? VerifyUrl(string originalUrlResult)
    {
        if (string.IsNullOrEmpty(originalUrlResult))
        {
            return Results.BadRequest(new RedirectResponse
            {
                ErrorDetail = "Invalid shortened URL - please call the `/shorten` endpoint to shorten a URL."
            });
        }
        return null;
    }
}