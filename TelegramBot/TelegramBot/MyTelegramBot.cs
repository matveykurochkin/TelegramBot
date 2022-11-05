using NLog;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot;

class MyTelegramBot
{
    // ReSharper disable once InconsistentNaming
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    private readonly ITelegramBotClient _telegramBotClient;
    private readonly MessageProcessorFactory _processorFactory = new();

    // ReSharper disable once InconsistentNaming
    protected static readonly Random _random = new();

    public MyTelegramBot(ITelegramBotClient telegramBotClient)
    {
        _telegramBotClient = telegramBotClient;
    }

    internal async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        _logger.Debug($"Update received: {update}");

        if (update.Type != Telegram.Bot.Types.Enums.UpdateType.Message) return;

        var message = update.Message;
        _logger.Info($"Пользователь {message?.From?.FirstName} {message?.From?.LastName} написал боту данное сообщение: {message?.Text}\nid Пользователя: {message?.From?.Id}");

        var processor = _processorFactory.GetProcessor(message?.Text);
        if (processor != null)
        {
            await processor.ProcessMessage(bot, update, cancellationToken);
            return;
        }

        var count = _random.Next(ArrDataClass.AnswOther.Length);
        await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"{ArrDataClass.AnswOther[count]}", cancellationToken: cancellationToken);
    }

    internal Task ErrorHandler(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        _logger.Error(exception, "Error received in telegram bot");
        return Task.CompletedTask;
    }
}