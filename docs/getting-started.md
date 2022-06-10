# Getting Started

## Installation
You can find the nuget package here: https://www.nuget.org/packages/Hawf/


Install with .NET CLI:
```
dotnet add package Hawf
```

Install with Package Manager:
```
Install-Package Hawf -Version 1.0.0
```

Alternatively download the source and build the project yourself.

## Tutorial: Create a basic HTTP API client
In this example we will create a simple API client that can fetch basic information about a Github user.

### Create the response model for the endpoint
We're going to omit some of the response properties to keep it simple:
```csharp
public class GithubBasicUserInfo
{
    public string Login { get; set; }

    public long Id { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("public_repos")]
    public int PublicRepos { get; set; }

    [JsonPropertyName("public_gists")]
    public int PublicGists { get; set; }

    public int Followers { get; set; }

    public int Following { get; set; }

    [JsonPropertyName("avatar_url")]
    public string AvatarUrl { get; set; }
}
```

### Create the API client wrapper
To create a new client, begin creating a new class that will hold all the methods which call the API endpoints. The class should inherit the `ApiBase` base class and be annotated with the `ApiClient` attribute.

Set the base URL for the API in the attribute:
```csharp

[ApiClient("https://api.github.com")]
public class GithubApi : ApiBase<GithubApi>
{
}
```

#### Adding methods
We're going to add a method for obtaining a user's basic information, parsed into and instance of `GithubBasicUserInfo`.

Since the Github API returns JSON, we can use the `GetJsonAsync` method for this. The new code for the class will now look like this:
```csharp
[ApiClient("https://api.github.com")]
public class GithubApi : ApiBase<GithubApi>
{
    /// <summary>
    /// Get basic information about a user.
    /// </summary>
    /// <param name="username">The user's username.</param>
    /// <returns></returns>
    public Task<GithubBasicUserInfo?> GetUserAsync(string username) =>
        GetJsonAsync<GithubBasicUserInfo>("/users/{username}", username);
}
```

`GetJsonAsync` takes a path to the endpoint we are going to call. We can insert the value pattern `{username}` which will be replaced by our parameter value.

It does not really matter what the name inside `{}` is, but the order is important as they are replaced by the values provided in the same order as the parameters to the `GetJsonAsync` method.

### Using the newly created client
Let's create a simple program that uses the new client we created:
```csharp
using GettingStartedTutorial;

// instantiate the API class
var api = new GithubApi();

// call the API method
var user = await api.GetUserAsync("snixtho");

// print the info we got returned
Console.WriteLine($"snixtho created their account at {user.CreatedAt}, has {user.Followers} follower(s) and made {user.PublicRepos} public repositories.");

```

You can find the full source code for this tutorial [here](Examples/GettingStartedTutorial).
