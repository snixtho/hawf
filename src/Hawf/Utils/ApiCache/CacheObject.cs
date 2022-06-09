namespace Hawf.Utils;

public class CacheObject
{
    public string Key { get; set; } = string.Empty;
    public object? Value { get; set; } = null;
    public TimeSpan? LifeTime { get; set; } = TimeSpan.Zero;
    public DateTime Registered { get; set; } = DateTime.Now;
}