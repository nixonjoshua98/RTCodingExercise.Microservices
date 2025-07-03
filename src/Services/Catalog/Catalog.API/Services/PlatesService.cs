using Catalog.API.Abstractions;
using Catalog.API.Extensions;
using Catalog.API.Models;
using HttpClients.Catalog.Contracts.Dtos;
using HttpClients.Catalog.Contracts.Enumerations;
using System.Text.RegularExpressions;

namespace Catalog.API.Services
{
    internal sealed class PlatesService : IPlatesService
    {
        const int PageSize = 20;

        private readonly ApplicationDbContext _context;
        private readonly ILogger<PlatesService> _logger;

        public PlatesService(ApplicationDbContext context, ILogger<PlatesService> logger)
        {
            _context = context; 
            _logger = logger;
        }

        public async Task<PlatesRevenueDto> GetRevenueAsync(CancellationToken cancellationToken)
        {
            var data = await _context.Plates
                .Where(x => x.Availablity == Domain.PlateAvailablity.Sold && x.SoldPrice.HasValue)
                .SumAsync(x => x.SoldPrice!.Value, cancellationToken);

            return new PlatesRevenueDto(data);
        }

        public async Task<SellPlateResult> SellPlateAsync(Guid plateId, DiscountCode? discountCode, CancellationToken cancellationToken)
        {
            var plate = await _context.Plates.SingleOrDefaultAsync(x => x.Id == plateId, cancellationToken)
                ?? throw new Exception("Plate not found");

            var sellPrice = discountCode is null ? 
                plate.SalePrice : 
                discountCode.ApplyToNumber(plate.SalePrice);

            if (sellPrice / plate.SalePrice < 0.9m)
            {
                return new SellPlateResult(IsSold: false, "Discounted price exceeds the safe boundaries");
            }

            plate.Sell(sellPrice);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Plate {PlateId} has been sold", plateId);

            return new SellPlateResult(IsSold: true, ErrorMessage: null);
        }

        public async Task ReservePlateAsync(Guid plateId, CancellationToken cancellationToken)
        {
            var plate = await _context.Plates.SingleOrDefaultAsync(x => x.Id == plateId, cancellationToken)
                ?? throw new Exception("Plate not found");

            plate.Reserve();

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Plate {PlateId} availablity changed to Reserved", plateId);
        }

        public async Task<PaginatedPlatesList> GetPaginatedListingPlatesAsync(int page, string? query, PlateSortOrder sort, CancellationToken cancellationToken)
        {
            var queryable = CreateOrderedPlateQueryable(sort)
                .Where(x => x.Availablity == Domain.PlateAvailablity.Available);

            if (!string.IsNullOrEmpty(query))
            {
                queryable = ApplyQueryFiltering(query, queryable);
            }

            var results = await queryable
                .Skip(page * PageSize)
                .Take(PageSize + 1)
                .Select(x => new { x.Id, x.Registration, x.PurchasePrice, x.SalePrice, x.Availablity })
                .ToListAsync(cancellationToken);

            var hasMore = results.Count > PageSize;

            if (hasMore)
            {
                results.RemoveAt(results.Count - 1); // Remove the extra we used to check if more exist
            }

            var contracts = results
                .Select(x => new ListingPlateDto(x.Id, x.Registration, x.PurchasePrice, x.SalePrice, x.Availablity.ToPublic()))
                .ToList();

            return new PaginatedPlatesList(contracts, hasMore);
        }

        private static IQueryable<Plate> ApplyQueryFiltering(string query, IQueryable<Plate> queryable)
        {
            // Net 9.0 we can use a compiled regex property
            var letters = Regex.Matches(query, "[a-zA-Z]+");
            var numbers = Regex.Matches(query, @"\d+");

            if (numbers.Count > 0)
            {
                var number = int.Parse(string.Join(string.Empty, numbers.Select(x => x.Value)));

                queryable = queryable
                    .Where(x => x.Numbers == number);
            }

            if (letters.Count > 0)
            {
                var str = string.Join(string.Empty, letters.Select(x => x.Value));

                queryable = queryable
                    .Where(x => !string.IsNullOrEmpty(x.Letters) && x.Letters.Contains(str));
            }

            return queryable;
        }

        IOrderedQueryable<Plate> CreateOrderedPlateQueryable(PlateSortOrder sort)
        {
            return sort switch
            {
                PlateSortOrder.PurchasePrice => _context.Plates.OrderBy(x => x.PurchasePrice),
                PlateSortOrder.SalePrice => _context.Plates.OrderBy(x => x.SalePrice),
                _ => _context.Plates.OrderBy(x => x.Registration),
            };
        }
    }
}
