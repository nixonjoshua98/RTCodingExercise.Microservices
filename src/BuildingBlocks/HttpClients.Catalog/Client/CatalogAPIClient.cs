using HttpClients.Catalog.Abstractions;
using HttpClients.Catalog.Contracts.Enumerations;
using HttpClients.Catalog.Contracts.Responses;
using System.Net.Http.Json;

namespace HttpClients.Catalog.Client
{
    internal sealed class CatalogAPIClient : ICatalogAPIClient
    {
        private readonly HttpClient _httpClient;

        public CatalogAPIClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GetPlatesListingResponse> GetListingPlatesAsync(
            int page, 
            string? query, 
            PlateSortOrder sort, 
            string? discountCode,
            CancellationToken cancellationToken)
        {
            using var response = await _httpClient.GetAsync(
                $"plates/listing/page/{page}?sort={sort}&query={query}&discountCode={discountCode}",  
                cancellationToken
            );

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<GetPlatesListingResponse>(cancellationToken: cancellationToken) 
                ?? throw new Exception("Plates listing found, but deserialization failed");
        }
    }
}
