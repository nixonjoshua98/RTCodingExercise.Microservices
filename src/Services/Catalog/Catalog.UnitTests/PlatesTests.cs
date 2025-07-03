using Catalog.API.Services;
using Catalog.Domain;
using HttpClients.Catalog.Contracts.Dtos;
using HttpClients.Catalog.Contracts.Enumerations;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Catalog.UnitTests
{
    public class PlatesTests
    {
        [Theory]
        [InlineData(PlateSortOrder.Registration)]
        [InlineData(PlateSortOrder.SalePrice)]
        [InlineData(PlateSortOrder.PurchasePrice)]
        public async Task PlatesPaginationShouldBeOrdered(PlateSortOrder sort)
        {
            using var context = TestHelper.CreateInMemoryContext();

            for (int i = 0; i < 10; i++)
            {
                context.Plates.Add(new Plate { Registration = Guid.NewGuid().ToString(), SalePrice = i, PurchasePrice = i });
            }

            context.SaveChanges();

            var svc = new PlatesService(context, TestHelper.CreateMockLogger<PlatesService>());

            var results = await svc.GetPaginatedListingPlatesAsync(0, null, sort, CancellationToken.None);

            Func<ListingPlateDto, object?> filterFunc = sort switch
            {
                PlateSortOrder.Registration => x => x.Registration,
                PlateSortOrder.SalePrice => x => x.SalePrice,
                PlateSortOrder.PurchasePrice => x => x.PurchasePrice,
                _ => throw new Exception("Unreachable")
            };

            Assert.Equal(
                results.Plates.OrderBy(filterFunc),
                results.Plates
            );
        }

        [Theory]
        [InlineData(10, false)]
        [InlineData(20, false)]
        [InlineData(50, true)]
        public async Task PlatesPaginationResultShouldHintRemainingResults(int numValues, bool expectedValue)
        {
            using var context = TestHelper.CreateInMemoryContext();

            context.Plates.AddRange(Enumerable.Range(0, numValues).Select(x => new Plate()));

            context.SaveChanges();

            var svc = new PlatesService(context, TestHelper.CreateMockLogger<PlatesService>());

            var results = await svc.GetPaginatedListingPlatesAsync(0, null, PlateSortOrder.Registration, CancellationToken.None);

            Assert.True(expectedValue == results.HasMore);
        }

        [Theory]
        [InlineData(null, 4)]
        [InlineData("PL", 4)]
        [InlineData("P", 4)]
        [InlineData("0", 2)]
        [InlineData("33", 1)]
        public async Task PlatesQueryingShouldReturnMatching(string? query, int expectedResultCount)
        {
            using var context = TestHelper.CreateInMemoryContext();

            context.Plates.Add(new Plate { Registration = "PLATE0NE", Numbers = 0, Letters = "PLATENE" });
            context.Plates.Add(new Plate { Registration = "PLATETW4", Numbers = 4, Letters = "PLATETW" });
            context.Plates.Add(new Plate { Registration = "PLATETHR33", Numbers = 33, Letters = "PLATETHR" });
            context.Plates.Add(new Plate { Registration = "PLATEF0UR", Numbers = 0, Letters = "PLATEFUR" });

            context.SaveChanges();

            var svc = new PlatesService(context, TestHelper.CreateMockLogger<PlatesService>());

            var results = await svc.GetPaginatedListingPlatesAsync(0, query, PlateSortOrder.Registration, CancellationToken.None);

            Assert.Equal(expectedResultCount, results.Plates.Count);
        }

        [Fact]
        public async Task ReservedPlatesShouldbeExcludedFromSearch()
        {
            using var context = TestHelper.CreateInMemoryContext();

            context.Plates.Add(new Plate { Availablity = Domain.PlateAvailablity.Available });
            context.Plates.Add(new Plate { Availablity = Domain.PlateAvailablity.Reserved });

            context.SaveChanges();

            var svc = new PlatesService(context, TestHelper.CreateMockLogger<PlatesService>());

            var results = await svc.GetPaginatedListingPlatesAsync(0, null, PlateSortOrder.Registration, CancellationToken.None);

            Assert.Equal(1, results.Plates.Count);
        }

        [Fact]
        public async Task SoldPlatesShouldBeExcludedFromSearch()
        {
            using var context = TestHelper.CreateInMemoryContext();

            context.Plates.Add(new Plate { Availablity = Domain.PlateAvailablity.Sold });
            context.Plates.Add(new Plate { Availablity = Domain.PlateAvailablity.Available });
            context.Plates.Add(new Plate { Availablity = Domain.PlateAvailablity.Available });
            context.Plates.Add(new Plate { Availablity = Domain.PlateAvailablity.Reserved });

            context.SaveChanges();

            var svc = new PlatesService(context, TestHelper.CreateMockLogger<PlatesService>());

            var results = await svc.GetPaginatedListingPlatesAsync(0, null, PlateSortOrder.Registration, CancellationToken.None);

            Assert.Equal(2, results.Plates.Count);
        }

        [Fact]
        public async Task SoldPlatesShouldIncrementRevenue()
        {
            using var context = TestHelper.CreateInMemoryContext();

            var plate = new Plate { Availablity = Domain.PlateAvailablity.Available, SalePrice = 100 };

            context.Plates.Add(plate);

            context.SaveChanges();

            var svc = new PlatesService(context, TestHelper.CreateMockLogger<PlatesService>());

            var results = await svc.SellPlateAsync(plate.Id, null, CancellationToken.None);

            var revenue = await svc.GetRevenueAsync(CancellationToken.None);

            Assert.Equal(100m, revenue.TotalRevenue);
        }

        [Fact]
        public async Task DiscountedSoldPlatesShouldIncrementRevenueByDiscountedPrice()
        {
            using var context = TestHelper.CreateInMemoryContext();

            var plate = new Plate { Availablity = Domain.PlateAvailablity.Available, SalePrice = 100 };
            var code = new DiscountCode { Code = "CODE", Type = DiscountCodeType.Percentage, Value = 0.1m };

            context.Plates.Add(plate);

            context.DiscountCodes.Add(code);

            context.SaveChanges();

            var svc = new PlatesService(context, TestHelper.CreateMockLogger<PlatesService>());

            var result = await svc.SellPlateAsync(plate.Id, code, CancellationToken.None);

            var revenue = await svc.GetRevenueAsync(CancellationToken.None);

            Assert.Equal(90m, revenue.TotalRevenue);
        }

        [Fact]
        public async Task HeavyDiscountedPlateSaleShouldBeRejected()
        {
            using var context = TestHelper.CreateInMemoryContext();

            var plate = new Plate { Availablity = Domain.PlateAvailablity.Available, SalePrice = 100 };
            var code = new DiscountCode { Code = "CODE", Type = DiscountCodeType.Percentage, Value = 0.11m };

            context.Plates.Add(plate);

            context.DiscountCodes.Add(code);

            context.SaveChanges();

            var svc = new PlatesService(context, TestHelper.CreateMockLogger<PlatesService>());

            var result = await svc.SellPlateAsync(plate.Id, code, CancellationToken.None);

            Assert.False(result.IsSold);
        }
    }
}