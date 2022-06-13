using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using Hawf.Client;
using Hawf.Client.Http;

namespace Hawf.Utils;

public static class ApiRequestExtensions
{
    public static string BuildPath(this ApiRequest request)
    {
        var matches = Regex.Matches(request.Path, @"{[a-zA-Z0-9_]+}");

        if (matches.Count != request.PathValues.Count)
            throw new InvalidOperationException(
                $"Parameter count in path is not equal to values count ({matches.Count}!={request.PathValues.Count}).");
            
        var paramsPath = request.Path;

        for (var i = 0; i < matches.Count; i++)
        {
            var match = matches[i];
            paramsPath = paramsPath.Replace(match.Groups[0].Value, request.PathValues[i].ToString());
        }

        return paramsPath;
    }

    public static HttpContent? CreateBodyContent(this ApiRequest request)
    {
        switch (request.MimeType)
        {
            case MimeType.Json:
                return JsonContent.Create(request.BodyObject);
            // case MimeType.Text:
            default:
                return new StringContent(request.BodyObject?.ToString() ?? "");
        }
    }

    public static HttpRequestMessage BuildRequest(this ApiRequest request)
    {
        var path = request.BuildPath();
        var query = request.Query.GenerateQuery();
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

        if (request.Headers.Count > 0)
        {
            // add headers
            foreach (var (key, value) in request.Headers)
                requestMsg.Headers.Add(key, value);
        }

        requestMsg.Headers.ConnectionClose = !request.KeepAlive;

        return requestMsg;
    }
}