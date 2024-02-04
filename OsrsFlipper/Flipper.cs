using OsrsFlipper.Api;
using OsrsFlipper.Data.Mapping;

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
        {
            Console.WriteLine($"Loaded {mapping.Count} items");
            foreach (ItemData itemData in mapping.GetAllItemData())
            {
                Console.WriteLine($"Item: {itemData.Id} - {itemData.Name}");
            }
        }
        else
        {
            Console.WriteLine("Failed to load item mappings");
        }
    }


    public void Dispose()
    {
        _api.Dispose();
    }
}