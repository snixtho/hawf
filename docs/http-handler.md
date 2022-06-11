# Configuring The HTTP Handler
The HTTP handler (see [HttpMessageHandler](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpmessagehandler?view=net-6.0) on microsoft docs) is a base class used for configuring the request and response of a HTTP message. There are several classes that inherits the HttpMessageHandler, which allows you do various things like configuring proxy, authorization, client certificates, cookies etc.

By default an instance of `HttpClientHandler` is used, but you can configure the handler to however you want by using the `Configure` method.

For example, to use proxy, you can set it to an instance of `HttpClientHandler`:
```cs
Configure(options => {
    options.HttpHandler = new HttpClientHandler
    {
        Proxy = new WebProxy("http://127.0.0.1:5000"),
        UseProxy = true
    };
});
```
