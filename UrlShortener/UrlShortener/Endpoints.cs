using UrlShortener.Interface;
using UrlShortener.Url.Endpoints;

namespace UrlShortener;

public static class Endpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("");
        
        endpoints.MapUrlEndpoints();
    }
    
    private static void MapUrlEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/url")
            .WithTags("Urls");

        endpoints.MapPublicGroup()
            .MapEndpoint<Shorten>();
    }
    
    private static RouteGroupBuilder MapPublicGroup(this IEndpointRouteBuilder app, string? prefix = null)
    {
        return app.MapGroup(prefix ?? string.Empty)
            .AllowAnonymous();
    }
    
    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app) where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}
