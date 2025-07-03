using HttpClients.Catalog.Contracts.Dtos;

namespace HttpClients.Catalog.Contracts.Responses
{
    public sealed record GetPlatesListingResponse(IEnumerable<ListingPlateDto> Plates, bool HasMore);
}
