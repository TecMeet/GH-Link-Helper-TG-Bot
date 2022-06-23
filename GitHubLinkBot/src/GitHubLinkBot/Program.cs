using FastEndpoints;
using GitHubLinkBot.Config;
using GitHubLinkBot.Services;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);
var botConfig = builder.Configuration.GetSection("BotConfiguration").Get<BotConfig>();

builder.Services.AddFastEndpoints(config: builder.Configuration);

builder.Services.AddHostedService<ConfigureWebhook>();

builder.Services.AddHttpClient("tg").AddTypedClient<ITelegramBotClient>(client =>
    new TelegramBotClient(botConfig.BotToken, client));

var app = builder.Build();

app.UseFastEndpoints(config =>
{
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
