using Hawf.Attributes;

namespace Hawf.Tests.Models;

public enum TestEnumQueryOption
{
    One = 5,
    Two = 10
}

public class TestObjQueryOptions
{
    [QueryProperty("myoption")]
    public string MyOption { get; set; }
    [QueryProperty("MyIntOption")]
    public int MyIntOption { get; set; }
    [QueryProperty(IntegerEnum = true)]
    public TestEnumQueryOption MyEnum { get; set; }
}
