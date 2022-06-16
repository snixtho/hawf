using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Serialization;
using Hawf.Client;
using Hawf.Client.Http;

namespace Hawf.Utils;

public static class ApiRequestExtensions
{
    public static string BuildPath(this ApiRequest request)
    {
        var path = request?.Path ?? "";
        
        var matches = Regex.Matches(path, @"{[a-zA-Z0-9_]+}");

        if (matches.Count != request.PathValues.Count)
            throw new InvalidOperationException(
                $"Parameter count in path is not equal to values count ({matches.Count}!={request.PathValues.Count}).");

        for (var i = 0; i < matches.Count; i++)
        {
            var match = matches[i];
            var encoded = HttpUtility.UrlEncode(request.PathValues[i].ToString());
            path = path.Replace(match.Groups[0].Value, encoded);
        }

        return path;
    }

    private static void EnsureFormDataExists(FormDataCollection? formData, ApiRequest request)
    {
        if (formData == null)
            throw new InvalidOperationException($"Form data cannot be null with mimetype: {request.ContentType}");
    }
    
    public static HttpContent? CreateBodyContent(this ApiRequest request)
    {
        switch (request.ContentType)
        {
            case MimeType.Json:
                return JsonContent.Create(request.BodyObject);
            case MimeType.Xml:
                return XmlContent.Create(request.BodyObject);
            case MimeType.FormUrlEncoded:
                EnsureFormDataExists(request.FormData, request);
                return new FormUrlEncodedContent(request.FormData.ToUrlEncodedCollection());
            case MimeType.MultipartForm:
                EnsureFormDataExists(request.FormData, request);
                return request.FormData.ToMultipartFormContent();
            // case MimeType.Text:
            default:
                return new StringContent(request.BodyObject?.ToString() ?? "");
        }
    }

    public static HttpRequestMessage BuildRequest(this ApiRequest request)
    {
        var path = request.BuildPath();
        var query = request.Query?.GenerateString() ?? "";
        var body = request.CreateBodyContent();

        if (request.BaseUrl == null)
            throw new InvalidOperationException("Base URL is null, cannot build request.");

        var urlBuilder = new UriBuilder(request.BaseUrl)
        {
            Query = query
        };

        if (urlBuilder.Path.StartsWith("/"))
            urlBuilder.Path = urlBuilder.Path.Substring(1) + path;
        else
            urlBuilder.Path += path;

        var requestMsg = new HttpRequestMessage
        {
            RequestUri = urlBuilder.Uri,
            Method = request.Method,
            Content = body
        };

        if (request.Headers != null && request.Headers.Count > 0)
        {
            // add headers
            foreach (var (key, value) in request.Headers)
                requestMsg.Headers.Add(key, value);
        }

        requestMsg.Headers.ConnectionClose = !request.KeepAlive;

        return requestMsg;
    }
}