﻿using System.ComponentModel;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;
using Hawf.Attributes;
using Hawf.Client.Configuration;
using Hawf.Client.Exceptions;
using Hawf.Client.Http;
using Hawf.Utils;

namespace Hawf.Client;

public class ApiBase<T> : ApiRequestBuilder<T> where T : ApiBase<T>
{
    private HttpClient _client;
    private int _requestCounter;
    private DateTime _requestCounterReset;
    private readonly ApiMemoryCache _cache;

    public ApiBase() : base(new ApiClientConfiguration<T>())
    {
        // client config
        var attr = GetType().GetCustomAttribute<ApiClientAttribute>();
        var attrInfo = attr ?? throw new CustomAttributeFormatException("The API must annotate the ApiClient attribute");

        ClientConfig.BaseUrl = attrInfo.BaseUrl;
        ClientConfig.DefaultUserAgent = attrInfo.DefaultUserAgent;
        ClientConfig.RateLimitMaxRequests = attrInfo.RateLimitMaxRequests;
        ClientConfig.RateLimitTimespan = attrInfo.RateLimitTimespan;
        ClientConfig.UseRateLimit = attrInfo.UseRateLimit;
        ClientConfig.DefaultThrowOnFail = attrInfo.DefaultThrowOnFail;

        _cache = new ApiMemoryCache();
        _requestCounter = -1;
    }

    private async Task TryPreventRateLimit(CancellationToken cancelToken)
    {
        if (ClientConfig.UseRateLimit)
        {
            if (_requestCounter < 0)
            {
                // first request
                _requestCounter = 0;
                _requestCounterReset = DateTime.Now;
            }

            if (_requestCounter >= ClientConfig.RateLimitMaxRequests)
            {
                var epoch = DateTime.Now - _requestCounterReset;

                if (epoch <= ClientConfig.RateLimitTimespan)
                {
                    var timeLeft = ClientConfig.RateLimitTimespan - epoch;
                    if (ClientConfig.WaitForRateLimit)
                        await Task.Delay(timeLeft.Milliseconds, cancelToken);
                    else
                        throw new RateLimitExceededException(timeLeft);
                }

                _requestCounter = 0;
                _requestCounterReset = DateTime.Now;
            }
        }
    }
    
    /// <summary>
    /// Perform a HTTP request.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    /// <exception cref="RateLimitExceededException"></exception>
    /// <exception cref="Exception"></exception>
    private async Task<HttpResponseMessage> SendRequestAsync(ApiRequest request, CancellationToken cancelToken)
    {
        try
        {
            _client = new HttpClient(ClientConfig.HttpHandler);
            
            // build request
            if (!request.Headers.ContainsKey(HttpHeader.UserAgent))
                WithUserAgent(ClientConfig.DefaultUserAgent);

            if (request.BaseUrl == null)
                WithBaseUrl(ClientConfig.BaseUrl);

            var httpRequest = request.BuildRequest();
            
            // return cached object if enabled and available
            if (httpRequest?.RequestUri?.AbsoluteUri == null)
                throw new InvalidOperationException("The absolute URI for the request is null, did you set a base address?");
            
            var cacheKey = httpRequest.RequestUri.AbsoluteUri;
            var cacheResponse = request.CacheResponse || ClientConfig.CacheResponse;
            if (cacheResponse)
            {
                HttpResponseMessage? cachedResponse = null;
                
                if (await _cache.Exists(cacheKey))
                    cachedResponse = await _cache.Get<HttpResponseMessage>(cacheKey);

                if (cachedResponse != null)
                    return cachedResponse;
            }
            
            // prevent rate limit
            await TryPreventRateLimit(cancelToken);

            // send the request and get response
            var response = await _client.SendAsync(httpRequest, cancelToken);

            // cache the response if enabled
            if (cacheResponse)
                await _cache.Set(cacheKey, response, request.CacheTime ?? ClientConfig.DefaultCacheTime);

            if (ClientConfig.DefaultThrowOnFail && !response.IsSuccessStatusCode)
                throw new HawfResponseException(response);
            
            return response;
        }
        catch (Exception ex)
        {
            if (ClientConfig.DefaultThrowOnFail)
                throw;
        }
        finally
        {
            BuilderRequestFinished();
            _requestCounter++;
        }

        throw new Exception("An unknown error occured.");
    }

