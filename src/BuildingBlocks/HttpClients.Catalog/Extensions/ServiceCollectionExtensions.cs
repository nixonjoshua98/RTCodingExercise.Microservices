using HttpClients.Catalog.Abstractions;
using HttpClients.Catalog.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HttpClients.Catalog.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCatalogApiClient(
            this IServiceCollection services, 
            IConfiguration configuration,
            string configurationKey = "CatalogApi:InternalBaseUrl")
        {
            var baseUrl = configuration[configurationKey];

            Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri); // Validates and throws

            services.AddHttpClient<ICatalogAPIClient, CatalogAPIClient>(cfg =>
            {
                cfg.BaseAddress = uri;
            });

            return services;
        }
    }
}
