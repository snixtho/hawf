using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hawf.Client;
using Hawf.Client.Http;
using Hawf.Utils;
using Xunit;

namespace Hawf.Tests;

public class RequestGeneratorTests
{
    [Fact]
    public void Path_Generated_Correctly()
    {
        var request = new ApiRequest
        {
            Path = "/my/path/{value}",
            PathValues = new List<object>() {"MyValue"}
        };

        var generated = request.BuildPath();

        Assert.Equal("/my/path/MyValue", generated);
    }

    [Fact]
    public void Path_Encodes_Value()
    {
        var request = new ApiRequest
        {
            Path = "/my/path/{value}",
            PathValues = new List<object>() {"My Value"}
        };

        var generated = request.BuildPath();

        Assert.Equal("/my/path/My+Value", generated);
    }

    [Fact]
    public void Query_Generated_Correctly()
    {
        var request = new ApiRequest
        {
            Query = new QueryParamsCollection
            {
                { "myParam1", "myValue1" },
                { "myParam2", 123 }
            }
        };

        var query = request.Query.GenerateQuery();
        
        Assert.Equal("?myParam1=myValue1&myParam2=123", query);
    }

    [Fact]
    public void Query_Values_Are_Encoded()
    {
        var request = new ApiRequest
        {
            Query = new QueryParamsCollection
            {
                { "myParam1", "my value" }
            }
        };
        
        var query = request.Query.GenerateQuery();
        
        Assert.Equal("?myParam1=my+value", query);
    }

    [Fact]
    public async Task Content_Body_Text_Content_Generated_Correctly()
    {
        var request = new ApiRequest
        {
            BodyObject = "My String Value",
            MimeType = MimeType.Text
        };

        var body = request.CreateBodyContent();
        
        Assert.IsType<StringContent>(body);

        var stringBody = body as StringContent;
        var value = await stringBody?.ReadAsStringAsync()!;
        
        Assert.Equal("My String Value", value);
    }

    [Fact]
    public async Task Content_Body_Json_Content_Generated_Correctly()
    {
        var request = new ApiRequest
        {
            BodyObject = new { MyKey = "MyValue" },
            MimeType = MimeType.Json
        };

        var body = request.CreateBodyContent();
        
        Assert.IsType<JsonContent>(body);

        var stringBody = body as JsonContent;
        var value = await stringBody?.ReadAsStringAsync()!;
        
        Assert.Equal("{\"myKey\":\"MyValue\"}", value);
    }

    [Fact]
    public void BuildRequest_Throws_On_Null_BaseAddress()
    {
        var request = new ApiRequest
        {
            Path = "",
            PathValues = new List<object>(),
            Query = new QueryParamsCollection()
        };

        Assert.Throws<InvalidOperationException>(() =>
        {
            request.BuildRequest();
        });
    }

    [Fact]
    public void BuildPath_Throws_On_Invalid_Number_Of_Path_Values()
    {
        var request = new ApiRequest
        {
            Path = "/{some}/{value}",
            PathValues = new List<object>
            {
                "myValue"
            }
        };

        Assert.Throws<InvalidOperationException>(() =>
        {
            request.BuildPath();
        });
    }

    [Fact]
    public void BuildRequest_Path_Set()
    {
        var request = new ApiRequest
        {
            Path = "/{some}",
            PathValues = new List<object>
            {
                "MyValue"
            },
            BaseUrl = new Uri("https://google.com")
        };

        var httpRequest = request.BuildRequest();
        
        Assert.Equal("https://google.com/MyValue", httpRequest.RequestUri?.AbsoluteUri);
    }

    [Fact]
    public void BuildRequest_Query_Set()
    {
        var request = new ApiRequest
        {
            Path = "",
            PathValues = new List<object>(),
            BaseUrl = new Uri("https://google.com"),
            Query = new QueryParamsCollection()
            {
                {"key", "value"}
            }
        };

        var httpRequest = request.BuildRequest();
        
        Assert.Equal("https://google.com/?key=value", httpRequest.RequestUri?.AbsoluteUri);
    }

    [Fact]
    public async Task BuildRequest_Body_Set()
    {
        var request = new ApiRequest
        {
            Path = "",
            PathValues = new List<object>(),
            BaseUrl = new Uri("https://google.com"),
            BodyObject = "content value",
            MimeType = MimeType.Text
        };

        var httpRequest = request.BuildRequest();
        var value = await httpRequest.Content?.ReadAsStringAsync()!;
        
        Assert.Equal("content value", value);
    }

    [Fact]
    public void BuildRequest_Full_Url_Set()
    {
        var request = new ApiRequest
        {
            Path = "/{some}",
            PathValues = new List<object>
            {
                "MyValue"
            },
            BaseUrl = new Uri("https://google.com"),
            Query = new QueryParamsCollection()
            {
                {"key", "value"}
            }
        };

        var httpRequest = request.BuildRequest();
        
        Assert.Equal("https://google.com/MyValue?key=value", httpRequest.RequestUri?.AbsoluteUri);
    }


    [Fact]
    public void BuildRequest_Avoids_Double_Slashes()
    {
        var request = new ApiRequest
        {
            Path = "/path",
            PathValues = new List<object>(),
            BaseUrl = new Uri("https://google.com/"),
        };
        
        var httpRequest = request.BuildRequest();
        
        Assert.Equal("https://google.com/path", httpRequest.RequestUri?.AbsoluteUri);
    }

    [Fact]
    public void BuildRequest_Sets_Correct_Method()
    {
        var request = new ApiRequest
        {
            PathValues = new List<object>(),
            BaseUrl = new Uri("https://google.com/"),
            Method = HttpMethod.Post
        };

        var httpRequest = request.BuildRequest();
        
        Assert.Equal(HttpMethod.Post, httpRequest.Method);
    }

    [Fact]
    public async Task BuildRequest_Sets_Correct_Body()
    {
        var request = new ApiRequest
        {
            PathValues = new List<object>(),
            BaseUrl = new Uri("https://google.com/"),
            BodyObject = "my value",
            MimeType = MimeType.Text
        };

        var httpRequest = request.BuildRequest();
        var content = await httpRequest.Content?.ReadAsStringAsync()!;
        
        Assert.Equal("my value", content);
    }

    [Fact]
    public void BuildRequest_Adds_Headers()
    {
        var request = new ApiRequest
        {
            PathValues = new List<object>(),
            BaseUrl = new Uri("https://google.com/"),
            Headers = new Dictionary<string, string>()
            {
                {"header1", "value1"},
                {HttpHeader.Authorization, "Bearer mytoken"},
                {HttpHeader.UserAgent, "my user agent"}
            }
        };

        var httpRequest = request.BuildRequest();
        
        Assert.Equal("Bearer", httpRequest.Headers.Authorization?.Scheme);
        Assert.Equal("mytoken", httpRequest.Headers.Authorization?.Parameter);
        
        Assert.Equal("my user agent", httpRequest.Headers.UserAgent.ToString());

        var header1Value = httpRequest.Headers.GetValues("header1").First();
        
        Assert.Equal("value1", header1Value);
    }

    [Fact]
    public void BuildRequest_Sets_KeepAlive()
    {
        var request = new ApiRequest
        {
            PathValues = new List<object>(),
            BaseUrl = new Uri("https://google.com/"),
            KeepAlive = true
        };
        
        var httpRequest = request.BuildRequest();
        
        Assert.False(httpRequest.Headers.ConnectionClose);
    }
}