using Catalog.API.Abstractions;
using HttpClients.Catalog.Contracts.Dtos;

namespace Catalog.API.Services
{
    internal sealed class DiscountCodeService : IDiscountCodeService
    {
        private readonly ApplicationDbContext _context;

        public DiscountCodeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DiscountCode?> GetDiscountCodeAsync(string? discountCode, CancellationToken cancellationToken)
        {
            return await _context.DiscountCodes
                .SingleOrDefaultAsync(x => x.Code == discountCode, cancellationToken);
        }

        public async Task<IEnumerable<ListingPlateDto>> ApplyDiscountAsync(
            string? discountCode,
            IEnumerable<ListingPlateDto> plates,
            CancellationToken cancellationToken)
        {
            var code = await GetDiscountCodeAsync(discountCode, cancellationToken);

            if (code is null)
            {
                return plates;
            }

            return CreateDiscountedPlates(plates, code).ToList();
        }

        IEnumerable<ListingPlateDto> CreateDiscountedPlates(IEnumerable<ListingPlateDto> plates, DiscountCode code)
        {
            foreach (var plate in plates)
            {
                yield return plate with
                {
                    SalePrice = code.ApplyToNumber(plate.SalePrice)
                };
            }
        }


    }
}
