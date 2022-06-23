using GitHubLinkBot.Config;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace GitHubLinkBot.Services;

public class ConfigureWebhook : IHostedService
{
    private readonly BotConfig _botConfig;
    private readonly IServiceProvider _services;
    public ConfigureWebhook(IConfiguration config, IServiceProvider services)
    {
        _services = services;
        _botConfig = config.GetSection("BotConfiguration").Get<BotConfig>();
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
        
        var webhookAddress = @$"{_botConfig.BotHostAddress}/bot/{_botConfig.BotToken}";
        Console.WriteLine($"Setting webhook: {webhookAddress}");
        await botClient.SetWebhookAsync(
            url: webhookAddress,
            allowedUpdates: new List<UpdateType>
            {
                UpdateType.Message
            },
            dropPendingUpdates: _botConfig.DropPendingUpdates ?? false,
            cancellationToken: cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

        // Remove webhook upon app shutdown
        Console.WriteLine("Removing webhook");
        await botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
    }
}