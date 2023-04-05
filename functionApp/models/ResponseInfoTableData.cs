using Azure;
using Azure.Data.Tables;

namespace functionApp.models;

public class ResponseInfoTableData : ITableEntity
{
    public string PartitionKey { get; set; } = Guid.NewGuid().ToString();

    public string RowKey { get; set; } = Guid.NewGuid().ToString();

    public DateTimeOffset? Timestamp { get; set; }

    public ETag ETag { get; set; }

    public bool Success { get; set; } = false;

    public string? ApiName { get; set; }

    public DateTimeOffset RequestTime { get; set; } = DateTimeOffset.Now;
}
