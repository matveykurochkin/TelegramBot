
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Internal;

namespace TelegramBot.Processors;

[TelegramCommand("Посмотреть погоду ⛅", "/cityweather")]
public class GetWeatherInCityButtonsProcessor : MessageProcessorBase, ITelegramMessageProcessor
{
    public async Task ProcessMessage(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        _logger.Debug("Get weather");
        await bot.SendTextMessageAsync(update.Message?.Chat.Id ?? 0, $"Узнать погоду можно в текущих городах: ", replyMarkup: BotButtons.ButtonWeather(), cancellationToken: cancellationToken);
    }
}
