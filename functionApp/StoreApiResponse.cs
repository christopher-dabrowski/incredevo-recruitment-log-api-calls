using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace functionApp;

public class StoreApiResponse
{
    private readonly ILogger _logger;

    public StoreApiResponse(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<StoreApiResponse>();
    }

    [Function("StoreApiResponse")]
    public void Run([TimerTrigger("0 */1 * * * *", RunOnStartup = true)] TimerTriggerInfo myTimer)
    {
        _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
    }
}
