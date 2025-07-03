using HttpClients.Catalog.Abstractions;
using HttpClients.Catalog.Contracts.Enumerations;
using RTCodingExercise.Microservices.Models;
using System.Diagnostics;

namespace RTCodingExercise.Microservices.Controllers
{
    public class PlatesController : Controller
    {
        private readonly ICatalogAPIClient _catalogClient;

        public PlatesController(ICatalogAPIClient catalogClient)
        {
            _catalogClient = catalogClient;
        }

        public async Task<IActionResult> Listing(
            int page = 0,
            string? query = null,
            PlateSortOrder sort = PlateSortOrder.Registration,
            string? discountCode = null)
        {
            var results = await _catalogClient.GetListingPlatesAsync(
                page, 
                query, 
                sort,
                discountCode,
                CancellationToken.None
            );

            var model = new PlatesListingViewModel
            {
                Page = page,
                HasMore = results.HasMore,
                Sort = sort,
                Plates = results.Plates.Select(PlateListingModel.CreateNew),
                Query = query,
                DiscountCode = discountCode
            };

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}