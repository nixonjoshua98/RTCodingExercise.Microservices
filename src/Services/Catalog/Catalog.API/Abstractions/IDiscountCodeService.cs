using HttpClients.Catalog.Contracts.Dtos;

namespace Catalog.API.Abstractions
{
    internal interface IDiscountCodeService
    {
        Task<IEnumerable<ListingPlateDto>> ApplyDiscountAsync(string? discountCode, IEnumerable<ListingPlateDto> plates, CancellationToken cancellationToken);
        Task<DiscountCode?> GetDiscountCodeAsync(string? discountCode, CancellationToken cancellationToken);
    }
}