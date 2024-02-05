using OsrsFlipper.Api;
using OsrsFlipper.Data.Mapping;
using OsrsFlipper.Data.Price.Average;
using OsrsFlipper.Data.Price.Latest;
using OsrsFlipper.Data.TimeSeries;

namespace OsrsFlipper;

public sealed class Flipper : IDisposable
{
    private readonly OsrsApi _api;


    public Flipper()
    {
        _api = new OsrsApi();
    }


    public async Task Test()
    {
        ItemMapping? mapping = await _api.GetItemMapping();
        if (mapping != null)
            Console.WriteLine($"Loaded {mapping.Count} items");
        else
            throw new Exception("Failed to load item mapping");
        
        ItemLatestPriceDataCollection? latestPrices = await _api.GetLatestPrices();
        if (latestPrices != null)
            Console.WriteLine($"Loaded {latestPrices.Data.Count} latest prices");
        else
            throw new Exception("Failed to load latest prices");
        
        ItemAveragePriceDataCollection? average5MinPrices = await _api.Get5MinAveragePrices();
        if (average5MinPrices != null)
            Console.WriteLine($"Loaded {average5MinPrices.Data.Count} 5 minute average prices");
        else
            throw new Exception("Failed to load 5 minute average prices");
        
        ItemAveragePriceDataCollection? average1HourPrices = await _api.Get1HourAveragePrices();
        if (average1HourPrices != null)
            Console.WriteLine($"Loaded {average1HourPrices.Data.Count} 1 hour average prices");
        else
            throw new Exception("Failed to load 1 hour average prices");
        
        if(!mapping.TryGetItemData(2, out ItemData item))
            throw new Exception("Failed to get item data");
        
        ItemPriceHistory? priceHistory = await _api.GetPriceHistory(item, TimeSeriesApi.TimeSeriesTimeStep.Day);
        if (priceHistory != null)
            Console.WriteLine($"Loaded {priceHistory.Data.Count} price history entries for {item.Name}");
    }


    public void Dispose()
    {
        _api.Dispose();
    }
}