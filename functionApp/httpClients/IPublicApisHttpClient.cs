using functionApp.models.responses;

namespace functionApp.httpClients;

public interface IPublicApisHttpClient
{
    Task<PublicApisApiResponse?> GetRanomApiInfo();
}
