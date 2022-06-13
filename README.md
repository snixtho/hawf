# hawf
Http API Wrapper Framework - Quickly build API wrappers and clients

# Installation
You can find the package on [NuGet](https://www.nuget.org/packages/Hawf/) or install through command line:

```
dotnet add package Hawf
```

# Basic Usage

Creating an API is as simple as inherting the `ApiBase` and annotate the `ApiClient` attribute:

```csharp
[ApiClient("https://api.mywebsite.com")]
public class MyApi : ApiBase<MyApi>
{
    public Task<string> MyApiCallAsync() => GetStringAsync("/myendpoint");
}
```

You can then instantiate the class and call the api endpoint:

```csharp
var api = new MyApi();

var response = await api.MyApiCallAsync();

Console.WriteLine(response);
```

This is only a very basic example of how it works. The framework provides various convenient methods and tools to quickly create a complete client for your API.

# Features
- Quickly define your API endpoints into client code with a rich builder API.
- Supports automatic JSON deserialization.
- Handles authentication and authorization of your endpoints.
- Can handle rate limits on APIs
- Based on the Task Asynchronous Programming (TAP) pattern.
- Response caching.
- Standardizes the code structure to keep it readable and maintainable.

For a complete overview of all the features, check out the documentation.

# Documentation
Check out the main documentation [here](docs/).
