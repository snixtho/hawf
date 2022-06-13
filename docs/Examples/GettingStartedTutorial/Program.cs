
using GettingStartedTutorial;

var api = new GithubApi();

var user = await api.GetUserAsync("snixtho");

Console.WriteLine($"snixtho created their account at {user?.CreatedAt}, has {user?.Followers} follower(s) and made {user?.PublicRepos} public repositories.");
