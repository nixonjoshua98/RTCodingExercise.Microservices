namespace Catalog.Domain
{
    public class Plate
    {
        public Guid Id { get; set; }

        public string? Registration { get; set; }

        public decimal PurchasePrice { get; set; }

        public decimal SalePrice { get; init; }

        public string? Letters { get; set; }

        public int Numbers { get; set; }

        public PlateAvailablity Availablity { get; set; } = PlateAvailablity.Available;

        public decimal? SoldPrice { get; set; }

        public void Reserve()
        {
            if (Availablity != PlateAvailablity.Available)
            {
                throw new Exception("Plate cannot be reserved");
            }

            Availablity = PlateAvailablity.Reserved;
        }

        public void Sell(decimal soldPrice)
        {
            if (Availablity == PlateAvailablity.Sold)
            {
                throw new Exception("Plate has already been sold");
            }

            SoldPrice = soldPrice;
            Availablity = PlateAvailablity.Sold;
        }
    }
}