    /// <summary>
    /// Edit the API client's configuration.
    /// </summary>
    /// <param name="configAction"></param>
    public void Configure(Action<ApiClientConfiguration<T>> configAction) => configAction.Invoke(ClientConfig);
    
    #region Request Methods

    /// <summary>
    /// Send a new request based on current config
    /// and get the response message.
    /// </summary>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    protected async Task<HttpResponseMessage> RequestAsync(CancellationToken cancelToken = default)
        => await SendRequestAsync(RequestInfo, cancelToken);

    /// <summary>
    /// Send a new request based on current config and return a JSON parsed object.
    /// </summary>
    /// <param name="cancelToken"></param>
    /// <typeparam name="TReturn"></typeparam>
    /// <returns></returns>
    protected async Task<TReturn?> RequestJsonAsync<TReturn>(CancellationToken cancelToken = default)
    {
        var response = await SendRequestAsync(RequestInfo, cancelToken);
        var responseText = await response.Content.ReadAsStringAsync(cancelToken);
        return JsonSerializer.Deserialize<TReturn>(responseText, ClientConfig.SerializerOptions);
    }

    /// <summary>
    /// Perform a request and serialize the XML response.
    /// </summary>
    /// <param name="cancelToken"></param>
    /// <typeparam name="TReturn"></typeparam>
    /// <returns></returns>
    protected async Task<TReturn?> RequestXmlAsync<TReturn>(CancellationToken cancelToken = default)
    {
        var response = await SendRequestAsync(RequestInfo, cancelToken);
        var responseText = await response.Content.ReadAsStreamAsync(cancelToken);
        var serializer = new XmlSerializer(typeof(TReturn));
        var responseObject = serializer.Deserialize(responseText);
        return (TReturn?) responseObject;
    }
    
    /// <summary>
    /// Perform a request and return the result as a string.
    /// </summary>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    protected async Task<string> RequestStringAsync(CancellationToken cancelToken = default)
    {
        var response = await SendRequestAsync(RequestInfo, cancelToken);
        return await response.Content.ReadAsStringAsync(cancelToken);
    }
    
    /// <summary>
    /// Perform a request and return a stream containing the result.
    /// </summary>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    protected async Task<Stream> RequestStreamAsync(CancellationToken cancelToken = default)
    {
        var response = await SendRequestAsync(RequestInfo, cancelToken);
        return await response.Content.ReadAsStreamAsync(cancelToken);
    }
    
    /// <summary>
    /// Perform a request and get the raw bytes of the response content.
    /// </summary>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    protected async Task<byte[]> RequestBytesAsync(CancellationToken cancelToken = default)
    {
        var response = await SendRequestAsync(RequestInfo, cancelToken);
        return await response.Content.ReadAsByteArrayAsync(cancelToken);
    }

    /***********/
    
    /// <summary>
    /// Perform a GET request and serialize the returned JSON.
    /// </summary>
    /// <param name="path">Path to the resource</param>
    /// <param name="values">Values to be replaced in the path parameters</param>
    /// <typeparam name="TReturn"></typeparam>
    /// <returns></returns>
    protected async Task<TReturn?> GetJsonAsync<TReturn>(string path="", params object[] values)
    {
        WithPath(path).WithPathValues(values).WithMethod(HttpMethod.Get);
        return await RequestJsonAsync<TReturn>(RequestInfo.CancelToken);
    }
    
    /// <summary>
    /// Perform a POST request and serialize the returned JSON.
    /// </summary>
    /// <param name="path">Path to the resource</param>
    /// <param name="values">Values to be replaced in the path parameters</param>
    /// <typeparam name="TReturn"></typeparam>
    /// <returns></returns>
    protected async Task<TReturn?> PostJsonAsync<TReturn>(string path="", params object[] values)
    {
        WithPath(path).WithPathValues(values).WithMethod(HttpMethod.Post);
        return await RequestJsonAsync<TReturn>(RequestInfo.CancelToken);
    }
    
