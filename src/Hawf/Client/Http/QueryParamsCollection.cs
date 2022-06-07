using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Hawf.Client.Http;

public class QueryParamsCollection : Dictionary<string, List<object>>
{
    public string GenerateQuery()
    {
        var query = "";
        var first = true;

        foreach (var (key, values) in this)
        {
            var arrId = values.Count > 1 ? "[]" : "";

            foreach (var value in values)
            {
                var valueEncoded = HttpUtility.UrlEncode(value.ToString());

                if (first)
                {
                    query += '?';
                    first = false;
                }
                else
                    query += '&';
                
                query += $"{key}{arrId}={valueEncoded}";
            }
        }

        return query;
    }

    /// <summary>
    /// Add a query parameter.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="FormatException"></exception>
    public void Add<T>(string name, T value)
    {
        if (name.Trim().Length == 0)
            throw new FormatException("Name cannot be empty.");

        if (Regex.IsMatch(name, "[\\:\\/\\?\\#\\[\\]\\@]"))
            throw new FormatException("Name has invalid characters.");
        
        if (ContainsKey(name))
            this[name].Add(value);
        else
            base.Add(name, new List<object> {value});
    }
}