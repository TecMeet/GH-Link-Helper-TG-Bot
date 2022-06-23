using System.Text.RegularExpressions;
using FastEndpoints;
using GitHubLinkBot.Config;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace GitHubLinkBot.EndPoints;

public class PostTelegramUpdate : Endpoint<Update>
{
    public override void Configure()
    {
        var botToken = Config?.GetSection("BotConfiguration").Get<BotConfig>().BotToken;
        Verbs(Http.POST);
        Routes($"bot/{botToken}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Update req, CancellationToken ct)
    {
        Console.WriteLine("HandleAsync");
        var msg = req.Message?.Text;
        if (req.Type != UpdateType.Message ||
            string.IsNullOrWhiteSpace(msg)) 
            return;
        
        Console.WriteLine($"Telegram message: {msg}");

        var matches = Regex.Matches(msg, "#[0-9]+");
        foreach (var m in matches)
        {
            Console.WriteLine($"Match found: {m}");
        }
    }
}