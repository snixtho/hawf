using System;
using Hawf.Client.Http;
using Xunit;

namespace Hawf.Tests;

public class QueryParamsCollectionTests
{
    [Fact]
    public void Test_Single_Query_Item()
    {
        var queries = new QueryParamsCollection();
        queries.Add("mypar", "myvalue");

        var query = queries.GenerateQuery();
        
        Assert.Equal("?mypar=myvalue", query);
    }

    [Fact]
    public void Test_Empty_Name()
    {
        var queries = new QueryParamsCollection();

        Assert.Throws<FormatException>(() =>
        {
            queries.Add("", "value");
        });
    }

    [Theory]
    [InlineData("na:e")]
    [InlineData("n?me")]
    [InlineData("n/me")]
    [InlineData("n#me")]
    [InlineData("n[me")]
    [InlineData("na]e")]
    [InlineData("n@me")]
    public void Test_Incorrect_Characters(string name)
    {
        var queries = new QueryParamsCollection();

        Assert.Throws<FormatException>(() =>
        {
            queries.Add(name, "value");
        });
    }

    [Fact]
    public void Test_Supports_Array()
    {
        var queries = new QueryParamsCollection();
        
        queries.Add("myKey", "value1");
        queries.Add("myKey", "value2");
        queries.Add("myKey", "value3");

        var query = queries.GenerateQuery();
        
        Assert.Equal("?myKey[]=value1&myKey[]=value2&myKey[]=value3", query);
    }

    [Fact]
    public void Test_Multiple_Params()
    {
        var queries = new QueryParamsCollection();
        
        queries.Add("myKey1", "value");
        queries.Add("myKey2", "value");
        queries.Add("myKey3", "value");

        var query = queries.GenerateQuery();
        
        Assert.Equal("?myKey1=value&myKey2=value&myKey3=value", query);
    }
}