    /// <summary>
    /// Perform a PATCH request and serialize the returned JSON.
    /// </summary>
    /// <param name="path">Path to the resource</param>
    /// <param name="values">Values to be replaced in the path parameters</param>
    /// <typeparam name="TReturn"></typeparam>
    /// <returns></returns>
    protected async Task<TReturn?> PatchJsonAsync<TReturn>(string path="", params object[] values)
    {
        WithPath(path).WithPathValues(values).WithMethod(HttpMethod.Patch);
        return await RequestJsonAsync<TReturn>(RequestInfo.CancelToken);
    }
    
    /// <summary>
    /// Perform a PUT request and serialize the returned JSON.
    /// </summary>
    /// <param name="path">Path to the resource</param>
    /// <param name="values">Values to be replaced in the path parameters</param>
    /// <typeparam name="TReturn"></typeparam>
    /// <returns></returns>
    protected async Task<TReturn?> PutJsonAsync<TReturn>(string path="", params object[] values)
    {
        WithPath(path).WithPathValues(values).WithMethod(HttpMethod.Put);
        return await RequestJsonAsync<TReturn>(RequestInfo.CancelToken);
    }
    
    /// <summary>
    /// Perform a DELETE request and serialize the returned JSON.
    /// </summary>
    /// <param name="path">Path to the resource</param>
    /// <param name="values">Values to be replaced in the path parameters</param>
    /// <typeparam name="TReturn"></typeparam>
    /// <returns></returns>
    protected async Task<TReturn?> DeleteJsonAsync<TReturn>(string path="", params object[] values)
    {
        WithPath(path).WithPathValues(values).WithMethod(HttpMethod.Delete);
        return await RequestJsonAsync<TReturn>(RequestInfo.CancelToken);
    }
    
    /***********/
    
    /// <summary>
    /// Perform a GET request and return the response as a string.
    /// </summary>
    /// <param name="path">Path to the resource</param>
    /// <param name="values">Values to be replaced in the path parameters</param>
    /// <returns></returns>
    protected async Task<string> GetStringAsync(string path="", params object[] values)
    {
        WithPath(path).WithPathValues(values).WithMethod(HttpMethod.Get);
        return await RequestStringAsync(RequestInfo.CancelToken);
    }
    
    /// <summary>
    /// Perform a POST request and return the response as a string.
    /// </summary>
    /// <param name="path">Path to the resource</param>
    /// <param name="values">Values to be replaced in the path parameters</param>
    /// <returns></returns>
    protected async Task<string> PostStringAsync(string path="", params object[] values)
    {
        WithPath(path).WithPathValues(values).WithMethod(HttpMethod.Post);
        return await RequestStringAsync(RequestInfo.CancelToken);
    }
    
    /// <summary>
    /// Perform a PATCH request and return the response as a string.
    /// </summary>
    /// <param name="path">Path to the resource</param>
    /// <param name="values">Values to be replaced in the path parameters</param>
    /// <returns></returns>
    protected async Task<string> PatchStringAsync(string path="", params object[] values)
    {
        WithPath(path).WithPathValues(values).WithMethod(HttpMethod.Patch);
        return await RequestStringAsync(RequestInfo.CancelToken);
    }
    
    /// <summary>
    /// Perform a PUT request and return the response as a string.
    /// </summary>
    /// <param name="path">Path to the resource</param>
    /// <param name="values">Values to be replaced in the path parameters</param>
    /// <returns></returns>
    protected async Task<string> PutStringAsync(string path="", params object[] values)
    {
        WithPath(path).WithPathValues(values).WithMethod(HttpMethod.Put);
        return await RequestStringAsync(RequestInfo.CancelToken);
    }
    
    /// <summary>
    /// Perform a DELETE request and return the response as a string.
    /// </summary>
    /// <param name="path">Path to the resource</param>
    /// <param name="values">Values to be replaced in the path parameters</param>
    /// <returns></returns>
    protected async Task<string> DeleteStringAsync(string path="", params object[] values)
    {
        WithPath(path).WithPathValues(values).WithMethod(HttpMethod.Delete);
        return await RequestStringAsync(RequestInfo.CancelToken);
    }
    
