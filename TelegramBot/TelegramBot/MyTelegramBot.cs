using System.Web;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot;

class MyTelegramBot
{
    private readonly ITelegramBotClient _telegramBotClient;

    public MyTelegramBot(ITelegramBotClient telegramBotClient)
    {
        _telegramBotClient = telegramBotClient;
    }

    public MyTelegramBot main { get; set; }
    public float Temp { get; set; }

    string[] HelloArr = {"Привет!", "привет", "Привет", "Ку", "ghbdtn", "ку", "дороу", "Дороу"};
    string[] WhatsUpArr = {"Как дела?", "как дела?", "как дела"};
    string[] WeatherCity = {"Владимир", "Москва", "Санкт-Петербург", "Головино", "Боголюбово", "Дубай", "Гусь-Хрустальный"};

    string nameofCity;
    float tempOfCity;

    public IReplyMarkup ButtonOnTGbot()
    {
        var tgButton = new ReplyKeyboardMarkup(new[]
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
        tgButton.ResizeKeyboard = true;
        return tgButton;
    }

    internal async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
        {
            var message = update.Message;

            Console.WriteLine($"Пользователь {message?.From?.FirstName} {message?.From?.LastName} написал боту данное сообщение: {message?.Text}");
            Console.WriteLine($"\tid Пользователя: {message?.From?.Id}");

            var hashHelloArr = new HashSet<string>(HelloArr);
            var hashWhatsUpArr = new HashSet<string>(WhatsUpArr);
            var hashWeatherCity = new HashSet<string>(WeatherCity);

            await using var dataBase = new StreamWriter("E:\\DataBase.txt", true);
            await dataBase.WriteLineAsync(JsonConvert.SerializeObject(update));
            dataBase.Close();

            if (!string.IsNullOrEmpty(message?.Text) && hashHelloArr.Contains(message.Text))
            {
                await bot.SendTextMessageAsync(message.Chat.Id, $"Привет {message.From?.FirstName}! 🙂", cancellationToken: cancellationToken);
                return;
            }

            if (string.Equals(message?.Text, "/start", StringComparison.OrdinalIgnoreCase))
            {
                await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, "Смотри что я умею! :)", replyMarkup: ButtonOnTGbot(), cancellationToken: cancellationToken);
                return;
            }

            if (!string.IsNullOrEmpty(message?.Text) && hashWhatsUpArr.Contains(message.Text))
            {
                await bot.SendTextMessageAsync(message.Chat.Id, "Хорошо, у тебя как?", cancellationToken: cancellationToken);
                return;
            }

            if (string.Equals(message?.Text, "/getimage", StringComparison.OrdinalIgnoreCase)
                || string.Equals(message?.Text, "Скинь картинку", StringComparison.OrdinalIgnoreCase))
            {
                await _telegramBotClient.SendPhotoAsync(message?.Chat.Id ?? 0, "https://www.animationsource.org/sites_content/lion_king/img_screenshot/85441.jpg", "\U0001F605", cancellationToken: cancellationToken);
                return;
            }

            if (string.Equals(message?.Text, "Посмотреть погоду\U0001F325", StringComparison.OrdinalIgnoreCase))
            {
                await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, "Для того, чтобы бот показал погоду, напишите название города!\nДля того чтобы узнать какие города доступны, нажмите на это: /cityWeather", cancellationToken: cancellationToken);
                return;
            }

            if (string.Equals(message?.Text, "/cityWeather", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var city in WeatherCity)
                {
                    await _telegramBotClient.SendTextMessageAsync(message.Chat.Id, $"{city}\n", cancellationToken: cancellationToken);
                }

                return;
            }

            if (hashWeatherCity.Contains(message.Text))
            {
                nameofCity = message.Text;
                await Weather(nameofCity, cancellationToken);
                await _telegramBotClient.SendTextMessageAsync(message.Chat.Id, $"Температура в {nameofCity}: {Math.Round(tempOfCity)} °C", cancellationToken: cancellationToken);
                return;
            }

            await bot.SendTextMessageAsync(message.Chat.Id, "Я не знаю как ответить на это \U0001F914", cancellationToken: cancellationToken);
        }
    }

    internal Task ErrorHandler(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine(JsonConvert.SerializeObject(exception));
        return Task.CompletedTask;
    }

    private async Task Weather(string cityName, CancellationToken cancellationToken)
    {
        const string appid = "2351aaee5394613fc0d14424239de2bd";

        try
        {
            var url = $"https://api.openweathermap.org/data/2.5/weather?q={HttpUtility.UrlEncode(cityName)}&appid={HttpUtility.UrlEncode(appid)}";
            using var hc = new HttpClient();
            var response = await hc.GetStringAsync(url, cancellationToken);
            var myTelegramBot = JsonConvert.DeserializeObject<MyTelegramBot>(response);
            tempOfCity = myTelegramBot.main.Temp - 273;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Непредвиденная ошибка :(  {ex.Message}");
        }
    }
}