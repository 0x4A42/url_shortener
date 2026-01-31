namespace UrlShortener.Url;

public interface IEndpoint
{
    static abstract void Map(IEndpointRouteBuilder app);
}