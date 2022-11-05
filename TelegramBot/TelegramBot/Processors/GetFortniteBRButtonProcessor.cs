using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Internal;

namespace TelegramBot.Processors;

[TelegramCommand("Fortnite меню 👾", "/menufortnite")]
// ReSharper disable once InconsistentNaming
internal class GetFortniteBRButtonProcessor : MessageProcessorBase, ITelegramMessageProcessor
{
    public async Task ProcessMessage(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {

        _logger.Debug("Start");
        var count = _random.Next(ArrDataClass.SticerArr.Length);
        await bot.SendTextMessageAsync(update.Message?.Chat.Id ?? 0, $"Держи функции Королевской Битвы Fortnite! {ArrDataClass.SticerArr[count]}", replyMarkup: BotButtons.ButtonBR(), cancellationToken: cancellationToken);
    }
}
