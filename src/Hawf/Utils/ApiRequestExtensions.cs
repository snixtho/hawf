using System.Text.RegularExpressions;
using Hawf.Client;

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

    public static HttpRequestMessage BuildRequest(this ApiRequest request)
    {
        var path = request.BuildPath();
        var query = request.Query.GenerateQuery();

        var urlBuilder = new UriBuilder(request.BaseUrl)
        {
            Query = query
        };

        urlBuilder.Path += path;

        var requestMsg = new HttpRequestMessage
        {
            RequestUri = urlBuilder.Uri,
            Method = request.Method
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