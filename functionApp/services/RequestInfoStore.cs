using System.Text.Json;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using functionApp.extensions;
using functionApp.models;
using functionApp.models.responses;
using Microsoft.Extensions.Configuration;

namespace functionApp.services;

/// <summary>
/// Class responsible for persistent information about occurred requests
/// </summary>
public class RequestInfoStore : IRequestInfoStore
{
    private readonly TableClient _tableClient;
    private readonly BlobContainerClient _blobContainerClient;

    public RequestInfoStore(IConfiguration configuration)
    {
        _tableClient = new TableClient(configuration.GetStorageAccountConnectionString(), Config.TableName);

        var blobStorageClient = new BlobServiceClient(configuration.GetStorageAccountConnectionString());
        _blobContainerClient = blobStorageClient.GetBlobContainerClient(Config.StorageContainerName);
    }

    public async Task StoreSuccessfulResponse(PublicApisApiResponse response, DateTimeOffset requestTime, CancellationToken cancellationToken = default)
    {
        var randomApiInfo = response.entries.FirstOrDefault();

        var responseInfo = new ResponseInfoTableData
        {
            PartitionKey = randomApiInfo?.API ?? Guid.NewGuid().ToString(),
            Success = true,
            ApiName = randomApiInfo?.API,
            RequestTime = requestTime,
        };

        await _tableClient.CreateIfNotExistsAsync(cancellationToken);
        await _tableClient.AddEntityAsync(responseInfo, cancellationToken);

        await _blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        await _blobContainerClient.UploadBlobAsync(responseInfo.RowKey, BinaryData.FromString(JsonSerializer.Serialize(response)), cancellationToken);
    }

    public async Task StoreUnsuccessfulResponse(DateTimeOffset requestTime, CancellationToken cancellationToken = default)
    {
        await _tableClient.CreateIfNotExistsAsync(cancellationToken);

        var responseInfo = new ResponseInfoTableData
        {
            Success = false,
            RequestTime = requestTime
        };
        await _tableClient.AddEntityAsync(responseInfo, cancellationToken);
    }
}
