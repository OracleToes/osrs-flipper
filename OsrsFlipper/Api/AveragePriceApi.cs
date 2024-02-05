using OsrsFlipper.Data.Price.Average;
using RestSharp;

namespace OsrsFlipper.Api;

public class AveragePriceApi : OsrsApi<ItemAveragePriceDataCollection>
{
    private readonly RestRequest _request5Min;
    private readonly RestRequest _request1Hour;
    private readonly RestRequest _request6Hour;
    private readonly RestRequest _request24Hour;
    
    
    public AveragePriceApi()
    {
        _request5Min = new RestRequest("5m");
        _request1Hour = new RestRequest("1h");
        _request6Hour = new RestRequest("6h");
        _request24Hour = new RestRequest("24h");
    }


    public async Task<ItemAveragePriceDataCollection?> Get5MinAverage(RestClient client) => await ExecuteRequest(client, _request5Min);
    public async Task<ItemAveragePriceDataCollection?> Get1HourAverage(RestClient client) => await ExecuteRequest(client, _request1Hour);
    public async Task<ItemAveragePriceDataCollection?> Get6HourAverage(RestClient client) => await ExecuteRequest(client, _request6Hour);
    public async Task<ItemAveragePriceDataCollection?> Get24HourAverage(RestClient client) => await ExecuteRequest(client, _request24Hour);
}