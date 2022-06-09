using Hawf.Client.Http;

namespace Hawf.Client;

public class ApiRequest
{
    public Dictionary<string, string> Headers { get; set; }
    public QueryParamsCollection Query { get; set; }
    public List<object> PathValues { get; set; }
    public string Path { get; set; }
    public HttpMethod Method { get; set; }
    public Uri BaseUrl { get; set; }
    public CancellationToken CancelToken { get; set; }
    public bool KeepAlive { get; set; } = true;
}
