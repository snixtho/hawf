namespace Hawf.Client.Exceptions;

public class RateLimitExceededException : InvalidOperationException
{
    public TimeSpan TimeLeft { get; }
    
    public RateLimitExceededException(TimeSpan timeLeft)
    {
        TimeLeft = timeLeft;
    }
}