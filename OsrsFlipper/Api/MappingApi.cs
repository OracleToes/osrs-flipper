using OsrsFlipper.Data.Mapping;
using RestSharp;

namespace OsrsFlipper.Api;

public class MappingApi
{
    private readonly RestRequest _request;
    
    
    public MappingApi()
    {
        _request = new RestRequest("mapping");
    }


    public async Task<ItemMapping?> GetMapping(RestClient client)
    {
        RestResponse<List<ItemData>> response = await client.ExecuteAsync<List<ItemData>>(_request);
        if (response.IsSuccessful)
        {
            if (response.Data != null)
            {
                return new ItemMapping(response.Data);
            }

            Logger.Warn("Failed to deserialize latest item data mapping response");
        }
        else
        {
            Logger.Error($"Failed to get latest item data mapping: {response.ErrorMessage}");
        }
        
        return null;
    }
}