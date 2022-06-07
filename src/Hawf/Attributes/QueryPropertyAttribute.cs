namespace Hawf.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class QueryPropertyAttribute : Attribute
{
    public string? Name { get; set; }
    public bool IntegerEnum { get; set; }
    public string ListSeparator { get; set; }

    public QueryPropertyAttribute(string? name = null, bool integerEnum = false, string listSeparator=",")
    {
        Name = name;
        IntegerEnum = integerEnum;
        ListSeparator = listSeparator;
    }
}