using UrlShortener.Url;

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
        var urlEndpoints = app.MapGroup("/url")
            .WithTags("Urls");

        urlEndpoints.MapPublicGroup()
            .MapEndpoint<Shorten>()
            .MapEndpoint<Redirect>();
        
        var systemEndpoints = app.MapGroup("")
            .WithTags("Sys");
        
        systemEndpoints.MapAuthorizedGroup()
            .MapEndpoint<Purge>();
    }
    
    private static RouteGroupBuilder MapPublicGroup(this IEndpointRouteBuilder app, string? prefix = null)
    {
        return app.MapGroup(prefix ?? string.Empty)
            .AllowAnonymous();
    }
    
    private static RouteGroupBuilder MapAuthorizedGroup(this IEndpointRouteBuilder app, string? prefix = null)
    {
        return app.MapGroup(prefix ?? string.Empty)
            .RequireAuthorization();
    }
    
    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app) where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}
