using System.Text.Json;

namespace Hawf.Client.Exceptions;

public class HawfResponseException : HttpRequestException
{
    public HttpResponseMessage Response { get; }
    
    public HawfResponseException(HttpResponseMessage response) : base(
        $"Request failed with response: {response.StatusCode}", null,
        response.StatusCode)
    {
        Response = response;
    }

    public async Task<TResponse?> DeserializeResponse<TResponse>(CancellationToken cancelToken = default)
    {
        var responseText = await Response.Content.ReadAsStringAsync(cancelToken);
        return JsonSerializer.Deserialize<TResponse>(responseText, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}