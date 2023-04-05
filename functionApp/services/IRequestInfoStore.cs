using functionApp.models;
using functionApp.models.responses;

namespace functionApp.services;

public interface IRequestInfoStore
{
    Task StoreSuccessfulResponse(PublicApisApiResponse response, DateTimeOffset requestTime, CancellationToken cancellationToken = default);

    Task StoreUnsuccessfulResponse(DateTimeOffset requestTime, CancellationToken cancellationToken = default);

    Task<IEnumerable<ResponseInfoTableData>> ListResponseInfo(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);

    Task<PublicApisApiResponse?> GetResponseData(string responseId, CancellationToken cancellationToken = default);
}
