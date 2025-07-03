using Catalog.API.Models;
using HttpClients.Catalog.Contracts.Dtos;
using HttpClients.Catalog.Contracts.Enumerations;

namespace Catalog.API.Abstractions
{
    internal interface IPlatesService
    {
        Task<PaginatedPlatesList> GetPaginatedListingPlatesAsync(int page, string? query, PlateSortOrder sort, CancellationToken cancellationToken);
        Task<PlatesRevenueDto> GetRevenueAsync(CancellationToken cancellationToken);
        Task ReservePlateAsync(Guid plateId, CancellationToken cancellationToken);
        Task<SellPlateResult> SellPlateAsync(Guid plateId, DiscountCode? discountCode, CancellationToken cancellationToken);
    }
}
