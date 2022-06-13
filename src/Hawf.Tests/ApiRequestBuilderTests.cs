using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using Hawf.Client.Http;
using Hawf.Tests.Models;
using Xunit;

namespace Hawf.Tests;

public class ApiRequestBuilderTests : ApiRequestBuilder<ApiRequestBuilderTests>
{
    [Fact]
    public void Test_Initial_Setup()
    {
        BuilderRequestFinished();
        EnsureNewRequest();
        
        Assert.NotNull(RequestInfo.Headers);
        Assert.NotNull(RequestInfo.Query);
        Assert.NotNull(RequestInfo.PathValues);
    }

    [Fact]
    public void Test_Header_Values_Added()
    {
        BuilderRequestFinished();
        WithHeader("MyHeader", "MyValue");

        var value = RequestInfo.Headers["MyHeader"];
        
        Assert.Equal("MyValue", value);
    }
    
    [Fact]
    public void Test_Ignore_Null_Header_Value()
    {
        BuilderRequestFinished();
        WithHeader("MyHeader", null);

        Assert.DoesNotContain("MyHeader", (IDictionary<string, string>) RequestInfo.Headers);
    }

    [Fact]
    public void Test_UserAgent_Added()
    {
        BuilderRequestFinished();
        WithUserAgent("MyUserAgent");

        Assert.Contains(HttpHeader.UserAgent, (IDictionary<string, string>) RequestInfo.Headers);

        var value = RequestInfo.Headers[HttpHeader.UserAgent];

        Assert.Equal("MyUserAgent", value);
    }

    [Fact]
    public void Test_Query_Param_Added()
    {
        BuilderRequestFinished();
        WithQueryParam("MyParam", "MyValue");

        Assert.Contains("MyParam", (IDictionary<string, List<object>>) RequestInfo.Query);

        var value = RequestInfo.Query["MyParam"];

        Assert.Contains("MyValue", value);
    }

    [Fact]
    public void Test_Ignore_Null_Query_Param()
    {
        BuilderRequestFinished();
        WithQueryParam("MyParam", (string?) null);

        Assert.DoesNotContain("MyParam", (IDictionary<string, List<object>>) RequestInfo.Query);
    }

    [Fact]
    public void Test_Path_Added()
    {
        BuilderRequestFinished();
        WithPath("/my/path");
        
        Assert.Equal("/my/path", RequestInfo.Path);
    }

    [Fact]
    public void Path_Value_Added()
    {
        BuilderRequestFinished();
        WithPathValues("MyValue");
        
        Assert.Equal(new List<object>{"MyValue"}, RequestInfo.PathValues);
    }

    [Fact]
    public void BaseUrl_Set_With_String()
    {
        BuilderRequestFinished();
        WithBaseUrl("https://google.com");

        Assert.Equal(new Uri("https://google.com"), RequestInfo.BaseUrl);
    }
    
    [Fact]
    public void BaseUrl_Set_With_Uri()
    {
        BuilderRequestFinished();
        WithBaseUrl(new Uri("https://google.com"));

        Assert.Equal(new Uri("https://google.com"), RequestInfo.BaseUrl);
    }

    [Fact]
    public void Cancel_Token_Set()
    {
        var cancelToken = new CancellationToken();
        
        BuilderRequestFinished();
        WithCancelToken(cancelToken);
        
        Assert.Equal(cancelToken, RequestInfo.CancelToken);
    }

    [Fact]
    public void String_Query_Param_Added()
    {
        BuilderRequestFinished();
        WithQueryOptions(new TestObjQueryOptions {MyOption = "MyValue"});

        var value = RequestInfo.Query["myoption"][0];
        
        Assert.Equal("MyValue", value);
    }

    [Fact]
    public void Int_Query_Param_Added()
    {
        BuilderRequestFinished();
        WithQueryOptions(new TestObjQueryOptions {MyIntOption = 1337});

        var value = RequestInfo.Query["MyIntOption"][0];
        
        Assert.Equal("1337", value);
    }

    [Fact]
    public void Integer_Enum_Query_Param_Added()
    {
        BuilderRequestFinished();
        WithQueryOptions(new TestObjQueryOptions {MyEnum = TestEnumQueryOption.One});
        
        var value = RequestInfo.Query["myEnum"][0];
        
        Assert.Equal("5", value);
    }
}
