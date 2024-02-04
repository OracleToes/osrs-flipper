namespace OsrsFlipper.Data.Mapping;

public class ItemMapping
{
    private readonly Dictionary<int, ItemData> _data;
    
    public int Count => _data.Count;


    public ItemMapping(IEnumerable<ItemData> dataList)
    {
        _data = dataList.ToDictionary(data => data.Id);
    }
    
    
    public bool TryGetItemData(int itemId, out ItemData? itemData) => _data.TryGetValue(itemId, out itemData);
    
    public IEnumerable<ItemData> GetAllItemData() => _data.Values;
}