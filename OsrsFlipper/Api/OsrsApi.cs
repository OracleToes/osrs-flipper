using OsrsFlipper.Data.Mapping;
using OsrsFlipper.Data.Price.Average;
using OsrsFlipper.Data.Price.Latest;
using OsrsFlipper.Data.TimeSeries;
using RestSharp;

namespace OsrsFlipper.Api;

internal class OsrsApi : IDisposable
{
    private const string USER_AGENT = "OSRS flipping tool - @Japsuu on Discord";
    
    private readonly RestClient _client;
    private readonly LatestPriceApi _latestPriceApi;
    private readonly AveragePriceApi _averagePriceApi;
    private readonly MappingApi _mappingApi;
    private readonly TimeSeriesApi _timeSeriesApi;


    public OsrsApi()
    {
        RestClientOptions options = new("https://prices.runescape.wiki/api/v1/osrs")
        {
            UserAgent = USER_AGENT
        };
        _client = new RestClient(options);
        
        _latestPriceApi = new LatestPriceApi();
        _averagePriceApi = new AveragePriceApi();
        _mappingApi = new MappingApi();
        _timeSeriesApi = new TimeSeriesApi();
    }
    
    
    public async Task<ItemLatestPriceDataCollection?> GetLatestPrices() => await _latestPriceApi.GetLatest(_client);
    public async Task<ItemAveragePriceDataCollection?> Get5MinAveragePrices() => await _averagePriceApi.Get5MinAverage(_client);
    public async Task<ItemAveragePriceDataCollection?> Get1HourAveragePrices() => await _averagePriceApi.Get1HourAverage(_client);
    public async Task<ItemMapping?> GetItemMapping() => await _mappingApi.GetMapping(_client);
    public async Task<ItemPriceHistory?> GetPriceHistory(ItemData item, TimeSeriesApi.TimeSeriesTimeStep timestep) => await _timeSeriesApi.GetPriceHistory(_client, item, timestep);


    public void Dispose()
    {
        _client.Dispose();
    }
}