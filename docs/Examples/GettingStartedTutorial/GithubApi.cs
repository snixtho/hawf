using Hawf.Attributes;
using Hawf.Client;

namespace GettingStartedTutorial;

[ApiClient("https://api.github.com")]
public class GithubApi : ApiBase<GithubApi>
{
    /// <summary>
    /// Get basic information about a user.
    /// </summary>
    /// <param name="username">The user's username.</param>
    /// <returns></returns>
    public Task<GithubBasicUserInfo?> GetUserAsync(string username) =>
        GetJsonAsync<GithubBasicUserInfo>("/users/{username}", username);
}