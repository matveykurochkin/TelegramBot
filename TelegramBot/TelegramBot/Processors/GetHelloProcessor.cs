using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Internal;

namespace TelegramBot.Processors;

[TelegramCommand("Привет!", "привет", "Привет", "Ку", "ghbdtn", "ку", "дороу", "Дороу")]
public class GetHelloProcessor : MessageProcessorBase, ITelegramMessageProcessor
{
    public async Task ProcessMessage(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        _logger.Debug("Command hello");
        var count = _random.Next(ArrDataClass.AnswHelloArr.Length);
        await bot.SendTextMessageAsync(update.Message?.Chat.Id ?? 0, $"{ArrDataClass.AnswHelloArr[count]} {update.Message?.From?.FirstName}! 🙂", cancellationToken: cancellationToken);
    }
}
