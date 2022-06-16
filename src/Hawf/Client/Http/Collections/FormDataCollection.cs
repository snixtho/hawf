namespace Hawf.Client.Http;

public class FormDataCollection : QueryParamsCollection
{
    public override string GenerateString() =>
        base.GenerateString().Substring(1);

    public IEnumerable<KeyValuePair<string, string>> ToUrlEncodedCollection()
    {
        return this.Select(e =>
        {
            return new KeyValuePair<string, string>(e.Key, e.Value?.ToString() ?? "null");
        });
    }

    public MultipartFormDataContent ToMultipartFormContent()
    {
        var form = new MultipartFormDataContent();
        foreach (var (key, values) in this)
        {
            var value = values[0];
            var type = value.GetType();
            if (typeof(FileStream).IsAssignableFrom(type))
                form.Add(new StreamContent((FileStream)value), key);
            else if (typeof(byte[]).IsAssignableFrom(type))
                form.Add(new ByteArrayContent((byte[]) value), key);
            else
                form.Add(new StringContent(value?.ToString() ?? "null"), key);
        }

        return form;
    }
}