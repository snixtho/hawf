# Code Structure
There are different ways you can structure your API clients. The most straight forward way is putting all API calls inside the same class. You can also have a resource based structure, with properties to an instance of clients for each resource of the API. You can also use inheritance to split the code up in different classes that use common API functions.

## Single API Class
The method is the most simple way and easiest to set up. You just define a class and the methods in it.

For example:
```cs
[ApiClient("https://api.my-website.com")]
class MyApi : ApiBase<MyApi> {
    public Task<string> GetResource1Async() => GetStringAsync("/resource1");
    public Task<string> GetResource2Async() => GetStringAsync("/resource2");
    public Task<string> GetResource3Async() => GetStringAsync("/resource3");
    /// ...
}
```

## Resource Based Approach
When you have many different resources in your API, it might be better to structure these into individual classes for better maintainability and readability of the code. In this case, you can create an API class for each resource and use a "master class" where you can access the individual resources.

For example, let's say you have 3 resources, you create 3 different classes:

*Resource 1*
```cs
[ApiClient("https://api.my-website.com/resource1")]
class MyResource1 : ApiBase<MyResource1> {
    // get resource
    public Task<string> Get(int id) => GetStringAsync("/{id}", values: id);
    // post resource
    public Task Post(Resource1Dto data) => WithJsonBody(data).PostStringAsync();
    /// ...
}
```

*Resource 2*
```cs
[ApiClient("https://api.my-website.com/resource2")]
class MyResource2 : ApiBase<MyResource2> {
    public Task<string> Get() => GetStringAsync();
    /// ...
}
```

*Resource 3*
```cs
[ApiClient("https://api.my-website.com/resource3")]
class MyResource3 : ApiBase<MyResource3> {
    public Task<string> Get() => GetStringAsync();
    /// ...
}
```

You can then create your main API class. Ofcourse you can also have these as static members but that provides less control over the instances. Instead, have them as properties and allow for creating new instances of the API:
```cs
class MyApi {
    public readonly MyResource1 Resource1 { get; }
    public readonly MyResource2 Resource2 { get; }
    public readonly MyResource3 Resource3 { get; }

    public MyApi() {
        Resource1 = new MyResource1();
        Resource2 = new MyResource2();
        Resource3 = new MyResource3();
    }
}
```

The usage of this would then be:
```cs
var api = new MyApi();

// get resource 1
var resource1 = await api.Resource1.Get(1);
// post resource 1
await api.Resource2.Post(new Resource1Dto()); // assume some data is created here
// get resource 2
var resource2 = await api.Resource2.Get();
// ...
```

---

Previous: [Configuring the HTTP Handler](http-handler.md) | [Keeping Instance State](keeping-state.md)
