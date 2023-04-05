using System.Text.Json;
using functionApp.httpClients;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace functionApp;

public class StoreApiResponse
{
    private readonly ILogger _logger;
    private readonly IPublicApisHttpClient _publicApisHttpClient;

    public StoreApiResponse(ILoggerFactory loggerFactory, IPublicApisHttpClient publicApisHttpClient)
    {
        _publicApisHttpClient = publicApisHttpClient;
        _logger = loggerFactory.CreateLogger<StoreApiResponse>();
    }

    [Function("StoreApiResponse")]
    public async Task Run([TimerTrigger("0 */1 * * * *", RunOnStartup = true)] TimerTriggerInfo myTimer)
    {
        _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

        try
        {
            var apiResponse = await _publicApisHttpClient.GetRanomApiInfo();
            _logger.LogInformation(JsonSerializer.Serialize(apiResponse));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
