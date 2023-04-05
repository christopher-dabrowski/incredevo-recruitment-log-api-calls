using functionApp.models.responses;

namespace functionApp.services;

public interface IRequestInfoStore
{
    Task StoreSuccessfulResponse(PublicApisApiResponse response, DateTimeOffset requestTime, CancellationToken cancellationToken = default);

    Task StoreUnsuccessfulResponse(DateTimeOffset requestTime, CancellationToken cancellationToken = default);
}
