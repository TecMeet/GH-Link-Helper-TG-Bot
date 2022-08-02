using FastEndpoints;
using GitHubLinkBot.Config;
using GitHubLinkBot.Services;
using Octokit;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);
var botConfig = builder.Configuration.GetSection("BotConfiguration").Get<BotConfig>();

// need to set configuration here because it can't be injected yet
builder.Services.AddFastEndpoints(config: builder.Configuration);

// configure webhook for Telegram bot on app Startup, and unregister hook on app shutdown
builder.Services.AddHostedService<ConfigureTelegramWebhook>();

// add GitHub client
builder.Services.AddScoped<IGitHubClient>(_ => new GitHubClient(new ProductHeaderValue("GH-Link-Helper-Telegram-Bot")));

// add Telegram client
builder.Services.AddHttpClient("tg").AddTypedClient<ITelegramBotClient>(client =>
    new TelegramBotClient(botConfig.BotToken, client));

var app = builder.Build();

app.MapGet("/site-status", () => "Site is working");

app.UseFastEndpoints(config =>
{
    // set up for Newtonsoft serializer because Telegram does not yet support the current default System.Text.Json
    config.RequestDeserializer = async (req, tDto, jCtx, ct) =>
    {
        using var reader = new StreamReader(req.Body);
        return Newtonsoft.Json.JsonConvert.DeserializeObject(await reader.ReadToEndAsync(), tDto);
    };
    config.ResponseSerializer = (rsp, dto, cType, jCtx, ct) =>
    {
        rsp.ContentType = cType;
        return rsp.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(dto), ct);
    };
});

app.Run();
