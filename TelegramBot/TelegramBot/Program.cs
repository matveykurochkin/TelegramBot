using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

namespace TelegramBot;

static class Program
{
    static void Main(string[] args)
    {
        ITelegramBotClient bot = new TelegramBotClient("");
        var tgBot = new MyTelegramBot(bot);

        Console.WriteLine($"\tБот {bot.GetMeAsync().Result.FirstName} успешно запущен!");
        var receiverOptions = new ReceiverOptions();
        bot.StartReceiving(tgBot.UpdateHandler, tgBot.ErrorHandler, receiverOptions, CancellationToken.None);
        Console.ReadLine();
    }
}
