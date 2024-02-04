using OsrsFlipper.Data.Mapping;
using OsrsFlipper.Data.Price;
using OsrsFlipper.Data.TimeSeries;
using RestSharp;

namespace OsrsFlipper.Api;

internal class OsrsApi : IDisposable
{
    private const string USER_AGENT = "OSRS flipping tool - @Japsuu on Discord";
    
    private readonly RestClient _client;
    private readonly PriceApi _priceApi;
    private readonly MappingApi _mappingApi;
    private readonly TimeSeriesApi _timeSeriesApi;


    public OsrsApi()
    {
        RestClientOptions options = new("https://prices.runescape.wiki/api/v1/osrs")
        {
            UserAgent = USER_AGENT
        };
        _client = new RestClient(options);
        
        _priceApi = new PriceApi();
        _mappingApi = new MappingApi();
        _timeSeriesApi = new TimeSeriesApi();
    }
    
    
    public async Task<ItemPriceDataCollection?> GetLatestPrices() => await _priceApi.GetLatest(_client);
    public async Task<ItemPriceDataCollection?> Get5MinAveragePrices() => await _priceApi.Get5MinAverage(_client);
    public async Task<ItemPriceDataCollection?> Get1HourAveragePrices() => await _priceApi.Get1HourAverage(_client);
    public async Task<ItemMapping?> GetItemMapping() => await _mappingApi.GetMapping(_client);
    public async Task<ItemPriceHistory?> GetPriceHistory(ItemData item, TimeSeriesApi.TimeSeriesTimeStep timestep) => await _timeSeriesApi.GetPriceHistory(_client, item, timestep);


    public void Dispose()
    {
        _client.Dispose();
    }
}