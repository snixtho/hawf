using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Hawf.Client.Http;

public class QueryParamsCollection : HttpKeyValueCollection
{
    public override string GenerateString()
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
}