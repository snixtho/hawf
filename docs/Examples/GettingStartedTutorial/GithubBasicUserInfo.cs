using System.Text.Json.Serialization;

namespace GettingStartedTutorial;

public class GithubBasicUserInfo
{
    public string? Login { get; set; }
    public long Id { get; set; }
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    [JsonPropertyName("public_repos")]
    public int PublicRepos { get; set; }
    [JsonPropertyName("public_gists")]
    public int PublicGists { get; set; }
    public int Followers { get; set; }
    public int Following { get; set; }
    [JsonPropertyName("avatar_url")]
    public string? AvatarUrl { get; set; }
}