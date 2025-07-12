using Hawf.Client;
using Hawf.Client.Configuration;

namespace Hawf.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ApiClientAttribute<TApi> : Attribute where TApi : ApiRequestBuilder<TApi>
{
    public ApiClientConfiguration<TApi> ClientConfig { get; }

    public ApiClientAttribute(
        string baseUrl="", 
        string userAgent="Hawf", 
        int rateLimitMaxRequests=5, 
        int rateLimitTimespan=60000, 
        bool useRateLimit=false,
        bool throwOnFail=true
        )
    {
        ClientConfig = new ApiClientConfiguration<TApi>
        {
            BaseUrl = baseUrl,
            DefaultUserAgent = userAgent,
            RateLimitMaxRequests = rateLimitMaxRequests,
            RateLimitTimespan = TimeSpan.FromMilliseconds(rateLimitTimespan),
            UseRateLimit = useRateLimit,
            DefaultThrowOnFail = throwOnFail
        };
    }
}