    /***********/
    
    /// <summary>
    /// Perform a GET request and return the response as a raw bytes.
    /// </summary>
    /// <param name="path">Path to the resource</param>
    /// <param name="values">Values to be replaced in the path parameters</param>
    /// <returns></returns>
    protected async Task<byte[]> GetBytesAsync(string path="", params object[] values)
    {
        WithPath(path).WithPathValues(values).WithMethod(HttpMethod.Get);
        return await RequestBytesAsync(RequestInfo.CancelToken);
    }
    
    /// <summary>
    /// Perform a POST request and return the response as a raw bytes.
    /// </summary>
    /// <param name="path">Path to the resource</param>
    /// <param name="values">Values to be replaced in the path parameters</param>
    /// <returns></returns>
    protected async Task<byte[]> PostBytesAsync(string path="", params object[] values)
    {
        WithPath(path).WithPathValues(values).WithMethod(HttpMethod.Post);
        return await RequestBytesAsync(RequestInfo.CancelToken);
    }
    
    /// <summary>
    /// Perform a PATCH request and return the response as a raw bytes.
    /// </summary>
    /// <param name="path">Path to the resource</param>
    /// <param name="values">Values to be replaced in the path parameters</param>
    /// <returns></returns>
    protected async Task<byte[]> PatchBytesAsync(string path="", params object[] values)
    {
        WithPath(path).WithPathValues(values).WithMethod(HttpMethod.Patch);
        return await RequestBytesAsync(RequestInfo.CancelToken);
    }
    
    /// <summary>
    /// Perform a PUT request and return the response as a raw bytes.
    /// </summary>
    /// <param name="path">Path to the resource</param>
    /// <param name="values">Values to be replaced in the path parameters</param>
    /// <returns></returns>
    protected async Task<byte[]> PutBytesAsync(string path="", params object[] values)
    {
        WithPath(path).WithPathValues(values).WithMethod(HttpMethod.Put);
        return await RequestBytesAsync(RequestInfo.CancelToken);
    }
    
    /// <summary>
    /// Perform a DELETE request and return the response as a raw bytes.
    /// </summary>
    /// <param name="path">Path to the resource</param>
    /// <param name="values">Values to be replaced in the path parameters</param>
    /// <returns></returns>
    protected async Task<byte[]> DeleteBytesAsync(string path="", params object[] values)
    {
        WithPath(path).WithPathValues(values).WithMethod(HttpMethod.Delete);
        return await RequestBytesAsync(RequestInfo.CancelToken);
    }
    
    /***********/
    
    /// <summary>
    /// Perform a GET request and return the response as a stream.
    /// </summary>
    /// <param name="path">Path to the resource</param>
    /// <param name="values">Values to be replaced in the path parameters</param>
    /// <returns></returns>
    protected async Task<Stream> GetStreamAsync(string path="", params object[] values)
    {
        WithPath(path).WithPathValues(values).WithMethod(HttpMethod.Get);
        return await RequestStreamAsync(RequestInfo.CancelToken);
    }
    
    /// <summary>
    /// Perform a POST request and return the response as a stream.
    /// </summary>
    /// <param name="path">Path to the resource</param>
    /// <param name="values">Values to be replaced in the path parameters</param>
    /// <returns></returns>
    protected async Task<Stream> PostStreamAsync(string path="", params object[] values)
    {
        WithPath(path).WithPathValues(values).WithMethod(HttpMethod.Post);
        return await RequestStreamAsync(RequestInfo.CancelToken);
    }
    
    /// <summary>
    /// Perform a PATCH request and return the response as a stream.
    /// </summary>
    /// <param name="path">Path to the resource</param>
    /// <param name="values">Values to be replaced in the path parameters</param>
    /// <returns></returns>
    protected async Task<Stream> PatchStreamAsync(string path="", params object[] values)
    {
        WithPath(path).WithPathValues(values).WithMethod(HttpMethod.Patch);
        return await RequestStreamAsync(RequestInfo.CancelToken);
    }
    
