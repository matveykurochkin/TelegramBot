using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Internal;

namespace TelegramBot.Processors;

[TelegramCommand("Посмотреть другие фишки 😉", "/otherfeaturesfortnite")]
public class GetOtherFeatureProcessor : MessageProcessorBase, ITelegramMessageProcessor
{
    public async Task ProcessMessage(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        _logger.Debug("Start other feature bot");
        var count = _random.Next(ArrDataClass.SticerArr.Length);
        await bot.SendTextMessageAsync(update.Message?.Chat.Id ?? 0, $"Держи список других фишек бота! {ArrDataClass.SticerArr[count]}", replyMarkup: BotButtons.OtherButtonOnBot(), cancellationToken: cancellationToken);
    }
}
