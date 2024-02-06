using System.Diagnostics;
using OsrsFlipper.Data.Mapping;
using OsrsFlipper.Data.Price.Average;
using OsrsFlipper.Data.Price.Latest;
using OsrsFlipper.Data.TimeSeries;
using RestSharp;

namespace OsrsFlipper.Api;

/// <summary>
/// NOTE: Does not correctly fetch item volumes; see https://runescape.wiki/w/RuneScape:Grand_Exchange_Market_Watch/Usage_and_APIs#A_note_on_volumes.
/// The actual volume data can be fetched from https://oldschool.runescape.wiki/w/Module:GEVolumes/data.json.
/// </summary>
internal class OsrsApiController : IDisposable
{
    private const string USER_AGENT = "OSRS flipping tool - @Japsuu on Discord";

    private readonly RestClient _client;
    private readonly LatestPriceApi _latestPriceApi;
    private readonly AveragePriceApi _averagePriceApi;
    private readonly MappingApi _mappingApi;
    private readonly TimeSeriesApi _timeSeriesApi;


    public OsrsApiController()
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


    public async Task<ItemMapping> GetItemMapping()
    {
        return await _mappingApi.GetMapping(_client) ?? throw new Exception("Failed to load item mapping");
    }


    public async Task<ItemLatestPriceDataCollection?> GetLatestPrices()
    {
        return await _latestPriceApi.GetLatest(_client);
    }

    /// <summary>
    /// Gets the average prices averaged over a 5-minute period.
    /// </summary>
    /// <param name="timestamp">Represents the beginning of the 5-minute period being averaged. Must be divisible by 300 (5 minutes).</param>
    public async Task<ItemAveragePriceDataCollection?> Get5MinAveragePrices(DateTime timestamp = default)
    {
        return await _averagePriceApi.Get5MinAverage(_client, timestamp);
    }

    /// <summary>
    /// Gets the average prices averaged over a 1-hour period.
    /// </summary>
    /// <param name="timestamp">Represents the beginning of the 1-hour period being averaged. Must be divisible by 3600 (1h).</param>
    public async Task<ItemAveragePriceDataCollection?> Get1HourAveragePrices(DateTime timestamp = default)
    {
        return await _averagePriceApi.Get1HourAverage(_client, timestamp);
    }

    /// <summary>
    /// Gets the average prices averaged over a 6-hour period.
    /// </summary>
    /// <param name="timestamp">Represents the beginning of the 6-hour period being averaged. Must be divisible by 21600 (6h).</param>
    public async Task<ItemAveragePriceDataCollection?> Get6HourAveragePrices(DateTime timestamp = default)
    {
        return await _averagePriceApi.Get6HourAverage(_client, timestamp);
    }

    /// <summary>
    /// Gets the average prices averaged over a 24-hour period.
    /// </summary>
    /// <param name="timestamp">Represents the beginning of the 24-hour period being averaged. Must be divisible by 86400 (24h).</param>
    public async Task<ItemAveragePriceDataCollection?> Get24HourAveragePrices(DateTime timestamp = default)
    {
        return await _averagePriceApi.Get24HourAverage(_client, timestamp);
    }


    public async Task<ItemPriceHistory?> GetPriceHistory(ItemData item, TimeSeriesApi.TimeSeriesTimeStep timestep)
    {
        return await _timeSeriesApi.GetPriceHistory(_client, item, timestep);
    }


    public void Dispose()
    {
        _client.Dispose();
    }
}