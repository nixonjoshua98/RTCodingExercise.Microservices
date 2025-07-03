using HttpClients.Catalog.Contracts.Enumerations;

namespace HttpClients.Catalog.Contracts.Dtos
{
    public sealed record ListingPlateDto(
        Guid Id,
        string? Registration, 
        decimal PurchasePrice,
        decimal SalePrice,
        PlateAvailablity Availablity
    );
}