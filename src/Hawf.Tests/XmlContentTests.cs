using System;
using System.IO;
using System.Threading.Tasks;
using Hawf.Client.Http;
using Xunit;
using Xunit.Abstractions;

namespace Hawf.Tests;

public class XmlObject
{
    public string SomeKey { get; set; }
    public int AnotherKey { get; set; }
}

public class XmlContentTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public XmlContentTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task Serializes_Xml()
    {
        var obj = new XmlObject
        {
            SomeKey = "test",
            AnotherKey = 1234
        };

        var xmlContent = XmlContent.Create(obj);
        var stringContent = await xmlContent.ReadAsStringAsync();
        
        _testOutputHelper.WriteLine(stringContent);
        
        Assert.Equal(
            """
            <?xml version="1.0" encoding="utf-8"?>
            <XmlObject xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
              <SomeKey>test</SomeKey>
              <AnotherKey>1234</AnotherKey>
            </XmlObject>
            """,
            stringContent);
    }
}