    /// <summary>
    /// Perform a PUT request and return the response as a stream.
    /// </summary>
    /// <param name="path">Path to the resource</param>
    /// <param name="values">Values to be replaced in the path parameters</param>
    /// <returns></returns>
    protected async Task<Stream> PutStreamAsync(string path="", params object[] values)
    {
        WithPath(path).WithPathValues(values).WithMethod(HttpMethod.Put);
        return await RequestStreamAsync(RequestInfo.CancelToken);
    }
    
    /// <summary>
    /// Perform a DELETE request and return the response as a stream.
    /// </summary>
    /// <param name="path">Path to the resource</param>
    /// <param name="values">Values to be replaced in the path parameters</param>
    /// <returns></returns>
    protected async Task<Stream> DeleteStreamAsync(string path="", params object[] values)
    {
        WithPath(path).WithPathValues(values).WithMethod(HttpMethod.Delete);
        return await RequestStreamAsync(RequestInfo.CancelToken);
    }
    
    /***********/
    
    /// <summary>
    /// Perform a GET request and serialize the returned XML.
    /// </summary>
    /// <param name="path">Path to the resource</param>
    /// <param name="values">Values to be replaced in the path parameters</param>
    /// <typeparam name="TReturn"></typeparam>
    /// <returns></returns>
    protected async Task<TReturn?> GetXmlAsync<TReturn>(string path="", params object[] values)
    {
        WithPath(path).WithPathValues(values).WithMethod(HttpMethod.Get);
        return await RequestXmlAsync<TReturn>(RequestInfo.CancelToken);
    }
    
    /// <summary>
    /// Perform a POST request and serialize the returned XML.
    /// </summary>
    /// <param name="path">Path to the resource</param>
    /// <param name="values">Values to be replaced in the path parameters</param>
    /// <typeparam name="TReturn"></typeparam>
    /// <returns></returns>
    protected async Task<TReturn?> PostXmlAsync<TReturn>(string path="", params object[] values)
    {
        WithPath(path).WithPathValues(values).WithMethod(HttpMethod.Post);
        return await RequestXmlAsync<TReturn>(RequestInfo.CancelToken);
    }
    
    /// <summary>
    /// Perform a PATCH request and serialize the returned XML.
    /// </summary>
    /// <param name="path">Path to the resource</param>
    /// <param name="values">Values to be replaced in the path parameters</param>
    /// <typeparam name="TReturn"></typeparam>
    /// <returns></returns>
    protected async Task<TReturn?> PatchXmlAsync<TReturn>(string path="", params object[] values)
    {
        WithPath(path).WithPathValues(values).WithMethod(HttpMethod.Patch);
        return await RequestXmlAsync<TReturn>(RequestInfo.CancelToken);
    }
    
    /// <summary>
    /// Perform a PUT request and serialize the returned XML.
    /// </summary>
    /// <param name="path">Path to the resource</param>
    /// <param name="values">Values to be replaced in the path parameters</param>
    /// <typeparam name="TReturn"></typeparam>
    /// <returns></returns>
    protected async Task<TReturn?> PutXmlAsync<TReturn>(string path="", params object[] values)
    {
        WithPath(path).WithPathValues(values).WithMethod(HttpMethod.Put);
        return await RequestJsonAsync<TReturn>(RequestInfo.CancelToken);
    }
    
    /// <summary>
    /// Perform a DELETE request and serialize the returned XML.
    /// </summary>
    /// <param name="path">Path to the resource</param>
    /// <param name="values">Values to be replaced in the path parameters</param>
    /// <typeparam name="TReturn"></typeparam>
    /// <returns></returns>
    protected async Task<TReturn?> DeleteXmlAsync<TReturn>(string path="", params object[] values)
    {
        WithPath(path).WithPathValues(values).WithMethod(HttpMethod.Delete);
        return await RequestXmlAsync<TReturn>(RequestInfo.CancelToken);
    }
    
    #endregion
}
