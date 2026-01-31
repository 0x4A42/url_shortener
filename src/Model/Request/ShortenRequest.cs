namespace UrlShortener.Model.Request;

public class ShortenRequest
{
    public required string Url { get; init; }
    public int Length { get; init; } = 7;  // default to 7 if nothing is provided
}