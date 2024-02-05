using OsrsFlipper.Data.Price.Average;
using RestSharp;

namespace OsrsFlipper.Api;

public class AveragePriceApi : Api<ItemAveragePriceDataCollection>
{
    private readonly RestRequest _request5Min;
    private readonly RestRequest _request1Hour;
    
    
    public AveragePriceApi()
    {
        _request5Min = new RestRequest("5m");
        _request1Hour = new RestRequest("1h");
    }


    public async Task<ItemAveragePriceDataCollection?> Get5MinAverage(RestClient client) => await ExecuteRequest(client, _request5Min);
    public async Task<ItemAveragePriceDataCollection?> Get1HourAverage(RestClient client) => await ExecuteRequest(client, _request1Hour);
}