using HttpClients.Catalog.Contracts.Enumerations;

namespace WebMVC.Models
{
    public sealed class PlateSortOption
    {
        public PlateSortOption(PlateSortOrder value, string displayName)
        {
            Value = value;
            DisplayName = displayName;
        }

        public PlateSortOrder Value { get; init; }
        public string DisplayName { get; init; }
    }
}
