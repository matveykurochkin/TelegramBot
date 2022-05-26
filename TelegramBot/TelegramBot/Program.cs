using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.ReplyMarkups;


namespace TelegramBot
{
    class TelegramBot
    {
        static ITelegramBotClient bot = new TelegramBotClient("Token");

        static string[] HelloArr = new string[] { "Привет!", "привет", "Привет", "Ку", "ghbdtn", "ку", "дороу", "Дороу" };
        static string[] WhatsUpArr = new string[] { "Как дела?", "как дела?", "как дела" };

        public static IReplyMarkup ButtonOnTGbot()
        {
            return new ReplyKeyboardMarkup(new[]
                        {
                new[]
                {
                    new KeyboardButton("Привет!"),
                    new KeyboardButton("Как дела?")
                },
                new[]
                {
                    new KeyboardButton("Скинь картинку"),
                }
            });
        }

        public static async Task updateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;

                var hashHelloArr = new HashSet<string>(HelloArr);
                var hashWhatsUpArr = new HashSet<string>(WhatsUpArr);

                StreamWriter DataBase = new StreamWriter("E:\\DataBase.txt", true);
                DataBase.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
                DataBase.Close();

              //  bot.SendTextMessageAsync(message.Chat, "", replyMarkup: ButtonOnTGbot());

                if (hashHelloArr.Contains(message.Text))
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Привет " + message.From.FirstName + "!");
                    return;
                }
                else if (message.Text == "/start")
                {
                    bot.SendTextMessageAsync(message.Chat.Id, "Смотри что я умею! :)", replyMarkup: ButtonOnTGbot());
                    return;
                }
                else if (hashWhatsUpArr.Contains(message.Text))
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Хорошо, у тебя как?");
                    return;
                }
                else if (message.Text == "/getimage"|| message.Text == "Скинь картинку")
                {
                    await bot.SendPhotoAsync(message.Chat.Id, "https://avatarko.ru/img/kartinka/33/Star_Wars_Darth_Vader_32632.jpg", "Смотри, это Дарт Вейдер!");
                    return;
                }
                await botClient.SendTextMessageAsync(message.Chat.Id, "Я не знаю как ответить на это :(");
            }
        }

        public static async Task errorHandler(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }


        static void Main(string[] args)
        {
            Console.WriteLine("\tБот " + bot.GetMeAsync().Result.FirstName + " успешно запущен!");
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions { };
            bot.StartReceiving(updateHandler, errorHandler, receiverOptions, cancellationToken);
            Console.ReadLine();
        }
    }
}
