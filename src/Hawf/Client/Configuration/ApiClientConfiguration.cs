using System.Text.Json;

namespace Hawf.Client.Configuration;

public class ApiClientConfiguration<T> where T : ApiRequestBuilder<T>
{
    /// <summary>
    /// Baseurl all endpoints will be called from.
    /// </summary>
    public string BaseUrl { get; set; }
    /// <summary>
    /// The default user agent to send with the HTTP request
    /// unless specified otherwise.
    /// </summary>
    public string DefaultUserAgent { get; set; } = "Hawf";
    /// <summary>
    /// Max amount of requests before rate limit kicks in.
    /// </summary>
    public int RateLimitMaxRequests { get; set; } = 5;
    /// <summary>
    /// Timespan for max amount of requests before rate limit kicks in.
    /// </summary>
    public TimeSpan RateLimitTimespan { get; set; } = TimeSpan.FromMinutes(1);
    /// <summary>
    /// Whether to enable rate limit by.
    /// </summary>
    public bool UseRateLimit { get; set; } = false;
    /// <summary>
    /// If true, the code blocks and wait for the rate limit time is over.
    /// Otherwise it throws an RateLimitExceededException.
    /// </summary>
    public bool WaitForRateLimit { get; set; } = false;
    /// <summary>
    /// Whether to attempt to keep the connection alive.
    /// </summary>
    public bool KeepAlive { get; set; } = true;
    /// <summary>
    /// Throw an error by default if the http server returns a non 200 status code.
    /// </summary>
    public bool DefaultThrowOnFail { get; set; } = true;
    /// <summary>
    /// Whether to always cache responses.
    /// </summary>
    public bool CacheResponse { get; set; } = false;
    /// <summary>
    /// The default time span to keep a cache of responses.
    /// </summary>
    public TimeSpan DefaultCacheTime { get; set; } = TimeSpan.FromMinutes(1);
    /// <summary>
    /// The HTTP message handler to use in requests.
    /// </summary>
    public HttpMessageHandler HttpHandler { get; set; } = new HttpClientHandler();

    /// <summary>
    /// Options to pass to the JSON serializer.
    /// </summary>
    public JsonSerializerOptions SerializerOptions { get; set; } = new()
    {
        PropertyNameCaseInsensitive = true
    };
    /// <summary>
    /// Initial configuration of all requests.
    /// </summary>
    public ApiRequestBuilder<T>? BaseRequest { get; set; }
}
