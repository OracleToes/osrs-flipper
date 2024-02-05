using RestSharp;

namespace OsrsFlipper.Api;

public abstract class Api<T>
{
    public async Task<T?> ExecuteRequest(RestClient client, RestRequest request)
    {
        RestResponse<T> response = await client.ExecuteAsync<T>(request);
        if (response.IsSuccessful)
            return response.Data ?? default;

        Logger.Error($"Request was not successful! Error: {response.ErrorMessage}, Request: {request.Resource}, Response: {response.Content}");

        return default;
    }
}