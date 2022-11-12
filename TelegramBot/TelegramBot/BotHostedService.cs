using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NLog;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using TelegramBot.Configuration;

namespace TelegramBot;
internal class BotHostedService : IHostedService
{
    private readonly TelegramOptions _opts;

    public BotHostedService(IOptions<TelegramOptions> opts)
    {
        _opts = opts.Value;
    }

    // ReSharper disable once InconsistentNaming
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    private ITelegramBotClient? _bot;
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_opts.Token))
            throw new ArgumentNullException(nameof(_opts.Token), "Token must not be empty");
        _bot = new TelegramBotClient(_opts.Token);
        var tgBot = new MyTelegramBot(_bot);
        var user = await _bot.GetMeAsync(cancellationToken);
        _logger.Info($"Бот {@user} успешно запущен!", user);
        var receiverOptions = new ReceiverOptions();
        _logger.Debug("bot starting receive message");
        _bot.StartReceiving(tgBot.UpdateHandler, tgBot.ErrorHandler, receiverOptions, CancellationToken.None);
        _logger.Debug("bot started to receive messages");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.Info("Stoping telegram bot");
        return Task.CompletedTask;
    }
}
