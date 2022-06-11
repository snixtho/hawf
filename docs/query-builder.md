# Query Builder

The query parameters at the end of an URL can be set using the `WithQueryParam` method. This method adds the query parameters to an internal dictionary, and is automatically generated when the request action is performed. Values will be URL encoded automatically so you don't have to worry about this.

It is always recommended to use this method everytime you need to add an URL query value and keep the path in the request action method clean.

Example:
```cs
WithQueryParam("name", "value")
.WithQueryParam("intParam", 12345); // auto converted to string
```

### The `WithQueryOptions` method
There is actually another and more advanced way of providing query parameters to the URL. The `WithQueryOptions` provides a declerative way of defining URL parameters in a class, and then configure them through a lambda expression. This is very convenient if you have alot of query parameters or needs some more advanced parsing and value conversions.

#### `QueryProperty` Attribute
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

---

Previous: [Building API Requests](building-requests.md) | Next: [Rate Limiting](rate-limiting.md)