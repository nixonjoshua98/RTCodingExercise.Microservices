using HttpClients.Catalog.Contracts.Enumerations;
using WebMVC.Abstractions;
using WebMVC.Models;

namespace WebMVC.Services
{
    internal sealed class PlateFilterOptionsProvider : IPlateFilterOptionsProvider
    {
        /// <summary>
        /// List of plate sort options
        /// </summary>
        /// <remarks>
        /// Could have been done via reflection on the enum, but preferred to be more explicit
        /// </remarks>
        private readonly List<PlateSortOption> _sortOptions = new()
        {
            new PlateSortOption(PlateSortOrder.Registration, "Registration"),
            new PlateSortOption(PlateSortOrder.PurchasePrice, "Purchase price"),
            new PlateSortOption(PlateSortOrder.SalePrice, "Sale price")
        };

        public IEnumerable<PlateSortOption> GetSortOptions() => _sortOptions.AsReadOnly();
    }
}
