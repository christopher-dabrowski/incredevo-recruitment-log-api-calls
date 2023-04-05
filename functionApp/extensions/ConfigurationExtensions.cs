using Microsoft.Extensions.Configuration;

namespace functionApp.extensions;

public static class ConfigurationExtensions
{
    public static string GetStorageAccountConnectionString(this IConfiguration configuration) =>
        configuration["AzureWebJobsStorage"];
}
