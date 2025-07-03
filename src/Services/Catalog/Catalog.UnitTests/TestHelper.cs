using Catalog.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;

namespace Catalog.UnitTests
{
    internal static class TestHelper
    {
        public static ApplicationDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        public static ILogger<T> CreateMockLogger<T>() => Substitute.For<ILoggerFactory>().CreateLogger<T>();
    }
}