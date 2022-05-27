using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Net;
using Newtonsoft.Json;

namespace TelegramBot
{
    class TelegramBot
    {
        public TelegramBot main { get; set; }
        public float Temp { get; set; }

        static ITelegramBotClient bot = new TelegramBotClient("Token");

        static string[] HelloArr = new[] {"Привет!", "привет", "Привет", "Ку", "ghbdtn", "ку", "дороу", "Дороу"};
        static string[] WhatsUpArr = new[] {"Как дела?", "как дела?", "как дела"};
        static string[] WeatherCity = new[] {"Владимир", "Москва", "Санкт-Петербург", "Головино"};

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
                    new KeyboardButton("Погода"),
                }
            });
            TGbutton.ResizeKeyboard = true;
            return TGbutton;
        }

        private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;

                Console.WriteLine($"Пользователь {message.From.FirstName} {message.From.LastName} написал боту данное сообщение: {message.Text}");
                Console.WriteLine($"\tid Пользователя: {message.From.Id}");

                var hashHelloArr = new HashSet<string>(HelloArr);
                var hashWhatsUpArr = new HashSet<string>(WhatsUpArr);
                var hashWeatherCity = new HashSet<string>(WeatherCity);

                await using StreamWriter dataBase = new StreamWriter("E:\\DataBase.txt", true);
                await dataBase.WriteLineAsync(JsonConvert.SerializeObject(update));
                dataBase.Close();


                if (hashHelloArr.Contains(message.Text))
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Привет {message.From.FirstName}! 🙂");
                    return;
                }

                if (string.Equals(message.Text, "/start", StringComparison.OrdinalIgnoreCase))
                {
                    await bot.SendTextMessageAsync(message.Chat.Id, "Смотри что я умею! :)", replyMarkup: ButtonOnTGbot(), cancellationToken: cancellationToken);
                    return;
                }

                if (hashWhatsUpArr.Contains(message.Text))
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Хорошо, у тебя как?", cancellationToken: cancellationToken);
                    return;
                }

                if (string.Equals(message.Text, "/getimage", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(message.Text, "Скинь картинку", StringComparison.OrdinalIgnoreCase))
                {
                    await bot.SendPhotoAsync(message.Chat.Id, "https://avatarko.ru/img/kartinka/33/Star_Wars_Darth_Vader_32632.jpg", "Смотри, это Дарт Вейдер!", cancellationToken: cancellationToken);
                    return;
                }

                if (string.Equals(message.Text, "Погода", StringComparison.InvariantCulture))
                {
                    await bot.SendTextMessageAsync(message.Chat.Id, "Для того, чтобы бот показал погоду, напишите название города!\nДля того чтобы узнать какие города доступны, нажмите на это: /cityWeather", cancellationToken: cancellationToken);
                    return;
                }

                if (message.Text == "/cityWeather")
                {
                    foreach (var t in WeatherCity)
                        await bot.SendTextMessageAsync(message.Chat.Id, $"{t}\n", cancellationToken: cancellationToken);

                    return;
                }
                
                if (hashWeatherCity.Contains(message.Text))
                {
                    nameofCity = message.Text;
                    Weather(nameofCity);
                    await bot.SendTextMessageAsync(message.Chat.Id, $" \n\nТемпература в {nameofCity}: {Math.Round(tempOfCity)} °C", cancellationToken: cancellationToken);
                    return;
                }

                await botClient.SendTextMessageAsync(message.Chat.Id, "Я не знаю как ответить на это \U0001F914", cancellationToken: cancellationToken);
            }
        }

        private static async Task errorHandler(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(JsonConvert.SerializeObject(exception));
        }

        private static void Weather(string cityName)
        {
            try
            {
                string url = "https://api.openweathermap.org/data/2.5/weather?q=" + cityName + "&appid=2351aaee5394613fc0d14424239de2bd";
                HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse) httpWebRequest?.GetResponse();
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
            bot.StartReceiving(UpdateHandler, errorHandler, receiverOptions, cancellationToken);
            Console.ReadLine();
        }
    }
}