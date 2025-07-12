using Hawf.Client;
using Hawf.Client.Configuration;

namespace Hawf.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ApiClientAttribute : Attribute
{
    public string BaseUrl { get; }
    public string DefaultUserAgent { get; }
    public int RateLimitMaxRequests { get; }
    public TimeSpan RateLimitTimespan { get; }
    public bool UseRateLimit { get; }
    public bool DefaultThrowOnFail { get; }

    public ApiClientAttribute(
        string baseUrl="", 
        string userAgent="Hawf", 
        int rateLimitMaxRequests=5, 
        int rateLimitTimespan=60000, 
        bool useRateLimit=false,
        bool throwOnFail=true
        )
    {
        BaseUrl = baseUrl;
        DefaultUserAgent = userAgent;
        RateLimitMaxRequests = rateLimitMaxRequests;
        RateLimitTimespan = TimeSpan.FromMilliseconds(rateLimitTimespan);
        UseRateLimit = useRateLimit;
        DefaultThrowOnFail = throwOnFail;
    }
}