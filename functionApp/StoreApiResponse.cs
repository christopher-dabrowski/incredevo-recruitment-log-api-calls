using System.Text.Json;
using Azure;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using functionApp.httpClients;
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

    public StoreApiResponse(ILoggerFactory loggerFactory, IPublicApisHttpClient publicApisHttpClient, IConfiguration configuration, IClock clock)
    {
        _publicApisHttpClient = publicApisHttpClient;
        _configuration = configuration;
        _clock = clock;
        _logger = loggerFactory.CreateLogger<StoreApiResponse>();
    }

    [Function("StoreApiResponse")]
    public async Task Run([TimerTrigger("0 */1 * * * *", RunOnStartup = true)] TimerTriggerInfo myTimer)
    {
        var currentTime = _clock.DateTimeOffsetNow;
        _logger.LogInformation($"C# Timer trigger function executed at: {currentTime}");

        TableClient tableClient = new TableClient(_configuration["AzureWebJobsStorage"], "TestTable");
        await tableClient.CreateIfNotExistsAsync();

        try
        {
            var apiResponse = await _publicApisHttpClient.GetRanomApiInfo();
            _logger.LogInformation(JsonSerializer.Serialize(apiResponse));

            if (apiResponse is null)
                throw new InvalidOperationException("Unable to parse api response");

            var randomApiInfo = apiResponse.entries.First();

            var result = new MyTableData
            {
                PartitionKey = randomApiInfo.API,
                Success = true,
                ApiName = randomApiInfo.API,
                RequestTime = currentTime,
            };

            await tableClient.AddEntityAsync(result);

            var blobStorageClient = new BlobServiceClient(_configuration["AzureWebJobsStorage"]);
            var info = await blobStorageClient.GetAccountInfoAsync();

            var blobContainer = blobStorageClient.GetBlobContainerClient("testcontainer");
            await blobContainer.CreateIfNotExistsAsync();

            await blobContainer.UploadBlobAsync(result.RowKey, BinaryData.FromString(JsonSerializer.Serialize(apiResponse)));
        }
        catch
        {
            var result = new MyTableData
            {
                Success = false,
                RequestTime = currentTime
            };
            await tableClient.AddEntityAsync(result);
        }
    }

    public class MyTableData : Azure.Data.Tables.ITableEntity
    {
        public string PartitionKey { get; set; } = Guid.NewGuid().ToString();

        public string RowKey { get; set; } = Guid.NewGuid().ToString();

        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public bool Success { get; set; } = false;

        public string? ApiName { get; set; }

        public DateTimeOffset RequestTime { get; set; } = DateTimeOffset.Now;
    }
}
