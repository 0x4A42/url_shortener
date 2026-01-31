using Microsoft.AspNetCore.Mvc;
using UrlShortener.Data;
using UrlShortener.Model;
using UrlShortener.Model.Request;

namespace UrlShortener.Url;

public class Purge(ILogger<Purge> logger): IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapDelete("/purge", (
            [FromBody] PurgeRequest request,
            [FromServices] UrlCollection collection,
            [FromServices] Purge purge,
            CancellationToken cancellationToken) => purge.Handle(request, collection, cancellationToken))
        .WithSummary("Purges shortened URLs which are no longer in use");

    internal IResult Handle(
        [FromBody] PurgeRequest request,
        [FromServices] UrlCollection collection,
        CancellationToken cancellationToken)
    {
        var numberOfPurgedUrls = 0;
        try
        {
            numberOfPurgedUrls = collection.PurgeStaleUrls(request.RemoveAfterDays);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error experienced during execution of Purge endpoint");
        }
        
        var detailMessage = numberOfPurgedUrls switch
        {
            0 => "No URLs were eligible for purging.",
            _ => $"{numberOfPurgedUrls} URL(s) successfully purged."
        };

        return Results.Ok(new PurgeResponse
        {
            Detail = detailMessage
        });
    }
}