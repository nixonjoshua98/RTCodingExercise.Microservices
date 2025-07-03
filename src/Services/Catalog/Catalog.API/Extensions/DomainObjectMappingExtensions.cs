using DomainPlateAvailablity = Catalog.Domain.PlateAvailablity;

using PublicPlateAvailablity = HttpClients.Catalog.Contracts.Enumerations.PlateAvailablity;

namespace Catalog.API.Extensions
{
    internal static class DomainObjectMappingExtensions
    {
        public static PublicPlateAvailablity ToPublic(this DomainPlateAvailablity data)
        {
            return data switch
            {
                DomainPlateAvailablity.Available => PublicPlateAvailablity.Unreserved,
                DomainPlateAvailablity.Sold => PublicPlateAvailablity.Sold,
                DomainPlateAvailablity.Reserved => PublicPlateAvailablity.Reserved,
                _ => throw new NotImplementedException()
            };
        }
    }
}
