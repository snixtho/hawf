using System.Runtime.Serialization;

namespace Hawf.Client.Exceptions;

[Serializable]
public class RateLimitExceededException : InvalidOperationException
{
    protected RateLimitExceededException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public TimeSpan TimeLeft { get; }
    
    public RateLimitExceededException(TimeSpan timeLeft)
    {
        TimeLeft = timeLeft;
    }
}