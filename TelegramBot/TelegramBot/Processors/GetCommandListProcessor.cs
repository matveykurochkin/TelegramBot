using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Internal;

namespace TelegramBot.Processors;

[TelegramCommand("/command", "Список команд")]
public class GetCommandListProcessor : MessageProcessorBase, ITelegramMessageProcessor
{
    public async Task ProcessMessage(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        _logger.Debug("Request list of commands");
        await bot.SendTextMessageAsync(update.Message?.Chat.Id ?? 0, $"{ArrDataClass.CommandArr[0]}", cancellationToken: cancellationToken);
    }
}
