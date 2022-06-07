namespace Hawf.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ApiClientAttribute : Attribute
{
    /// <summary>
    /// The base URL of the api.
    /// </summary>
    public string BaseUrl { get; }
    /// <summary>
    /// User agent to use by default for all requests.
    /// </summary>
    public string DefaultUserAgent { get; }
    /// <summary>
    /// Max number of requests within the given timespan for all requests unless specified otherwise.
    /// The default timespan is 1 minute.
    /// </summary>
    public int DefaultRateLimitMaxRequests { get; }
    /// <summary>
    /// Timespan to count max number of requests in.
    /// </summary>
    public TimeSpan DefaultRateLimitTimespan { get; }
    /// <summary>
    /// Whether to limit number of requests within a timespan.
    /// </summary>
    public bool UseRatelimit { get; }
    /// <summary>
    /// Whether to throw on a non-200 response.
    /// </summary>
    public bool ThrowOnFail { get; }

    public ApiClientAttribute(
        string baseUrl, 
        string defaultUserAgent="", 
        int defaultRateLimitMaxRequests=5, 
        int defaultRateLimitTimespan=60000, 
        bool useRateLimit=false,
        bool throwOnFail=true
        )
    {
        BaseUrl = baseUrl;
        DefaultUserAgent = defaultUserAgent;
        DefaultRateLimitMaxRequests = defaultRateLimitMaxRequests;
        DefaultRateLimitTimespan = TimeSpan.FromMilliseconds(defaultRateLimitTimespan);
        UseRatelimit = useRateLimit;
        ThrowOnFail = throwOnFail;
    }
}