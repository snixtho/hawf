using System.Net;
using System.Xml.Serialization;

namespace Hawf.Client.Http;

public class XmlContent : HttpContent
{
    private readonly XmlSerializer _serializer;
    private object? _value;

    private XmlContent(object? inputValue)
    {
        _value = inputValue;

        if (inputValue == null)
            throw new ArgumentNullException(nameof(inputValue));

        _serializer = new XmlSerializer(inputValue.GetType());
    }
    
    protected override Task SerializeToStreamAsync(Stream targetStream, TransportContext? context)
    {
        _serializer.Serialize(targetStream, _value);
        return Task.CompletedTask;
    }

    protected override bool TryComputeLength(out long length)
    {
        length = 0;
        return false;
    }

    public static XmlContent Create(object? contentObject)
    {
        return new XmlContent(contentObject);
    }
}