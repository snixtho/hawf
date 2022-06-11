# Response Caching

A caching mechanism is provided for requests. The way it works is that it keeps a copy of the `HttpResponseMessage` instace made by the first request. The objects are cached based on the matching URLs.

This means that a request to `https://api.my-website.com/users/1` will be cached, and the cached object will be returned for the same url next time it is called. A request to `https://api.my-website.com/users/2` will not return the cached object for `https://api.my-website.com/users/1`.

## Configuration
You can enable caching for all requests globally and also the time for which a response is cached. This can be configured with the `Configure()` method:

```cs
Configure(options => {
    // enable caching for all requests
    options.CacheResponse = true;

    // default amount of time to cache a response
    options.DefaultCacheTime = TimeSpan.FromMinutes(1);
});
```

## Caching per individual request
The builder API provides methods for specifying whether an individual request should cache the response and for how long. You can use the `CacheResponseFor` method for this.

Example:
```cs
CacheResponseFor(TimeSpan.FromSeconds(10))
.GetJsonAsync<User>("/users/{id}", 1);
```

In this example, the response for the request `/users/1` will be cached for 10 seconds.