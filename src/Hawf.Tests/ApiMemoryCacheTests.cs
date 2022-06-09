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
}