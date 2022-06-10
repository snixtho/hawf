# Building API Requests

The `ApiBase` class inherits the `ApiRequestBuilder` class that provides many convenient builder methods to create the exact API request you need for one of your endpoints.

Whenever you call one of these methods, the internal state changes, indicating that a new request is being built. To end a request and call the endpoint, call any of the request action methods.

## Request Action Methods
`ApiBase` provides several action methods depending on the response format returned and the HTTP method required. Currently, the supported response formats are streams, plain strings, byte arrays and JSON.

All the method names are structured in a <HTTP_ACTION><RESPONSE_FORMAT>Async() format. For example to get a string from an endpoint, you can call the `GetStringAsync()` method.

### String Responses
For anything that just requires plain text to returned, you can call the string request methods.

- **HTTP GET:** `GetStringAsync(string path, params object[] values)`
- **HTTP POST:** `PostStringAsync(string path, params object[] values)`
- **HTTP PATCH:** `PatchStringAsync(string path, params object[] values)`
- **HTTP PUT:** `PutStringAsync(string path, params object[] values)`
- **HTTP DELETE:** `DeleteStringAsync(string path, params object[] values)`

### Byte Array Responses
These methods are useful for anything that returns binary data.

- **HTTP GET:** `GetBytesAsync(string path, params object[] values)`
- **HTTP POST:** `PostBytesAsync(string path, params object[] values)`
- **HTTP PATCH:** `PatchBytesAsync(string path, params object[] values)`
- **HTTP PUT:** `PutBytesAsync(string path, params object[] values)`
- **HTTP DELETE:** `DeleteBytesAsync(string path, params object[] values)`

### Stream Responses
Like byte arrays, these stream methods works well for binary data, the difference is that you get a stream back that you can read, copy to other streams, save to files etc.

- **HTTP GET:** `GetStreamAsync(string path, params object[] values)`
- **HTTP POST:** `PostStreamAsync(string path, params object[] values)`
- **HTTP PATCH:** `PatchStreamAsync(string path, params object[] values)`
- **HTTP PUT:** `PutStreamAsync(string path, params object[] values)`
- **HTTP DELETE:** `DeleteStreamAsync(string path, params object[] values)`

### JSON Responses
Most HTTP API's use JSON to return data as they provide a simple way to structure and serialize/deserialize the information sent. `ApiBase` provides automatic deserialization of JSON objects into native C# objects.

Internally `System.Text.Json` is used for deserialization and you can use the declarative model builder it provides to create your objects.

For example, imagine you have a class that holds some user information:

```cs
public class User {
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
}
```

And you want to call an endpoint that returns this user information. You can call the `GetJsonAsync` method:

```cs
// get user with id 1 from API
User user = await GetJsonAsync<User>("/user/{id}", 1);
```

For an example that uses the JSON methods, check out the tutorial in [Getting Started](getting-started.md).

The following methods are available for JSON responses:

- **HTTP GET:** `GetJsonAsync<T>(string path, params object[] values)`
- **HTTP POST:** `PostJsonAsync<T>(string path, params object[] values)`
- **HTTP PATCH:** `PatchJsonAsync<T>(string path, params object[] values)`
- **HTTP PUT:** `PutJsonAsync<T>(string path, params object[] values)`
- **HTTP DELETE:** `DeleteJsonAsync<T>(string path, params object[] values)`

