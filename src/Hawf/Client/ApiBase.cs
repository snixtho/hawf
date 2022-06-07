﻿using System.ComponentModel;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using Hawf.Attributes;
using Hawf.Client.Http;
using Hawf.Utils;

namespace Hawf.Client;

public class ApiBase<T> : ApiRequestBuilder<T> where T : ApiBase<T>
{
    private ApiClientAttribute _apiAttr;
    private HttpClient _client;
    private int _requestCounter;
    private DateTime _requestCounterReset;

    public ApiBase() => Init();
    public ApiBase(HttpMessageHandler handler) => Init(handler);

    private void Init(HttpMessageHandler? handler=null)
    {
        if (handler == null)
            // default handler
            handler = new HttpClientHandler();

        _client = new HttpClient(handler);
        
        // client info
        var attr = GetType().GetCustomAttribute<ApiClientAttribute>();
        _apiAttr = attr ?? throw new Exception("The API must annotate the ApiClient attribute");

        _requestCounter = -1;
    }
    
    private async Task<HttpResponseMessage> SendRequestAsync(ApiRequest request, CancellationToken cancelToken = default)
    {
        try
        {
            // prevent rate limit
            if (_apiAttr.UseRatelimit)
            {
                if (_requestCounter < 0)
                {
                    // first request
                    _requestCounter = 0;
                    _requestCounterReset = DateTime.Now;
                }

                if (_requestCounter >= _apiAttr.DefaultRateLimitMaxRequests)
                {
                    var epoch = DateTime.Now - _requestCounterReset;
                    
                    if (epoch <= _apiAttr.DefaultRateLimitTimespan)
                        await Task.Delay(epoch.Milliseconds);
                    
                    _requestCounter = 0;
                    _requestCounterReset = DateTime.Now;
                }
            }
            
            if (!request.Headers.ContainsKey(HttpHeader.UserAgent))
                WithUserAgent(_apiAttr.DefaultUserAgent);

            WithBaseUrl(_apiAttr.BaseUrl);

            var httpRequest = request.BuildRequest();
            var response = await _client.SendAsync(httpRequest, cancelToken);

            if (_apiAttr.ThrowOnFail && !response.IsSuccessStatusCode)
                throw new Exception($"Request failed with response: {response.StatusCode}");
            
            return response;
        }
        catch (Exception ex)
        {
            if (_apiAttr.ThrowOnFail)
                throw;
        }
        finally
        {
            BuilderRequestFinished();
            _requestCounter++;
        }

        throw new Exception("An unknown error occured.");
    }

    private async Task<HttpResponseMessage> RequestAsync(CancellationToken cancelToken = default)
        => await SendRequestAsync(RequestInfo, cancelToken);

