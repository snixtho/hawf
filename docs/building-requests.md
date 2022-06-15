# Building API Requests
The `ApiBase` class inherits the `ApiRequestBuilder` class that provides many convenient builder methods to create the exact API request you need for one of your endpoints.

Whenever you call one of these methods, the internal state changes, indicating that a new request is being built. To end a request and call the endpoint, call any of the [request action methods](calling-endpoints.md).

The builder methods provides various ways to configure the request itself, which allows you to control exactly how you want the request to be done.

## Setting HTTP Headers
You can set HTTP headers with the `WithHeader` method. For example:

```cs
WithHeader("MyHeader", "My value")
.WithHeader("MyHeader2", 1234)
.WithHeader("MyHeader3", null) // null values are ignored
// ...
```

If a value passed to one of these methods are null. The header will not be set. Several of the other builder method also have this functionality. This allows you to easily provide parameters that can be omitted and not included in the request in a simple way.

Some convenience methods are also provided for the `WithHeader` method as well.

### User Agent
The user agent can be easily set with its own method. For example:

```cs
WithUserAgent("My user Agent");
```

## Query Parameters
Query parameters can be automatically generated and encoded. To do this you can use the `WithQueryParam` method:
```cs
WithQueryParam("name", "value")
.WithQueryParam("intParam", 12345); // auto converted to string
```

The framework also provides a more advanced way of building query parameters, check out the [Query Builder](query-builder.md) docs for more information.

## Response Caching
You can enable caching of the response for a specific request by using the `CacheResponseFor` method. For example:

```cs
CacheResponseFor(TimeSpan.FromSeconds(10))
```

For more information about response caching check out the [Response Caching](response-caching.md) pages.

## Authorization
Two methods are currently provided for authorization of API endpoints.

### Bearer Tokens
You can use the `WithBearerToken` to authorize with access tokens in the bearer format:
```cs
WithBearerToken("myToken")
```

### Basic Auth
HTTP basic auth is supported out of the box with `WithBasicAuth`. It will automatically encode the passed parameters in the correct format and set the header.
```cs
WithBasicAuth("username", "password")
```

## Cancellation token
You can provide the user with a cancellation token that they use in their programs and can be used to stop current requests if needed. The method `WithCancelToken` is provided for this:
```cs
WithCancelToken(myCancellationToken)
```

## Manually Building the full requests
The methods above doesn't build the full requests. In fact, there are more methods available, but they are automatically called whenever you perform a request. It is possible, however, if you for some reason need the extra control, to use the main request methods. These methods require you to set things like the path, base URL, path values and the HTTP method before calling.

Here is a list of the main request methods available, they will perform the actual request and respond with a the response format chosen:
- `RequestStringAsync(CancellationToken cancelToken = default)`
- `RequestStreamAsync(CancellationToken cancelToken = default)`
- `RequestBytesAsync(CancellationToken cancelToken = default)`
- `GetJsonAsync<TReturn>(CancellationToken cancelToken = default)`

Before calling any of these methods, you need to set the following properties:

### Base URL
The base URL can be set with the `WithBaseUrl` method:
```cs
// example
WithBaseUrl("https://api.my-website.com")
```

### HTTP Method
Provide the HTTP method to use in the request with `WithMethod`:
```cs
// example
WithMethod(HttpMethod.Get)
```

### (Optional) WithPath
Most likely you need to set a path to the API endpoint, however, if this is not needed, you can omit this method.
```cs
// example
WithPath("/users/{id}")
```

### (Optional) WithPathValues
If you don't have any path parameters set, this method is optional. However, Keep in mind that the number of path parameters provided must have the same amount of values added.
```cs
// example
WithPathValues(1)
```

---
Previous: [Calling API Endpoints](calling-endpoints.md) | Next: [Query Builder](query-builder.md)