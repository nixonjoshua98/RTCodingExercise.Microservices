using Catalog.API.Abstractions;
using HttpClients.Catalog.Contracts.Enumerations;
using HttpClients.Catalog.Contracts.Responses;

namespace Catalog.API.Endpoints
{
    internal sealed record SellPlateBody(string? DiscountCode);

    internal static class PlateEndpoints
    {
        public static IEndpointRouteBuilder MapPlatesEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("plates/listing/page/{page}", GetPaginatedPlatesAsync);

            app.MapGet("plates/revenue", GetPlatesRevenueAsync);

            app.MapPost("plates/reserve/{plateId}", ReservePlateAsync);

            app.MapPost("plates/sell/{plateId}", SellPlateAsync);

            return app;
        }

        static async Task<IResult> GetPlatesRevenueAsync(
            IPlatesService platesService,
            CancellationToken cancellationToken)
        {
            var data = await platesService.GetRevenueAsync(cancellationToken);

            return Results.Ok(data);
        }

        static async Task<IResult> SellPlateAsync(
            [FromRoute] Guid plateId,
            [FromBody] SellPlateBody body,
            IPlatesService platesService,
            IDiscountCodeService discountCodeService,
            CancellationToken cancellationToken)
        {

            var discountCode = await discountCodeService.GetDiscountCodeAsync(body.DiscountCode, cancellationToken);

            var result = await platesService.SellPlateAsync(plateId, discountCode, cancellationToken);

            return result.IsSold ?
                Results.NoContent() :
                Results.BadRequest(result.ErrorMessage);
        }

        static async Task<IResult> ReservePlateAsync(
            [FromRoute] Guid plateId,
            IPlatesService platesService,
            CancellationToken cancellationToken)
        {
            await platesService.ReservePlateAsync(plateId, cancellationToken);

            return Results.NoContent();
        }

        static async Task<IResult> GetPaginatedPlatesAsync(
            [FromRoute] int page,
            [FromQuery] PlateSortOrder sort,
            [FromQuery] string? query,
            [FromQuery] string? discountCode,
            IPlatesService platesService,
            IDiscountCodeService discountCodeService,
            CancellationToken cancellationToken)
        {
            var results = await platesService.GetPaginatedListingPlatesAsync(page, query, sort, cancellationToken);

            var plates = await discountCodeService.ApplyDiscountAsync(discountCode, results.Plates, cancellationToken);

            var resp = new GetPlatesListingResponse(
                plates,
                results.HasMore
            );

            return Results.Ok(resp);
        }
    }
}
