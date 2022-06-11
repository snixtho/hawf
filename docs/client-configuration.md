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

  Whether to enable rate limit or not. *Default: `false`*

- **`throwOnFail`**

  If the HTTP server returns a non-success HTTP status code, an exception is raised. *Default: `true`*

## The `Configure()` base class method

This method gives you much more control over the internal configuration of the API client. Here you can configure things like caching, the default cancellation token, the HTTP handler itself etc. The properties in the `ApiClient` attribute is automatically assigned to this internal configuration object.

The method is provides an Action<ApiClientConfiguration> parameter, which you can pass a lambda expression to configure the internal config object. Example:

```cs
Configure(options => {
  options.DefaultUserAgent = "My User Agent";
  options.CacheResponse = true;
});
```

This method is commonly called within the contructor of the API client class. Here is a list of options it provides:

- **`BaseUrl`**

  Baseurl all endpoints will be called from.

- **`DefaultUserAgent`**

  The default user agent to send with the HTTP request, unless specified otherwise. *Default: `Hawf`*

- **`RateLimitMaxRequests`**

  Max amount of requests before rate limit kicks in. *Default: `5`*

- **`RateLimitTimespan`**

  Timespan for max amount of requests before rate limit kicks in. *Default: `1 Minute`*

- **`UseRateLimit`**

  Whether to enable rate limit by. *Default: `false`*

- **`WaitForRateLimit`**

  If true, the code blocks and wait for the rate limit time is over. Otherwise it throws an RateLimitExceededException. *Default: `true`*

- **`KeepAlive`**

  Whether to attempt to keep the connection alive. *Default: `true`*

- **`DefaultThrowOnFail`**

  Throw an error by default if the http server returns a non 200 status code. *Default: `true`*

- **`CacheResponse`**

  Whether to always cache responses (See [Response Caching](response-caching.md) for more information) *Default: `false`*

- **`DefaultCacheTime`**

  The default time span to keep a cache of responses (See [Response Caching](response-caching.md) for more information) *Default: `false`*

- **`HttpHandler`**

  The HTTP message handler to use in requests. *Default: `default instance of HttpClientHandler`*

---
Previous: [Getting Started](getting-started.md) | Next: [Calling API Endpoints](calling-endpoints.md)