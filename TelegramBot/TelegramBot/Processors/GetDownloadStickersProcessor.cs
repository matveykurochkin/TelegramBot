using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Internal;

namespace TelegramBot.Processors;

[TelegramCommand( "/downloadstickers", "Загрузить стикеры 🎁")]
internal class GetDownloadStickersProcessor : MessageProcessorBase, ITelegramMessageProcessor
{
    public async Task ProcessMessage(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        _logger.Debug("Download stickers refernce");
        await bot.SendTextMessageAsync(update.Message?.Chat.Id ?? 0, $"Ссылка для загрузки стикеров: https://t.me/addstickers/BusyaEveryDay", cancellationToken: cancellationToken);
        await bot.SendPhotoAsync(update.Message?.Chat.Id ?? 0, $"https://ibb.co/Z8fbXGH", $"QR code для загрузки стикеров!", cancellationToken: cancellationToken);
    }
}
