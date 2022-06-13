using System.Collections.Generic;
using Hawf.Client;
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
    
    
}