using HttpClients.Catalog.Contracts.Enumerations;
using HttpClients.Catalog.Contracts.Responses;

namespace HttpClients.Catalog.Abstractions
{
    public interface ICatalogAPIClient
    {
        Task<GetPlatesListingResponse> GetListingPlatesAsync(int page, string? query, PlateSortOrder sort, string? discountCode, CancellationToken cancellationToken);
    }
}