using System.Text.RegularExpressions;
using FastEndpoints;
using GitHubLinkBot.Config;
using Octokit;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace GitHubLinkBot.EndPoints;

public class PostTelegramUpdate : Endpoint<Update>
{
    private ITelegramBotClient TelegramClient { get; }
    private IConfiguration Configuration { get; }
    private IGitHubClient GitHubClient { get; }

    public PostTelegramUpdate(ITelegramBotClient telegramClient, IConfiguration configuration, IGitHubClient gitHubClient)
    {
        TelegramClient = telegramClient;
        Configuration = configuration;
        GitHubClient = gitHubClient;
    }
    
    public override void Configure()
    {
        // use FastEndpoints config because it has not been injected yet at this point
        var config = Config!.GetSection("BotConfiguration").Get<BotConfig>();
        var botToken = config.BotToken;
        Verbs(Http.POST);
        Routes($"bot/{botToken}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Update req, CancellationToken ct)
    {
        // if message is empty or not "Message" type then don't do anything
        var msg = req.Message?.Text;
        if (req.Type != UpdateType.Message ||
            string.IsNullOrWhiteSpace(msg)) 
            return;
        
        var matches = Regex.Matches(msg, "#[0-9]+");
        if (!matches.Any()) return; // no GH issues are mentioned in the Telegram message (i.e. format #123)
        
        Console.WriteLine($"Telegram message: {msg}");
        
        var config = Configuration.GetSection("BotConfiguration").Get<BotConfig>();
        var repoOwner = config.GitHubRepoOwner;
        var repoName = config.GitHubRepoName;
        
        foreach (var m in matches)
        {
            if (string.IsNullOrWhiteSpace(m.ToString())) continue;
            Console.WriteLine($"Match found: {m}");
            var issue = Convert.ToInt32(m.ToString()!.TrimStart('#'));
            try
            {
                // check if GH issue exists, if not it will throw "NotFoundException"
                var gIssue = await GitHubClient.Issue.Get(repoOwner, repoName, issue);
                
                // not awaiting because it's unnecessary and if there's more than 1 issue number it will get to adding
                //  the next link faster
                _ = TelegramClient.SendTextMessageAsync(req.Message!.Chat.Id, gIssue.HtmlUrl, disableNotification:true, cancellationToken:ct);
            }
            catch (NotFoundException) {} // issue not found on GH
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}