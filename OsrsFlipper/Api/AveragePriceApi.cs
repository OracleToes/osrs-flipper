using OsrsFlipper.Data.Price.Average;
using RestSharp;

namespace OsrsFlipper.Api;

public class AveragePriceApi : OsrsApi<ItemAveragePriceDataCollection>
{
    private readonly RestRequest _request5Min;
    private readonly RestRequest _request10Min;
    private readonly RestRequest _request30Min;
    private readonly RestRequest _request1Hour;
    private readonly RestRequest _request6Hour;
    private readonly RestRequest _request24Hour;
    
    
    public AveragePriceApi()
    {
        _request5Min = new RestRequest("5m");
        _request10Min = new RestRequest("10m");
        _request30Min = new RestRequest("30m");
        _request1Hour = new RestRequest("1h");
        _request6Hour = new RestRequest("6h");
        _request24Hour = new RestRequest("24h");
    }


    /// <summary>
    /// Gets the average prices averaged over a 5-minute period.
    /// </summary>
    /// <param name="client">The client to use for the request</param>
    /// <param name="timestamp">Represents the beginning of the 5-minute period being averaged. Must be divisible by 300 (5 minutes).</param>
    public async Task<ItemAveragePriceDataCollection?> Get5MinAverage(RestClient client, DateTime timestamp) => await GetAverage(client, _request5Min, timestamp);


    /// <summary>
    /// Gets the average prices averaged over a 10-minute period.
    /// </summary>
    /// <param name="client">The client to use for the request</param>
    /// <param name="timestamp">Represents the beginning of the 10-minute period being averaged. Must be divisible by 600 (10 minutes).</param>
    public async Task<ItemAveragePriceDataCollection?> Get10MinAverage(RestClient client, DateTime timestamp) => await GetAverage(client, _request10Min, timestamp);


    /// <summary>
    /// Gets the average prices averaged over a 30-minute period.
    /// </summary>
    /// <param name="client">The client to use for the request</param>
    /// <param name="timestamp">Represents the beginning of the 30-minute period being averaged. Must be divisible by 1800 (30 minutes).</param>
    public async Task<ItemAveragePriceDataCollection?> Get30MinAverage(RestClient client, DateTime timestamp) => await GetAverage(client, _request30Min, timestamp);

    /// <summary>
    /// Gets the average prices averaged over a 1-hour period.
    /// </summary>
    /// <param name="client">The client to use for the request</param>
    /// <param name="timestamp">Represents the beginning of the 1-hour period being averaged. Must be divisible by 3600 (1h).</param>
    public async Task<ItemAveragePriceDataCollection?> Get1HourAverage(RestClient client, DateTime timestamp) => await GetAverage(client, _request1Hour, timestamp);

    /// <summary>
    /// Gets the average prices averaged over a 6-hour period.
    /// </summary>
    /// <param name="client">The client to use for the request</param>
    /// <param name="timestamp">Represents the beginning of the 6-hour period being averaged. Must be divisible by 21600 (6h).</param>
    public async Task<ItemAveragePriceDataCollection?> Get6HourAverage(RestClient client, DateTime timestamp) => await GetAverage(client, _request6Hour, timestamp);

    /// <summary>
    /// Gets the average prices averaged over a 24-hour period.
    /// </summary>
    /// <param name="client">The client to use for the request</param>
    /// <param name="timestamp">Represents the beginning of the 24-hour period being averaged. Must be divisible by 86400 (24h).</param>
    public async Task<ItemAveragePriceDataCollection?> Get24HourAverage(RestClient client, DateTime timestamp) => await GetAverage(client, _request24Hour, timestamp);


    private async Task<ItemAveragePriceDataCollection?> GetAverage(RestClient client, RestRequest request, DateTime timestamp)
    {
        // Add the unix timestamp to the request
        if (timestamp != default)
            request.AddOrUpdateParameter("timestamp", Utils.DateTimeToUnixTime(timestamp).ToString());
        
        return await ExecuteRequest(client, request);
    }
}