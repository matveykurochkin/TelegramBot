using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;


namespace TelegramBot
{
    class TelegramBot
    {
        static ITelegramBotClient bot = new TelegramBotClient("Token");
        public static async Task updateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;

                StreamWriter DataBase = new StreamWriter("E:\\DataBase.txt", true);
                DataBase.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
                DataBase.Close();

                if (message.Text == "Привет" || message.Text == "привет")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Привет!");
                    return;
                }
                else if (message.Text == "Как дела?" || message.Text == "как дела?" || message.Text == "как дела")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Хорошо, у тебя как?");
                    return;
                }
                await botClient.SendTextMessageAsync(message.Chat, "Я не знаю как ответить на это :(");
            }
        }

        public static async Task errorHandler(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Бот " + bot.GetMeAsync().Result.FirstName + " успешно запущен!");
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions { };
            bot.StartReceiving(updateHandler, errorHandler, receiverOptions, cancellationToken);
            Console.ReadLine();
        }
    }
}
