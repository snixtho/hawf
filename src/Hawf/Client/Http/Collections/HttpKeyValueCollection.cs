using System.Text.RegularExpressions;

namespace Hawf.Client.Http;

public abstract class HttpKeyValueCollection : Dictionary<string, List<object>>
{
    public abstract string GenerateString();

    /// <summary>
    /// Add a query parameter.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="FormatException"></exception>
    public void Add<T>(string name, T? value)
    {
        if (value == null)
            throw new ArgumentException("Value cannot be null.");
        
        if (name.Trim().Length == 0)
            throw new ArgumentException("Name cannot be empty.");

        if (Regex.IsMatch(name, "[\\:\\/\\?\\#\\[\\]\\@]"))
            throw new FormatException("Name has invalid characters.");
        
        if (ContainsKey(name))
            this[name].Add(value);
        else
            base.Add(name, new List<object> {value});
    }
}
