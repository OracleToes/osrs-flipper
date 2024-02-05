using OsrsFlipper.Data.Mapping;
using RestSharp;

namespace OsrsFlipper.Api;

public class MappingApi : Api<List<ItemData>>
{
    private readonly RestRequest _request;
    
    
    public MappingApi()
    {
        _request = new RestRequest("mapping");
    }


    public async Task<ItemMapping?> GetMapping(RestClient client)
    {
        List<ItemData>? result = await ExecuteRequest(client, _request);
        return result != null ? new ItemMapping(result) : null;
    }
}