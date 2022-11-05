using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Internal;

namespace TelegramBot.Processors;

[TelegramCommand("/getsticer", "Скинуть стикос 😉")]
public class GetStickerProcessor : MessageProcessorBase, ITelegramMessageProcessor
{
    public async Task ProcessMessage(ITelegramBotClient telegramBotClient, Update update, CancellationToken cancellationToken)
    {
        _logger.Debug("Get sticker");
        var count = _random.Next(ArrDataClass.SticerArr.Length);
        await telegramBotClient.SendTextMessageAsync(update.Message?.Chat.Id ?? 0, $"{ArrDataClass.SticerArr[count]}", cancellationToken: cancellationToken);
    }
}