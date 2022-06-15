# Extending the API
Most of the time, extending the API is as simple as creating some subclasses of the main `ApiBase` class.

## Inheriting `ApiBase`
To keep access to the protected methods of the builder API and `ApiBase` you need to pass the generic type of the inherited class in a different way.

For example:
```cs
public class MyExtendedApiBase<T> : ApiBase<T> where T : MyExtendedApiBase<T> 
{
    // ...
}
```

You can then use your newly created `MyExtendedApiBase` like you would with `ApiBase` and have access to all the methods:
```cs
public class MyApi : MyExtendedApiBase<MyApi>
{
    // ...
}
```

## Extending the Builder
The same method exists for extending the builder. Each method in the builder returns the class again. So always return the generic type when creating a builder method.

For example:
```cs
public class ExtendedBuilderApiBase<T> : ApiBase<T> where T : ExtendedBuilderApiBase<T>
{
    protected T WithMyCustomAuth(string token) =>
        WithHeader(HttpHeader.Authorization, $"myauth token={token}");
}
```

### Editing `RequestInfo` directly
All information about the current request lies in the property called `ApiRequest`. Most of the time, you should call the base builder methods. But if you for some reason need to edit `ApiRequest`, you need to make follow a certain pattern.

Every time you are about to do a modification to `ApiRequest` directly, always call `EnsureNewRequest()` first. This method makes sure all default values are set if we are about to create a new request.

The `BuilderRequestFinished()` tells the builder that we are done creating the request. It is automatically called when a request has been performed.

An example can be from the `WithHeader` method:
```cs
protected T WithHeader(string name, string? value)
{
    // make sure we have defaults set up incase of a new request
    EnsureNewRequest();

    if (value == null)
        return (T) this;

    RequestInfo.Headers.Add(name, value);
    return (T) this;
}
```

---

Previous: [Keeping Instance State](keeping-state.md)
