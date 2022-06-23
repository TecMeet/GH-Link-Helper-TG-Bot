namespace GitHubLinkBot.Config;

public class BotConfig
{
    public string BotToken { get; init; } = null!;
    public string BotHostAddress { get; init; } = null!;
    public string GitHubRepoOwner { get; init; } = null!;
    public string GitHubRepoName { get; init; } = null!;
    public bool? DropPendingUpdates { get; init; }
}