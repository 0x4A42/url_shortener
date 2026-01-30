namespace UrlShortener.Model;

public class ShortenRequest
{
    public required string Url { get; set; }
    public int Length { get; set; } = 7;  // default to 7 if nothing is provided
}