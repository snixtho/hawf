using System.Net;
using Hawf.Attributes;
using Hawf.Client;

namespace GettingStartedTutorial;

[ApiClient("https://api.github.com")]
public class GithubApi : ApiBase<GithubApi>
{
    public GithubApi()
    {
        Configure(options =>
        {
            options.HttpHandler = new HttpClientHandler
            {
                Proxy = new WebProxy("https://127.0.0.1:5000"),
                UseProxy = true
            };
        });
    }
    
    /// <summary>
    /// Get basic information about a user.
    /// </summary>
    /// <param name="username">The user's username.</param>
    /// <returns></returns>
    public Task<GithubBasicUserInfo?> GetUserAsync(string username) =>
        GetJsonAsync<GithubBasicUserInfo>("/users/{username}", username);
}