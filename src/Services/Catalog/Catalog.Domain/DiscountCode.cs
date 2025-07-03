namespace Catalog.Domain
{

    public sealed class DiscountCode
    {
        public Guid Id { get; set; }

        public string Code { get; init; } = default!;

        public DiscountCodeType Type { get; init; }

        public decimal Value { get; init; }

        public decimal ApplyToNumber(decimal value)
        {
            return Type switch
            {
                DiscountCodeType.Value => value - Value,
                DiscountCodeType.Percentage => value * (1 - Value),
                _ => throw new NotImplementedException()
            };
        }
    }
}
