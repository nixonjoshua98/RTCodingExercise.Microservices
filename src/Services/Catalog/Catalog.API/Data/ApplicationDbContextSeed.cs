using Newtonsoft.Json;

namespace Catalog.API.Data
{
    public class ApplicationDbContextSeed
    {
        public async Task SeedAsync(ApplicationDbContext context, IWebHostEnvironment env, ILogger<ApplicationDbContextSeed> logger, IOptions<AppSettings> settings, int? retry = 0)
        {
            int retryForAvaiability = retry.Value;

            try
            {
                await SeedDiscountCodesAsync(context, logger);

                await SeedPlatesAsync(context, env, logger);
            }
            catch (Exception ex)
            {
                // used for initilisaton of docker containers
                if (retryForAvaiability < 10)
                {
                    retryForAvaiability++;

                    logger.LogError(ex.Message, $"There is an error migrating data for ApplicationDbContext");

                    await SeedAsync(context, env, logger, settings, retryForAvaiability);
                }
            }
        }

        public async Task SeedDiscountCodesAsync(ApplicationDbContext context, ILogger<ApplicationDbContextSeed> logger)
        {
            try
            {
                await context.DiscountCodes.AddRangeAsync(new DiscountCode[]
                {
                     new DiscountCode { Id = Guid.Parse("7C88B586-AABA-400A-8EF2-AF2073FC0CB2"), Code = "DISCOUNT", Type = DiscountCodeType.Value, Value = 25 },
                     new DiscountCode { Id = Guid.Parse("C3AAE272-56E3-4DB3-80E2-E4C686820EAA"), Code = "PERCENTOFF", Type = DiscountCodeType.Percentage, Value = 0.15m }
                });

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
                throw;
            }
        }

        public async Task SeedPlatesAsync(ApplicationDbContext context, IWebHostEnvironment env, ILogger<ApplicationDbContextSeed> logger)
        {
            try
            {
                var plates = ReadApplicationRoleFromJson(env.ContentRootPath, logger);

                await context.Plates.AddRangeAsync(plates);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
                throw;
            }
        }

        public List<Plate> ReadApplicationRoleFromJson(string contentRootPath, ILogger<ApplicationDbContextSeed> logger)
        {
            string filePath = Path.Combine(contentRootPath, "Setup", "plates.json");
            string json = File.ReadAllText(filePath);

            var plates = JsonConvert.DeserializeObject<List<Plate>>(json) ?? new List<Plate>();

            return plates;
        }
    }
}
