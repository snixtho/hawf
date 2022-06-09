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