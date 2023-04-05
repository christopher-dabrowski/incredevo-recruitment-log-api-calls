using functionApp.httpClients;
using functionApp.services;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Extensions.DependencyInjection;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(worker => worker.UseNewtonsoftJson())
    .ConfigureServices(services =>
    {
        services.AddTransient<IClock, Clock>();
        services.AddHttpClient<IPublicApisHttpClient, PublicApisHttpClient>();
    })
    .Build();

host.Run();
