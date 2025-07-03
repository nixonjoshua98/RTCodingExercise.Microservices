using HttpClients.Catalog.Contracts.Dtos;

namespace Catalog.API.Models
{
    internal sealed record PaginatedPlatesList(IReadOnlyList<ListingPlateDto> Plates, bool HasMore);
}
