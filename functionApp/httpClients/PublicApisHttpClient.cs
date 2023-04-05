using System.Net.Http.Json;
using functionApp.httpClients.responses;

namespace functionApp.httpClients;

public class PublicApisHttpClient : IPublicApisHttpClient
{
    private readonly HttpClient _httpClient;

    public PublicApisHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;

        _httpClient.BaseAddress = new Uri("https://api.publicapis.org/");
    }

    public async Task<PublicApisApiResponse?> GetRanomApiInfo() =>
        await _httpClient.GetFromJsonAsync<PublicApisApiResponse>("random?auth=null");
}
