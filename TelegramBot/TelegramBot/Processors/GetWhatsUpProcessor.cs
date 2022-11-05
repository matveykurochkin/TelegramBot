using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Internal;

namespace TelegramBot.Processors;

[TelegramCommand("Как дела?", "как дела?", "как дела", "Как дела")]
internal class GetWhatsUpProcessor : MessageProcessorBase, ITelegramMessageProcessor
{
    public async Task ProcessMessage(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        _logger.Debug("Command WhatsUp");
        var count = _random.Next(ArrDataClass.AnswWhatsUpArr.Length);
        await bot.SendTextMessageAsync(update.Message?.Chat.Id ?? 0, $"{ArrDataClass.AnswWhatsUpArr[count]}", cancellationToken: cancellationToken);
    }
}