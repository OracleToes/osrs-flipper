using OsrsFlipper.Data.Mapping;
using OsrsFlipper.Data.TimeSeries;
using RestSharp;

namespace OsrsFlipper.Api;

public class TimeSeriesApi
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
        
        RestResponse<ItemPriceHistory> response = await client.ExecuteAsync<ItemPriceHistory>(_request);
        if (response.IsSuccessful)
        {
            if (response.Data != null)
            {
                return response.Data;
            }

            Logger.Warn("Failed to deserialize item price history response");
        }
        else
        {
            Logger.Error($"Failed to get item price history: {response.ErrorMessage}");
        }
        
        return null;
    }
}