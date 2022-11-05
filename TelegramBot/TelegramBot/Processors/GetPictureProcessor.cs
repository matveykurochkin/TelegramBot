using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Internal;

namespace TelegramBot.Processors;

[TelegramCommand("/getimage", "Скинуть пикчу 🗿")]
public class GetPictureProcessor : MessageProcessorBase, ITelegramMessageProcessor
{
    public async Task ProcessMessage(ITelegramBotClient telegramBotClient, Update update, CancellationToken cancellationToken)
    {
        _logger.Debug("Get image");
        var count = _random.Next(ArrDataClass.PicArr.Length);
        await telegramBotClient.SendPhotoAsync(update.Message?.Chat.Id ?? 0, $"{ArrDataClass.PicArr[count]}", $"{ArrDataClass.SticerArr[count]}", cancellationToken: cancellationToken);
    }
}