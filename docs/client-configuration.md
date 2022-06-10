# Client Configuration

Both the API and the internal HTTP client can be configured to serve your specific needs. There are two main ways to configure the API client. The first is through the `ApiClient` attribute, and the second is calling the `Configure` base class method.

## `ApiClient` Attribute

The `ApiClient` attribute is a mandatory annotation to all classes that implements an API client. For example:

```cs
[ApiClient("https://api.my-website.com")]
class MyApiClient : BaseApi<MyApiClient>
{
  // ... methods
}
```

It requires you to provide a base address, but it also contains some other optional properties:

- **`baseUrl`**
  
  The base address for all API calls in this client. *Required*
  
- **`userAgent`**

  Default value for the User-Agent header. *Default: `Hawf`*
  
- **`rateLimitMaxRequests`**
  
  If rate limit is enabled, maximum requests that can happen within the `rateLimitTimespan` timespan (See [Rate Limiting](rate-limiting.md) for more information) *Default: `5`*
  
- **`rateLimitTimespan`**

  If rate limit is enabled, the timespan (in milliseconds) in which `rateLimitMaxRequests` applies (See [Rate Limiting](rate-limiting.md) for more information) *Default: `60000`*

- **`useRateLimit`**

  Whether to enable rate limit or not. *Default: false*

- **`throwOnFail`**

  If the HTTP server returns a non-success HTTP status code, an exception is raised. *Default: true*
  
## The `Configure()` base class method

This method gives you much more control over the internal configuration of the API client. Here you can configure things like caching, the default cancellation token, the HTTP handler itself etc.

The method is provides an Action<ApiClientConfiguration> parameter, which you can pass a lambda expression to configure the internal config object. Example:

```cs
Configure(options => {
  options.DefaultUserAgent = "My User Agent";
  options.CacheResponse = true;
});
```
