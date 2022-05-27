using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.ReplyMarkups;
using System.Net;
using Newtonsoft.Json;

namespace TelegramBot
{

    class TelegramBot
    {
        public TelegramBot main { get; set; }
        public string Name { get; set; }
        public float Temp { get; set; }

        static ITelegramBotClient bot = new TelegramBotClient("Token");

        static string[] HelloArr = new string[] { "Привет!", "привет", "Привет", "Ку", "ghbdtn", "ку", "дороу", "Дороу" };
        static string[] WhatsUpArr = new string[] { "Как дела?", "как дела?", "как дела" };
        static string[] WeatherCity = new string[] { "Владимир", "Москва", "Санкт-Петербург", "Головино", "Боголюбово", "Дубай", "Гусь-Хрустальный" };

        static string nameofCity;
        static float tempOfCity;
        public static IReplyMarkup ButtonOnTGbot()
        {

            var TGbutton = new ReplyKeyboardMarkup(new[]
                        {
                new[]
                {
                    new KeyboardButton("Привет!"),
                },
                new[]
                {
                    new KeyboardButton("Как дела?")
                },
                new[]
                {
                    new KeyboardButton("Скинь картинку"),
                },
                new[]
                {
                    new KeyboardButton("Посмотреть погоду\U0001F325"),
                }
            });
            TGbutton.ResizeKeyboard = true;
            return TGbutton;
        }

        public static async Task updateHandler(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;

                Console.WriteLine("Пользователь " + message.From.FirstName + " " + message.From.LastName + " написал боту данное сообщение: " + message.Text);
                Console.WriteLine("\tid Пользователя: " + message.From.Id);

                var hashHelloArr = new HashSet<string>(HelloArr);
                var hashWhatsUpArr = new HashSet<string>(WhatsUpArr);
                var hashWeatherCity = new HashSet<string>(WeatherCity);

                StreamWriter DataBase = new StreamWriter("E:\\DataBase.txt", true);
                DataBase.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
                DataBase.Close();


                if (hashHelloArr.Contains(message.Text))
                {
                    await bot.SendTextMessageAsync(message.Chat.Id, "Привет " + message.From.FirstName + "! \U0001F642");
                    return;
                }
                else if (message.Text == "/start")
                {
                    TelegramBot.bot.SendTextMessageAsync(message.Chat.Id, "Смотри что я умею! :)", replyMarkup: ButtonOnTGbot());
                    return;
                }
                else if (hashWhatsUpArr.Contains(message.Text))
                {
                    await bot.SendTextMessageAsync(message.Chat.Id, "Хорошо, у тебя как?");
                    return;
                }
                else if (message.Text == "/getimage" || message.Text == "Скинь картинку")
                {
                    await TelegramBot.bot.SendPhotoAsync(message.Chat.Id, "https://www.animationsource.org/sites_content/lion_king/img_screenshot/85441.jpg", "\U0001F605");
                    return;
                }
                else if (message.Text == "Посмотреть погоду\U0001F325")
                {
                    await TelegramBot.bot.SendTextMessageAsync(message.Chat.Id, "Для того, чтобы бот показал погоду, напишите название города!\nДля того чтобы узнать какие города доступны, нажмите на это: /cityWeather");
                    return;
                }
                else if (message.Text == "/cityWeather")
                {
                    for (int i = 0; i < WeatherCity.Length; i++)
                    {
                        await TelegramBot.bot.SendTextMessageAsync(message.Chat.Id, WeatherCity[i] + "\n");
                    }
                    return;
                }
                else if (hashWeatherCity.Contains(message.Text))
                {
                    nameofCity = message.Text;
                    Weather(nameofCity);
                    await TelegramBot.bot.SendTextMessageAsync(message.Chat.Id, $"Температура в {nameofCity}: {Math.Round(tempOfCity)} °C");
                    return;
                }

                await bot.SendTextMessageAsync(message.Chat.Id, "Я не знаю как ответить на это \U0001F914");
            }
        }

        public static async Task errorHandler(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }

        public static void Weather(string cityName)
        {
            try
            {
                string url = "https://api.openweathermap.org/data/2.5/weather?q=" + cityName + "&appid=2351aaee5394613fc0d14424239de2bd";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                TelegramBot telegramBot = JsonConvert.DeserializeObject<TelegramBot>(response);

                tempOfCity = telegramBot.main.Temp - 273;
            }
            catch (System.Net.WebException)
            {
                Console.WriteLine("Непредвиденная ошибка :(");
                return;
            }
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
