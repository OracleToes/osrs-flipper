using OsrsFlipper.Caching;
using OsrsFlipper.Data.TimeSeries;

namespace OsrsFlipper.Filtering;

internal class FilterCollection
{
    private readonly List<PruneFilter> _pruneFilters = new();
    private readonly List<FlipFilter> _flipFilters = new();


    public FilterCollection AddPruneFilter(PruneFilter filter)
    {
        _pruneFilters.Add(filter);
        return this;
    }
    
    
    public FilterCollection AddFlipFilter(FlipFilter filter)
    {
        _flipFilters.Add(filter);
        return this;
    }
    
    
    public void InitializeFilters()
    {
        foreach (PruneFilter filter in _pruneFilters)
            filter.Initialize();
        foreach (FlipFilter filter in _flipFilters)
            filter.Initialize();
    }
    
    
    public bool PassesPruneTest(CacheEntry itemData)
    {
        foreach (PruneFilter filter in _pruneFilters)
        {
            if (!filter.CheckPassFilter(itemData))
                return false;
        }

        return true;
    }
    
    
    public bool PassesFlipTest(CacheEntry itemData, ItemPriceHistory history)
    {
        foreach (FlipFilter filter in _flipFilters)
        {
            if (!filter.CheckPassFilter(itemData, history))
                return false;
        }

        return true;
    }


    public void DebugFilters()
    {
        foreach (PruneFilter filter in _pruneFilters)
        {
            Logger.Verbose($"Prune filter {filter.GetType().Name} block count: {filter.ItemsFailed} / {filter.ItemsChecked}");
        }

        foreach (FlipFilter filter in _flipFilters)
        {
            Logger.Verbose($"Flip filter {filter.GetType().Name} block count: {filter.ItemsFailed} / {filter.ItemsChecked}");
        }
    }
}