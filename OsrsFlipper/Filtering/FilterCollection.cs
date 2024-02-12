using OsrsFlipper.Caching;

namespace OsrsFlipper.Filtering;

internal class FilterCollection
{
    private readonly List<FlipFilter> _filters = new();


    public FilterCollection AddFilter(FlipFilter filter)
    {
        _filters.Add(filter);
        return this;
    }
    
    
    public bool PassesAllFilters(CacheEntry itemData)
    {
        foreach (FlipFilter filter in _filters)
        {
            if (!filter.CheckPass(itemData))
                return false;
        }

        return true;
    }
}