    private async Task<TReturn?> RequestJsonAsync<TReturn>(CancellationToken cancelToken = default)
    {
        var response = await SendRequestAsync(RequestInfo, cancelToken);
        var responseText = await response.Content.ReadAsStringAsync(cancelToken);
        return JsonSerializer.Deserialize<TReturn>(responseText, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
    
    protected async Task<string> RequestStringAsync(CancellationToken cancelToken = default)
    {
        var response = await SendRequestAsync(RequestInfo, cancelToken);
        return await response.Content.ReadAsStringAsync(cancelToken);
    }
    
    protected async Task<Stream> RequestStreamAsync(CancellationToken cancelToken = default)
    {
        var response = await SendRequestAsync(RequestInfo, cancelToken);
        return await response.Content.ReadAsStreamAsync(cancelToken);
    }
    
    protected async Task<byte[]> RequestBytesAsync(CancellationToken cancelToken = default)
    {
        var response = await SendRequestAsync(RequestInfo, cancelToken);
        return await response.Content.ReadAsByteArrayAsync(cancelToken);
    }

    protected async Task<TReturn?> GetJsonAsync<TReturn>(string path, params object[] values)
    {
        WithPath(path).AddPathValues(values).WithMethod(HttpMethod.Get);
        return await RequestJsonAsync<TReturn>(RequestInfo.CancelToken);
    }
    
    protected async Task<TReturn?> PostJsonAsync<TReturn>(string path, params object[] values)
    {
        WithPath(path).AddPathValues(values).WithMethod(HttpMethod.Post);
        return await RequestJsonAsync<TReturn>(RequestInfo.CancelToken);
    }
    
    protected async Task<TReturn?> PatchJsonAsync<TReturn>(string path, params object[] values)
    {
        WithPath(path).AddPathValues(values).WithMethod(HttpMethod.Patch);
        return await RequestJsonAsync<TReturn>(RequestInfo.CancelToken);
    }
    
    protected async Task<TReturn?> PutJsonAsync<TReturn>(string path, params object[] values)
    {
        WithPath(path).AddPathValues(values).WithMethod(HttpMethod.Put);
        return await RequestJsonAsync<TReturn>(RequestInfo.CancelToken);
    }
    
    protected async Task<TReturn?> DeleteJsonAsync<TReturn>(string path, params object[] values)
    {
        WithPath(path).AddPathValues(values).WithMethod(HttpMethod.Delete);
        return await RequestJsonAsync<TReturn>(RequestInfo.CancelToken);
    }
    
    
    protected async Task<string> GetStringAsync(string path, params object[] values)
    {
        WithPath(path).AddPathValues(values).WithMethod(HttpMethod.Get);
        return await RequestStringAsync(RequestInfo.CancelToken);
    }
    
    protected async Task<string> PostStringAsync(string path, params object[] values)
    {
        WithPath(path).AddPathValues(values).WithMethod(HttpMethod.Post);
        return await RequestStringAsync(RequestInfo.CancelToken);
    }
    
    protected async Task<string> PatchStringAsync(string path, params object[] values)
    {
        WithPath(path).AddPathValues(values).WithMethod(HttpMethod.Patch);
        return await RequestStringAsync(RequestInfo.CancelToken);
    }
    
    protected async Task<string> PutStringAsync(string path, params object[] values)
    {
        WithPath(path).AddPathValues(values).WithMethod(HttpMethod.Put);
        return await RequestStringAsync(RequestInfo.CancelToken);
    }
    
    protected async Task<string> DeleteStringAsync(string path, params object[] values)
    {
        WithPath(path).AddPathValues(values).WithMethod(HttpMethod.Delete);
        return await RequestStringAsync(RequestInfo.CancelToken);
    }
    
    
    protected async Task<byte[]> GetBytesAsync(string path, params object[] values)
    {
        WithPath(path).AddPathValues(values).WithMethod(HttpMethod.Get);
        return await RequestBytesAsync(RequestInfo.CancelToken);
    }
    
    protected async Task<byte[]> PostBytesAsync(string path, params object[] values)
    {
        WithPath(path).AddPathValues(values).WithMethod(HttpMethod.Post);
        return await RequestBytesAsync(RequestInfo.CancelToken);
    }
    
    protected async Task<byte[]> PatchBytesAsync(string path, params object[] values)
    {
        WithPath(path).AddPathValues(values).WithMethod(HttpMethod.Patch);
        return await RequestBytesAsync(RequestInfo.CancelToken);
    }
    
    protected async Task<byte[]> PutBytesAsync(string path, params object[] values)
    {
        WithPath(path).AddPathValues(values).WithMethod(HttpMethod.Put);
        return await RequestBytesAsync(RequestInfo.CancelToken);
    }
    
    protected async Task<byte[]> DeleteBytesAsync(string path, params object[] values)
    {
        WithPath(path).AddPathValues(values).WithMethod(HttpMethod.Delete);
        return await RequestBytesAsync(RequestInfo.CancelToken);
    }
    
    
    protected async Task<Stream> GetStreamAsync(string path, params object[] values)
    {
        WithPath(path).AddPathValues(values).WithMethod(HttpMethod.Get);
        return await RequestStreamAsync(RequestInfo.CancelToken);
    }
    
    protected async Task<Stream> PostStreamAsync(string path, params object[] values)
    {
        WithPath(path).AddPathValues(values).WithMethod(HttpMethod.Post);
        return await RequestStreamAsync(RequestInfo.CancelToken);
    }
    
    protected async Task<Stream> PatchStreamAsync(string path, params object[] values)
    {
        WithPath(path).AddPathValues(values).WithMethod(HttpMethod.Patch);
        return await RequestStreamAsync(RequestInfo.CancelToken);
    }
    
    protected async Task<Stream> PutStreamAsync(string path, params object[] values)
    {
        WithPath(path).AddPathValues(values).WithMethod(HttpMethod.Put);
        return await RequestStreamAsync(RequestInfo.CancelToken);
    }
    
    protected async Task<Stream> DeleteStreamAsync(string path, params object[] values)
    {
        WithPath(path).AddPathValues(values).WithMethod(HttpMethod.Delete);
        return await RequestStreamAsync(RequestInfo.CancelToken);
    }
}
