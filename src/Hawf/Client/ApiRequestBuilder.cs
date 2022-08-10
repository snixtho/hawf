using System.Collections;
using System.Net.Cache;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using Hawf.Attributes;
using Hawf.Client;
using Hawf.Client.Http;

namespace Hawf;

public class ApiRequestBuilder<T> where T : ApiRequestBuilder<T>
{
    protected ApiRequest RequestInfo { get; private set; }
    private bool _isBuildingRequest;

    /// <summary>
    /// Begin a new request if one isn't currently being built already.
    /// </summary>
    /// <returns></returns>
    protected T EnsureNewRequest()
    {
        if (_isBuildingRequest) return (T) this;
        
        RequestInfo = new ApiRequest();

        RequestInfo.Headers = new Dictionary<string, string>();
        RequestInfo.PathValues = new List<object>();
        RequestInfo.Query = new QueryParamsCollection();
        RequestInfo.FormData = new FormDataCollection();
        
        _isBuildingRequest = true;
        
        return (T) this;
    }

    /// <summary>
    /// Mark current request as finished. Should be called after
    /// it has been sent to the server.
    /// </summary>
    protected void BuilderRequestFinished()
    {
        _isBuildingRequest = false;
    }

    /// <summary>
    /// Add or set a HTTP header to the request.
    /// </summary>
    /// <param name="name">Name of the header</param>
    /// <param name="value">Value of the header</param>
    /// <returns></returns>
    protected T WithHeader(string name, string? value)
    {
        EnsureNewRequest();

        if (value == null)
            return (T) this;

        RequestInfo.Headers.Add(name, value);
        return (T) this;
    }

    /// <summary>
    /// Set the user agent header of the request.
    /// </summary>
    /// <param name="value">User agent value</param>
    /// <returns></returns>
    protected T WithUserAgent(string value) => WithHeader(HttpHeader.UserAgent, value);

    /// <summary>
    /// Add a URL query parameter.
    /// </summary>
    /// <param name="name">Name of the query value</param>
    /// <param name="value">Value of the query value, not encoded as it will be encoded later</param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    protected T WithQueryParam<TValue>(string name, TValue? value)
    {
        EnsureNewRequest();

        if (value == null)
            return (T) this;
        
        RequestInfo.Query.Add(name, value);
        
        return (T) this;
    }

    /// <summary>
    /// Add a form data key/value pair and set the content type
    /// to x-www-form-urlencoded.
    /// </summary>
    /// <param name="key">Name of the parameter.</param>
    /// <param name="content">Value of the parameter that can be serialized to a string.</param>
    /// <returns></returns>
    protected T WithFormParam(string key, object? content)
    {
        if (content == null)
            // ignore null values
            return (T) this;
        
        WithContentType(MimeType.FormUrlEncoded);
        RequestInfo.FormData.Add(key, content);

        return (T) this;
    }

    /// <summary>
    /// Add a form data key/value pair anset content type
    /// to form-data.
    /// </summary>
    /// <param name="key">Name of the value</param>
    /// <param name="content">Content of the value</param>
    /// <returns></returns>
    protected T WithMultipartFormData(string key, object? content)
    {
        if (content == null)
            // ignore null values
            return (T) this;
        
        WithContentType(MimeType.MultipartForm);
        RequestInfo.FormData.Add(key, content);

        return (T) this;
    }

    /// <summary>
    /// Set the request path pattern from the base address.
    /// </summary>
    /// <param name="path">Request path endpoint</param>
    /// <returns></returns>
    protected T WithPath(string path)
    {
        EnsureNewRequest().RequestInfo.Path = path;
        return (T) this;
    }

    /// <summary>
    /// Add values to be replaced in the path pattern.
    /// The order is important, make sure the values are added
    /// in the same order as they are specified in the path.
    /// </summary>
    /// <param name="values">Values to add</param>
    /// <returns></returns>
    protected T WithPathValues(params object[] values)
    {
        EnsureNewRequest();
        RequestInfo.PathValues.AddRange(values);
        return (T) this;
    }

    /// <summary>
    /// Set the HTTP method to use in the request.
    /// </summary>
    /// <param name="method">HTTP method</param>
    /// <returns></returns>
    protected T WithMethod(HttpMethod method)
    {
        EnsureNewRequest().RequestInfo.Method = method;
        return (T) this;
    }

    /// <summary>
    /// Set the base address/url for with to append the path and query to.
    /// </summary>
    /// <param name="url">Base URL for the request</param>
    /// <returns></returns>
    protected T WithBaseUrl(Uri url)
    {
        EnsureNewRequest().RequestInfo.BaseUrl = url;
        return (T) this;
    }

    /// <summary>
    /// Set the base address/url for with to append the path and query to.
    /// </summary>
    /// <param name="url">Base URL for the request</param>
    /// <returns></returns>
    protected T WithBaseUrl(string url) => WithBaseUrl(new Uri(url));

    /// <summary>
    /// Set the cancellation token to allow you to abort
    /// the current request.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected T WithCancelToken(CancellationToken token)
    {
        EnsureNewRequest().RequestInfo.CancelToken = token;
        return (T) this;
    }

