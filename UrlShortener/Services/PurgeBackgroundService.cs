using UrlShortener.Data;

namespace UrlShortener.Services;

public class PurgeBackgroundService(
    IServiceProvider serviceProvider,
    ILogger<PurgeBackgroundService> logger,
    IConfiguration configuration)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var intervalHours = configuration.GetValue("Purge:ServiceIntervalHours", 24);
        
        logger.LogInformation("Purge background service started. Running every {Hours} hours.", intervalHours);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromHours(intervalHours), stoppingToken);
                
                await PurgeStaleUrlsAsync(stoppingToken);
            }
            catch (TaskCanceledException ex)
            {
                logger.LogInformation(ex, "The purge background service is stopping.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in purge background service");
            }
        }
    }

    private Task PurgeStaleUrlsAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var urlCollection = scope.ServiceProvider.GetRequiredService<UrlCollection>();
        var numberOfDays = configuration.GetValue("Purge:RemoveAfterDays", 7);

        logger.LogInformation("Starting scheduled purge of URLs older than {Days} days", numberOfDays);
        
        var purgedCount = urlCollection.PurgeStaleUrls(numberOfDays);
        
        logger.LogInformation("Scheduled purge completed. Purged {Count} URLs.", purgedCount);
        
        return Task.CompletedTask;
    }
}