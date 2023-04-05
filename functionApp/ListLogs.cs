using System.Collections.Specialized;
using System.Net;
using functionApp.models;
using functionApp.services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace functionApp;

public class ListLogs
{
    private readonly IRequestInfoStore _requestInfoStore;
    private readonly ILogger _logger;

    public ListLogs(ILoggerFactory loggerFactory, IRequestInfoStore requestInfoStore)
    {
        _requestInfoStore = requestInfoStore;
        _logger = loggerFactory.CreateLogger<ListLogs>();
    }

    [OpenApiOperation(operationId: "ListLogs", Summary = "List logs from specific time frame", Description = "List logs from specific time frame", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiParameter(name: "from", In = ParameterLocation.Query, Required = false, Type = typeof(DateTimeOffset), Description = "ISO 8601 format - defaults to current time -1h")]
    [OpenApiParameter(name: "to", In = ParameterLocation.Query, Required = false, Type = typeof(DateTimeOffset), Description = "ISO 8601 format - defaults to current time")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/json", bodyType: typeof(IList<ResponseInfoTableData>), Summary = "List api response logs")]
    [Function("ListLogs")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req, CancellationToken cancellationToken)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        try
        {
            var (from, to) = ParseQueryParams(req.Query);

            var apiResponsesList = await _requestInfoStore.ListResponseInfo(from, to, cancellationToken);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(apiResponsesList, cancellationToken);

            return response;
        }
        catch (FormatException)
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }
    }

    private QueryParams ParseQueryParams(NameValueCollection query)
    {
        var from = query["from"] is { } fromString ? DateTimeOffset.Parse(fromString) : DateTimeOffset.Now.AddHours(-1);
        var to = query["to"] is { } toString ? DateTimeOffset.Parse(toString) : DateTimeOffset.Now;

        return new(from, to);
    }

    public record QueryParams(DateTimeOffset From, DateTimeOffset To);
}