    /// <summary>
    /// Add query parameter from a class instance.
    /// </summary>
    /// <param name="options">A class instance that defines query parameters as properties.</param>
    /// <typeparam name="TOptions"></typeparam>
    /// <returns></returns>
    protected T WithQueryOptions<TOptions>(TOptions? options)
    {
        if (options == null)
            return (T) this;
        
        var type = typeof(TOptions);
        var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

        foreach (var property in properties)
        {
            var propertyOptions = property.GetCustomAttribute<QueryPropertyAttribute>() ?? new QueryPropertyAttribute();

            var name = propertyOptions?.Name ?? char.ToLower(property.Name[0]) + property.Name[1..];
            object? value;
            var objValue = property.GetValue(options);

            if (objValue == null)
                continue;

            if (propertyOptions is {IntegerEnum: true})
                value = Convert.ToInt32(objValue);
            else if (typeof(string) != property.PropertyType && typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
            {
                var objArr = ((IEnumerable)objValue).Cast<object>();
                var objStrMapped = objArr.Select(e => e.ToString());
                value = string.Join(propertyOptions.ListSeparator, objStrMapped);
            }
            else
                value = objValue;

            WithQueryParam(name, value?.ToString());
        }

        return (T) this;
    }
    
    /// <summary>
    /// Add query parameter from a class instance.
    /// </summary>
    /// <param name="optionsAction">Callback for setting values of the class instance properties.</param>
    /// <typeparam name="TOptions"></typeparam>
    /// <returns></returns>
    protected T WithQueryOptions<TOptions>(Action<TOptions>? optionsAction)
    {
        var options = Activator.CreateInstance<TOptions>();
        optionsAction?.Invoke(options);
        WithQueryOptions(options);
        
        return (T) this;
    }

    /// <summary>
    /// Cache the current request for a specific amount of time.
    /// </summary>
    /// <param name="timeSpan">Time to cache the request for</param>
    /// <returns></returns>
    protected T CacheResponseFor(TimeSpan timeSpan)
    {
        EnsureNewRequest();

        if (timeSpan == TimeSpan.Zero)
        {
            // ignore zero times
            RequestInfo.CacheResponse = false;
            return (T) this;
        }
        
        RequestInfo.CacheResponse = true;
        RequestInfo.CacheTime = timeSpan;
        
        return (T) this;
    }
    
    /// <summary>
    /// Cache the current request for a specific amount of time.
    /// </summary>
    /// <param name="milliseconds">Time to cache the request for in milliseconds/param>
    /// <returns></returns>
    protected T CacheResponseFor(int milliseconds)
    {
        CacheResponseFor(TimeSpan.FromMilliseconds(milliseconds));
        
        return (T) this;
    }

    /// <summary>
    /// Add an authorization header with a bearer token.
    /// </summary>
    /// <param name="token">The bearer token to use.</param>
    /// <returns></returns>
    protected T WithBearerToken(string token)
    {
        WithHeader(HttpHeader.Authorization, $"Bearer {token}");
        
        return (T) this;
    }

    /// <summary>
    /// Set the authorization header to basic auth.
    /// </summary>
    /// <param name="username">Name of the user</param>
    /// <param name="password">User's password</param>
    /// <returns></returns>
    protected T WithBasicAuth(string username, string password)
    {
        var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
        WithHeader(HttpHeader.Authorization, $"Basic {encoded}");

        return (T) this;
    }

    /// <summary>
    /// Add authorization using a specific scheme.
    /// </summary>
    /// <param name="scheme">Name of the scheme.</param>
    /// <param name="token">Credentials/access token</param>
    /// <returns></returns>
    protected T WithAuthorization(string scheme, string token)
    {
        WithHeader(HttpHeader.Authorization, $"{scheme} {token}");

        return (T) this;
    }

    protected T WithContentType(string mimeType)
    {
        EnsureNewRequest();

        RequestInfo.ContentType = mimeType;

        return (T) this;
    }

    /// <summary>
    /// Add an object and set the content type to application/json.
    /// The object is serialized to JSON when performing the request
    /// </summary>
    /// <param name="bodyContent">Object to be serialized to JSON</param>
    /// <typeparam name="TBody"></typeparam>
    /// <returns></returns>
    protected T WithJsonBody<TBody>(TBody bodyContent)
    {
        EnsureNewRequest();

        RequestInfo.BodyObject = bodyContent;
        WithContentType(MimeType.Json);

        return (T) this;
    }

    /// <summary>
    /// Add a string body to the requests and set content type to plain/text.
    /// </summary>
    /// <param name="bodyContent"></param>
    /// <returns></returns>
    protected T WithStringBody(string bodyContent)
    {
        EnsureNewRequest();

        RequestInfo.BodyObject = bodyContent;
        WithContentType(MimeType.Text);

        return (T) this;
    }

    /// <summary>
    /// Add an object to be generated as XML in the request.
    /// </summary>
    /// <param name="bodyContent">Object to serialize to XML</param>
    /// <typeparam name="TBody"></typeparam>
    /// <returns></returns>
    protected T WithXmlBody<TBody>(TBody bodyContent)
    {
        EnsureNewRequest();

        RequestInfo.BodyObject = bodyContent;
        WithContentType(MimeType.Xml);

        return (T) this;
    }
}