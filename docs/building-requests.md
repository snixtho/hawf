# Building API Requests
The `ApiBase` class inherits the `ApiRequestBuilder` class that provides many convenient builder methods to create the exact API request you need for one of your endpoints.

Whenever you call one of these methods, the internal state changes, indicating that a new request is being built. To end a request and call the endpoint, call any of the [request action methods](calling-endpoints.md).

The builder methods provides various ways to configure the request itself, which allows you to control exactly how you want the request to be done.

# Setting HTTP Headers
You can set HTTP headers with the `WithHeader` method. For example:

```cs
WithHeader("MyHeader", "My value")
.WithHeader("MyHeader2", 1234)
.WithHeader("MyHeader3", null) // null values are ignored
// ...
```

If a value passed to one of these methods are null. The header will not be set. Several of the other builder method also have this functionality. This allows you to easily provide parameters that can be omitted and not included in the request in a simple way.

Some convenience methods are also provided for the `WithHeader` method as well.

## User Agent
The user agent can be easily set with its own method. For example:

```cs
WithUserAgent("My user Agent");
```

# Query Parameters
The query parameters at the end of an URL can be set using the `WithQueryParam` method. This method adds the query parameters to an internal dictionary, and is automatically generated when the request action is performed. Values will be URL encoded automatically so you don't have to worry about this.

It is always recommended to use this method everytime you need to add an URL query value and keep the path in the request action method clean.

Example:
```cs
WithQueryParam("name", "value)
.WithQueryParam("intParam", 12345); // auto converted to string
```

## The `WithQueryOptions` method
There is actually another and more advanced way of providing query parameters to the URL. The `WithQueryOptions` provides a declerative way of defining URL parameters in a class, and then configure them through a lambda expression. This is very convenient if you have alot of query parameters or needs some more advanced parsing and value conversions.

### `QueryProperty` Attribute
The `QueryProperty` allows you to control how a query parameter is generated.

Imagine you have the following class, which defines a query parameter:
```cs
public class MyQuery
{
    public string UserId { get; set; }
}
```

The name of the parameter is autoamtically converted to camelCase if nothing else is specified otherwise. But imagine in this scenario that the actual query parameter is `user_id`. You can annotate the `QueryProperty` attribute to configure the custom name:
```cs
public class MyQuery
{
    [QueryProperty("user_id")]
    public string UserId { get; set; }
}
```

You can now add the query parameter to the requeste by calling the `WithQueryOptions` method in two ways, either pass an instance or use the lambda configurator:
```cs
// passing an instance
WithQueryOptions(new MyQuery{ UserId = "some value" })

// lambda config
WithQueryOptions(query => query.userId = "some value");
```

The final query will now result in: `?user_id=some%20value`

Here are the options provided that controls how it is generated:
- `name`: If not null, use this as the name during generation of the query. *Default: `null`*
- `integerEnum`: If true, convert an enumeration type to an integer value. *Default: `false`*
- `listSeparator`: If an array or collection is defined as a query parameter, use this string to separate the items. *Default: `","`*

# Response Caching
You can enable caching of the response for a specific request by using the `CacheResponseFor` method. For example:

```cs
CacheResponseFor(TimeSpan.FromSeconds(10))
```

For more information about response caching check out the [Response Caching](response-caching.md) pages.
