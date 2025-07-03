using HttpClients.Catalog.Contracts.Dtos;
using HttpClients.Catalog.Contracts.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace RTCodingExercise.Microservices.Models
{
    public sealed class PlatesListingViewModel
    {
        [Range(0, int.MaxValue)]
        public int Page { get; set; }

        public bool HasMore { get; set; }

        public PlateSortOrder Sort { get; set; } = PlateSortOrder.Registration;

        public string? Query { get; set; } = null;

        public string? DiscountCode { get; set; } = null;

        public IEnumerable<PlateListingModel> Plates { get; set; } = Enumerable.Empty<PlateListingModel>();
    }

    public sealed class PlateListingModel
    {
        public Guid Id { get; init; }

        public string? Registration { get; init; }

        public decimal PurchasePrice { get; init; }

        public decimal SalePrice { get; init; }

        public PlateAvailablity Availablity { get; init; }

        public static PlateListingModel CreateNew(ListingPlateDto data)
        {
            return new PlateListingModel()
            {
                Id = data.Id,
                PurchasePrice = data.PurchasePrice,
                Registration = data.Registration,
                SalePrice = data.SalePrice,
                Availablity = data.Availablity
            };
        }
    }
}
