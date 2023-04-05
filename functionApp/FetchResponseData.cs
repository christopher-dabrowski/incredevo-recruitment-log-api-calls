using System.Net;
using functionApp.models.responses;
using functionApp.services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace functionApp;

public class FetchResponseData
{
    private readonly IRequestInfoStore _requestInfoStore;
    private readonly ILogger _logger;

    public FetchResponseData(ILoggerFactory loggerFactory, IRequestInfoStore requestInfoStore)
    {
        _requestInfoStore = requestInfoStore;
        _logger = loggerFactory.CreateLogger<FetchResponseData>();
    }

    [Function("FetchResponseData")]
    [OpenApiOperation(operationId: "greeting", Summary = "Download response", Description = "Download data of a specific response", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "id of log to download data")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/json", bodyType: typeof(PublicApisApiResponse), Summary = "The response")]
    [OpenApiResponseWithoutBody(HttpStatusCode.NotFound)]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "responses/{id}")] HttpRequestData req, string id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var apiResponse = await _requestInfoStore.GetResponseData(id, cancellationToken);
        if (apiResponse == null)
        {
            return req.CreateResponse(HttpStatusCode.NotFound);
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(apiResponse, cancellationToken);

        return response;
    }
}
