﻿namespace Hawf.Client.Configuration;

public class ApiClientConfiguration
{
    /// <summary>
    /// Baseurl all endpoints will be called from.
    /// </summary>
    public string BaseUrl { get; set; }
    /// <summary>
    /// The default user agent to send with the HTTP request
    /// unless specified otherwise.
    /// </summary>
    public string DefaultUserAgent { get; set; } = "Hawf";
    /// <summary>
    /// Max amount of requests before rate limit kicks in.
    /// </summary>
    public int RateLimitMaxRequests { get; set; } = 5;
    /// <summary>
    /// Timespan for max amount of requests before rate limit kicks in.
    /// </summary>
    public TimeSpan RateLimitTimespan { get; set; } = TimeSpan.FromMinutes(1);
    /// <summary>
    /// Whether to enable rate limit by.
    /// </summary>
    public bool UseRateLimit { get; set; } = false;
    /// <summary>
    /// If true, the code blocks and wait for the rate limit time is over.
    /// Otherwise it throws an RateLimitExceededException.
    /// </summary>
    public bool WaitForRateLimit { get; set; } = true;
    /// <summary>
    /// Whether to attempt to keep the connection alive.
    /// </summary>
    public bool KeepAlive { get; set; } = true;
    /// <summary>
    /// Throw an error by default if the http server returns a non 200 status code.
    /// </summary>
    public bool DefaultThrowOnFail { get; set; } = true;
    /// <summary>
    /// Whether to always cache responses.
    /// </summary>
    public bool CacheResponse { get; set; } = false;
    /// <summary>
    /// The default time span to keep a cache of responses.
    /// </summary>
    public TimeSpan DefaultCacheTime { get; set; } = TimeSpan.FromMinutes(1);
}
