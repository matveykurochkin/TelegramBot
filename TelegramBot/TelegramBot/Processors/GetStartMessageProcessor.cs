using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Internal;

namespace TelegramBot.Processors;

[TelegramCommand("/start", "Старт", "Назад ⬅")]
public class GetStartMessageProcessor : MessageProcessorBase, ITelegramMessageProcessor
{
    public async Task ProcessMessage(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        _logger.Debug("Start");
        var count = _random.Next(ArrDataClass.SticerArr.Length);
        await bot.SendTextMessageAsync(update.Message?.Chat.Id ?? 0, $"Смотри что я умею! {ArrDataClass.SticerArr[count]}", replyMarkup: BotButtons.MainButtonOnBot(), cancellationToken: cancellationToken);
    }
}