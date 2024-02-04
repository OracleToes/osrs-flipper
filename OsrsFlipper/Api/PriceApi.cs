using OsrsFlipper.Data.Price;
using RestSharp;

namespace OsrsFlipper.Api;

public class PriceApi
{
    private readonly RestRequest _requestLatest;
    private readonly RestRequest _request5Min;
    private readonly RestRequest _request1Hour;
    
    
    public PriceApi()
    {
        _requestLatest = new RestRequest("latest");
        _request5Min = new RestRequest("5m");
        _request1Hour = new RestRequest("1h");
    }


    public async Task<ItemPriceDataCollection?> GetLatest(RestClient client) => await ProcessRequest(client, _requestLatest);
    public async Task<ItemPriceDataCollection?> Get5MinAverage(RestClient client) => await ProcessRequest(client, _request5Min);
    public async Task<ItemPriceDataCollection?> Get1HourAverage(RestClient client) => await ProcessRequest(client, _request1Hour);


    private static async Task<ItemPriceDataCollection?> ProcessRequest(RestClient client, RestRequest request)
    {
        RestResponse<ItemPriceDataCollection> response = await client.ExecuteAsync<ItemPriceDataCollection>(request);
        if (response.IsSuccessful)
        {
            if (response.Data != null)
                return response.Data;

            Logger.Warn($"Failed to deserialize {request.Resource} prices response");
        }
        else
        {
            Logger.Error($"Failed to get {request.Resource} prices: {response.ErrorMessage}");
        }
        
        return null;
    }
}