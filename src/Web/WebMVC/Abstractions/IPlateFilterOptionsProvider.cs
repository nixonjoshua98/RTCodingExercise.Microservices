using WebMVC.Models;

namespace WebMVC.Abstractions
{
    public interface IPlateFilterOptionsProvider
    {
        IEnumerable<PlateSortOption> GetSortOptions();
    }
}