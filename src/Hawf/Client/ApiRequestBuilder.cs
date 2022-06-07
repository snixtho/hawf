using System.Collections;
using System.Net.Http.Headers;
using System.Reflection;
using Hawf.Attributes;
using Hawf.Client;
using Hawf.Client.Http;

namespace Hawf;

public class ApiRequestBuilder<T> where T : ApiRequestBuilder<T>
{
    protected ApiRequest RequestInfo { get; private set; }
    private bool _isBuildingRequest;

    protected T EnsureNewRequest()
    {
        if (_isBuildingRequest) return (T) this;
        
        RequestInfo = new ApiRequest();

        RequestInfo.Headers = new Dictionary<string, string>();
        RequestInfo.PathValues = new List<object>();
        RequestInfo.Query = new QueryParamsCollection();
        
        _isBuildingRequest = true;
        
        return (T) this;
    }

    protected void BuilderRequestFinished()
    {
        _isBuildingRequest = false;
    }

    protected T WithHeader(string name, string? value)
    {
        EnsureNewRequest();

        if (value == null)
            return (T) this;

        RequestInfo.Headers.Add(name, value);
        return (T) this;
    }

    protected T WithUserAgent(string value) => WithHeader(HttpHeader.UserAgent, value);

    protected T WithQueryParam<TValue>(string name, TValue? value)
    {
        EnsureNewRequest();

        if (value == null)
            return (T) this;
        
        RequestInfo.Query.Add(name, value);
        
        return (T) this;
    }

    protected T WithPath(string path)
    {
        EnsureNewRequest().RequestInfo.Path = path;
        return (T) this;
    }

    protected T AddPathValues(params object[] values)
    {
        EnsureNewRequest();
        RequestInfo.PathValues.AddRange(values);
        return (T) this;
    }

    protected T WithMethod(HttpMethod method)
    {
        EnsureNewRequest().RequestInfo.Method = method;
        return (T) this;
    }

    protected T WithBaseUrl(Uri url)
    {
        EnsureNewRequest().RequestInfo.BaseUrl = url;
        return (T) this;
    }

    protected T WithBaseUrl(string url) => WithBaseUrl(new Uri(url));

    protected T WithCancelToken(CancellationToken token)
    {
        EnsureNewRequest().RequestInfo.CancelToken = token;
        return (T) this;
    }

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
            else if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                value = string.Join(propertyOptions.ListSeparator, objValue);
            else
                value = objValue;

            WithQueryParam(name, value?.ToString());
        }

        return (T) this;
    }
    
    protected T WithQueryOptions<TOptions>(Action<TOptions>? optionsAction)
    {
        var options = Activator.CreateInstance<TOptions>();
        optionsAction?.Invoke(options);
        WithQueryOptions(options);
        
        return (T) this;
    }
}