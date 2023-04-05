using System.Text.Json;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using functionApp.extensions;
using functionApp.httpClients;
using functionApp.models;
using functionApp.services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace functionApp;

public class StoreApiResponse
{
    private readonly ILogger _logger;
    private readonly IPublicApisHttpClient _publicApisHttpClient;
    private readonly IConfiguration _configuration;
    private readonly IClock _clock;
    private readonly IRequestInfoStore _requestInfoStore;

    public StoreApiResponse(ILoggerFactory loggerFactory, IPublicApisHttpClient publicApisHttpClient, IConfiguration configuration, IClock clock, IRequestInfoStore requestInfoStore)
    {
        _publicApisHttpClient = publicApisHttpClient;
        _configuration = configuration;
        _clock = clock;
        _requestInfoStore = requestInfoStore;
        _logger = loggerFactory.CreateLogger<StoreApiResponse>();
    }

    [Function("StoreApiResponse")]
    public async Task Run([TimerTrigger("0 */1 * * * *", RunOnStartup = true)] TimerTriggerInfo triggerInfo, CancellationToken cancellationToken)
    {
        var currentTime = _clock.DateTimeOffsetNow;
        _logger.LogInformation($"C# Timer trigger function executed at: {currentTime}");

        try
        {
            var apiResponse = await _publicApisHttpClient.GetRanomApiInfo();
            _logger.LogInformation(JsonSerializer.Serialize(apiResponse));

            if (apiResponse is null)
                throw new InvalidOperationException("Unable to parse api response");

            await _requestInfoStore.StoreSuccessfulResponse(apiResponse, currentTime, cancellationToken);
        }
        catch
        {
            await _requestInfoStore.StoreUnsuccessfulResponse(currentTime, cancellationToken);
        }
    }
}
