using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Hawf.Client.Http;
using Hawf.Tests.Models;
using Xunit;
using Xunit.Abstractions;

namespace Hawf.Tests;

public class UrlEncodedFormDataCollectionTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public UrlEncodedFormDataCollectionTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Generates_Correct_Data_String()
    {
        var form = new FormDataCollection();
        
        form.Add("MyKey", 1);
        form.Add("SecondKey", "test");
        form.Add("ThirdKey", "my value");

        var generated = form.GenerateString();
        
        Assert.Equal("MyKey=1&SecondKey=test&ThirdKey=my+value", generated);
    }

    [Fact]
    public async Task MultipartFormDataContent_Generation_Includes_String()
    {
        var form = new FormDataCollection();

        form.Add("MyStringKey", "My Value");

        var content = form.ToMultipartFormContent();
        var stringContent = (StringContent)content.First();
        var value = await stringContent.ReadAsStringAsync();

        Assert.Equal("My Value", value);
    }

    [Fact]
    public async Task MultipartFormDataContent_Generation_Includes_Bytes()
    {
        var form = new FormDataCollection();

        form.Add("MyStringKey", new byte[] {1, 2, 3});

        var content = form.ToMultipartFormContent();
        var stringContent = (ByteArrayContent)content.First();
        var value = await stringContent.ReadAsByteArrayAsync();

        Assert.Equal(new byte[] {1, 2, 3}, value);
    }

    [Fact]
    public async Task MultipartFormDataContent_Generation_Includes_Filestream()
    {
        var form = new FormDataCollection();

        form.Add("MyStringKey", File.Open("Data/example.txt", FileMode.Open));

        var content = form.ToMultipartFormContent();
        var stringContent = (StreamContent)content.First();
        var value = await stringContent.ReadAsStringAsync();

        Assert.Equal("Hello There", value);
    }
}