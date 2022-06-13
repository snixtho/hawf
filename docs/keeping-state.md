# Keeping Instance State
Since the API classes allows for instantiation, you can also track internal states if needed between requests. This can be useful if you need to perform sort of authentication flow first to gain access.

Here is an example where you have to login with username and password (NOTE: this is normally not how you design API authentication & authorization, this is only to demonstrate the capabilities), the API client recieves an API key to use in further requests.

```cs
[ApiClient("https://api.my-website.com")]
class MyApi : ApiBase<MyApi> {
    private string _accessToken;

    // authenticate with the server and get an access token
    public async Task AuthenticateAsync(string username, string password) [
        _accessToken = await WithJsonBody(new {
            username = username,
            password = password
        }).GetStringAsync("/authenticate");
    ]

    // get data of current user based on obtained token
    public Task<ApiUser> GetCurrentUserAsync() =>
        WithBearerToken(_accessToken)
        .GetJsonAsync<ApiUser>("/userinfo");
}
```

You can now use the client like this:
```cs
var api = new MyApi();
await api.AuthenticateAsync("user", "password");
var user = await api.GetCurrentUserAsync();
// do something with user ...
```
