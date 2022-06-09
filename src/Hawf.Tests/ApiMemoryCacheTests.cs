using System;
using System.Threading.Tasks;
using Hawf.Utils;
using Xunit;

namespace Hawf.Tests;

public class ApiMemoryCacheTests
{
    [Fact]
    public async Task Value_Added()
    {
        var cache = new ApiMemoryCache();

        await cache.Set("MyKey", "MyValue");
        var value = await cache.Get<string>("MyKey");

        Assert.Equal("MyValue", value);
    }

    [Fact]
    public async Task Value_Expires_Correctly()
    {
        var cache = new ApiMemoryCache();

        await cache.Set("MyKey", "MyValue", TimeSpan.FromMilliseconds(1));
        await Task.Delay(2);
        var exists = await cache.Exists("MyKey");
        
        Assert.False(exists);
    }

    [Fact]
    public async Task Value_Not_Expired_Within_Timeframe()
    {
        var cache = new ApiMemoryCache();

        await cache.Set("MyKey", "MyValue", TimeSpan.FromDays(100));
        var exists = await cache.Exists("MyKey");
        
        Assert.True(exists);
    }

    [Fact]
    public async Task Cache_Object_Does_Not_Exist()
    {
        var cache = new ApiMemoryCache();

        var exists = await cache.Exists("MyKey");
        
        Assert.False(exists);
    }
    
    [Fact]
    public async Task Cache_Object_Exists()
    {
        var cache = new ApiMemoryCache();

        await cache.Set("MyKey", "MyValue");
        var exists = await cache.Exists("MyKey");
        
        Assert.True(exists);
    }

    [Fact]
    public async Task Null_Value_Allowed()
    {
        var cache = new ApiMemoryCache();
        
        await cache.Set<object?>("MyKey", null);
        var value = await cache.Get<object?>("MyKey");
        
        Assert.Null(value);
    }

    [Fact]
    public async Task CacheObject_Is_Unset_Properly()
    {
        var cache = new ApiMemoryCache();

        await cache.Set("MyKey", "MyValue");
        await cache.Unset("MyKey");

        var exists = await cache.Exists("MyKey");
        
        Assert.False(exists);
    }
}