using functionApp.httpClients.responses;

namespace functionApp.httpClients;

public interface IPublicApisHttpClient
{
    Task<PublicApisApiResponse?> GetRanomApiInfo();
}
