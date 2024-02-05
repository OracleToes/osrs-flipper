using OsrsFlipper.Data.Price.Latest;
using RestSharp;

namespace OsrsFlipper.Api;

public class LatestPriceApi : Api<ItemLatestPriceDataCollection>
{
    private readonly RestRequest _requestLatest;
    
    
    public LatestPriceApi()
    {
        _requestLatest = new RestRequest("latest");
    }


    public async Task<ItemLatestPriceDataCollection?> GetLatest(RestClient client) => await ExecuteRequest(client, _requestLatest);
}