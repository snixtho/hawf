using System;
using System.Collections.Generic;
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
}