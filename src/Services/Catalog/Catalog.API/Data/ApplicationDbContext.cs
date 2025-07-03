namespace Catalog.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Plate>(cfg =>
            {
                cfg.Property(x => x.Availablity).IsConcurrencyToken();
            });
        }

        public DbSet<Plate> Plates { get; set; } = default!;
        public DbSet<DiscountCode> DiscountCodes { get; set; } = default!;
    }
}
