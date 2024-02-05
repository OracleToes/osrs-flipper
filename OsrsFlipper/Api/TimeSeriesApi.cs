using OsrsFlipper.Data.Mapping;
using OsrsFlipper.Data.TimeSeries;
using RestSharp;

namespace OsrsFlipper.Api;

public class TimeSeriesApi : OsrsApi<ItemPriceHistory>
{
    public enum TimeSeriesTimeStep
    {
        FiveMinutes,
        Hour,
        SixHours,
        Day
    }
    private readonly RestRequest _request;
    
    
    public TimeSeriesApi()
    {
        _request = new RestRequest("timeseries");
    }


    public async Task<ItemPriceHistory?> GetPriceHistory(RestClient client, ItemData item, TimeSeriesTimeStep timestep)
    {
        _request.AddQueryParameter("id", item.Id.ToString());
        _request.AddQueryParameter("timestep", timestep.AsString());
        
        return await ExecuteRequest(client, _request);
    }
}