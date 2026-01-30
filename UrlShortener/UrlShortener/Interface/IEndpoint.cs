namespace UrlShortener.Interface;

public interface IEndpoint
{
    static abstract void Map(IEndpointRouteBuilder app);
}