using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot;
public interface ITelegramMessageProcessor
{
    Task ProcessMessage(ITelegramBotClient bot, Update update, CancellationToken cancellationToken);
}