# Rate Limiting

The rate limiting tools can be used to prevent rate limit functionality to be triggered on the server side. If the rate limit is triggered, you can configure it to either throw and exception or wait to do the request until the rate limit time is over. By default it will throw a `RateLimitExceededException`.

Rate limiting works on all requests within the same client class.

## Configuring Rate Limiting
To use rate limiting, it must first be enabled. You can either do this in the `ApiClient` attribute or in the `Configure` method.

### With the `ApiClient` attribute
To enable, set `useRateLimit` to `true`:
```cs
[ApiClient("https://api.my-website.com", useRateLimit: true)]
class MyClient : ApiBase<MyClient>
{
}
```

You can configure the behavior with the two options:
- `rateLimitMaxRequests`: Max amount of requests before rate limit kicks in.
- `rateLimitTimespan`: Timespan for max amount of requests before rate limit kicks in.

### With `Configure()`:
Set `UseRateLimit` to `true`. It also provides some more properties to configure the behavior:

```cs
Configure(options => {
    // enable rate limiting
    options.UseRateLimit = true;

    // Max amount of requests before rate limit kicks in.
    options.RateLimitMaxRequests = 5;

    // Timespan for max amount of requests before rate limit kicks in.
    options.RateLimitTimespan = TimeSpan.FromMinutes(1);

    // If true, the code blocks and wait for the rate limit time is over.
    // Otherwise it throws an RateLimitExceededException.
    options.WaitForRateLimit = false;
});
```

## RateLimitExceededException
This is thrown when the rate limit is triggered and it contains a property called `TimeLeft` which indicates how much time left before the rate limit can be released.

---

Previous: [Query Builder](query-builder.md) | Next: [Response Caching](response-caching.md)