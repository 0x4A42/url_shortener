namespace UrlShortener.Model;

public class ShortenResponse
{
    public required string OriginalUrl { get; set; }
    public required string ShortenedUrl { get; set; }
    public required string Detail { get; set; }
}