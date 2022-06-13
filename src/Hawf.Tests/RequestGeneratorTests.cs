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
}