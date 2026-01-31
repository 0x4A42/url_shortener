namespace UrlShortener.Model.Request;

public class PurgeRequest
{
    public required int RemoveAfterDays { get; set; } = 7; // default to 1 week if not set
}