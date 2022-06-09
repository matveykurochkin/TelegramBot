using System.Net.Http.Json;
using System.Web;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot;

class MyTelegramBot
{
    private readonly ITelegramBotClient _telegramBotClient;

    private string? _nameofCity, Cloud;
    private int _clouds, count, _pressure, _humidity;
    private double _tempOfCity, _fellsLikeOfCity, _speed;
    DateTime _sunRiseDate, _sunSetDate;
    Random _random = new Random();
    const string IgnoredText = "@MyTelegGBot";
    private bool isRequest = false;

    public MyTelegramBot(ITelegramBotClient telegramBotClient)
    {
        _telegramBotClient = telegramBotClient;
    }

    string[] HelloArr = ArrDataClass.HelloArr;
    string[] AnswHelloArr = ArrDataClass.AnswHelloArr;
    string[] WhatsUpArr = ArrDataClass.WhatsUpArr;
    string[] AnswWhatsUpArr = ArrDataClass.AnswWhatsUpArr;
    string[] WeatherCity = ArrDataClass.WeatherCity;
    string[] WhatAreYouDoArr = ArrDataClass.WhatAreYouDoArr;
    string[] AnswWhatAreYouDoArr = ArrDataClass.AnswWhatAreYouDoArr;
    string[] PicArr = ArrDataClass.PicArr;
    string[] CommandArr = ArrDataClass.CommandArr;
    string[] AnswSearchArr = ArrDataClass.AnswSearchArr;
    public IReplyMarkup ButtonOnTGbot()
    {
        var tgButton = new ReplyKeyboardMarkup(new[]
        {
            new[]
            {
                new KeyboardButton("Привет!"),
                new KeyboardButton("Как дела?"),
                new KeyboardButton("Чд?"),
            },
            new[]
            {
                new KeyboardButton("Скинуть пикчу🗿"),
                new KeyboardButton("Посмотреть погоду⛅"),
            },
            new[]
            {
                new KeyboardButton("Список доступных команд"),
            },
            new[]
            {
                new KeyboardButton("Найти в интернете🔎"),
            }
        });
        tgButton.ResizeKeyboard = true;
        return tgButton;
    }
    public IReplyMarkup ButtonOnChatTGbot(string City)
    {
        return new InlineKeyboardMarkup(new[]
        {
        new []
        {
            InlineKeyboardButton.WithSwitchInlineQueryCurrentChat(text: City,$"{City}"),
        }
        });
    }
    public IReplyMarkup ButtonOnRequest()
    {
        return new InlineKeyboardMarkup(new[]
        {
        new []
        {
            InlineKeyboardButton.WithSwitchInlineQueryCurrentChat(text: "Отменить поиск","Отменить поиск"),
        }
        });
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
            var hashWhatAreYouDoArr = new HashSet<string>(WhatAreYouDoArr);

            await using var dataBase = new StreamWriter("E:\\DataBase.txt", true);
            await dataBase.WriteLineAsync(JsonConvert.SerializeObject(update));
            dataBase.Close();

            if (!string.IsNullOrEmpty(message?.Text) && message.Text.StartsWith(IgnoredText))
                message.Text = message.Text.Remove(0, 13);

            if (string.Equals(message?.Text, "/request", StringComparison.OrdinalIgnoreCase)
                 || string.Equals(message?.Text, "Найти в интернете🔎", StringComparison.OrdinalIgnoreCase))
            {
                count = _random.Next(AnswSearchArr.Length);
                await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"{AnswSearchArr[count]}", replyMarkup: ButtonOnRequest(), cancellationToken: cancellationToken);
                isRequest = true;
                return;
            }

            if (isRequest == true)
            {
                if (string.Equals(message?.Text, "Отменить поиск", StringComparison.OrdinalIgnoreCase))
                {
                    await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"Поиск отменен! Можно продолжать общение с ботом!", cancellationToken: cancellationToken);
                    isRequest = false;
                    return;
                }
                var url = $"https://www.google.ru/search?q={message?.Text?.Replace(" ", "+")}";
                await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"{url}", cancellationToken: cancellationToken);
                isRequest = false;
                return;
            }

            if (!string.IsNullOrEmpty(message?.Text) && hashHelloArr.Contains(message.Text))
            {
                count = _random.Next(AnswHelloArr.Length);
                await _telegramBotClient.SendTextMessageAsync(message.Chat.Id, $"{AnswHelloArr[count]} {message.From?.FirstName}! 🙂", cancellationToken: cancellationToken);
                return;
            }

            if (string.Equals(message?.Text, "/command", StringComparison.OrdinalIgnoreCase)
                || string.Equals(message?.Text, "Список доступных команд", StringComparison.OrdinalIgnoreCase))
            {
                await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"{CommandArr[0]}", cancellationToken: cancellationToken);
                return;
            }

            if (string.Equals(message?.Text, "/start", StringComparison.OrdinalIgnoreCase)
                || string.Equals(message?.Text, "Старт", StringComparison.OrdinalIgnoreCase))
            {
                await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, "Смотри что я умею! \U0001F600", replyMarkup: ButtonOnTGbot(), cancellationToken: cancellationToken);
                return;
            }

            if (!string.IsNullOrEmpty(message?.Text) && hashWhatsUpArr.Contains(message.Text))
            {
                count = _random.Next(AnswWhatsUpArr.Length);
                await _telegramBotClient.SendTextMessageAsync(message.Chat.Id, $"{AnswWhatsUpArr[count]}", cancellationToken: cancellationToken);
                return;
            }

            if (string.Equals(message?.Text, "/getimage", StringComparison.OrdinalIgnoreCase)
                || string.Equals(message?.Text, "Скинуть пикчу🗿", StringComparison.OrdinalIgnoreCase))
            {
                count = _random.Next(PicArr.Length);
                await _telegramBotClient.SendPhotoAsync(message?.Chat.Id ?? 0, $"{PicArr[count]}", "\U0001F605", cancellationToken: cancellationToken);
                return;
            }

            if (string.Equals(message?.Text, "Посмотреть погоду⛅", StringComparison.OrdinalIgnoreCase))
            {
                await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, "Для того, чтобы бот показал погоду, напишите название города!\nДля того, чтобы узнать какие города доступны, нажмите на это: /cityWeather", cancellationToken: cancellationToken);
                return;
            }

            if (!string.IsNullOrEmpty(message?.Text) && hashWhatAreYouDoArr.Contains(message.Text))
            {
                count = _random.Next(AnswWhatAreYouDoArr.Length);
                await _telegramBotClient.SendTextMessageAsync(message.Chat.Id, $"{AnswWhatAreYouDoArr[count]}", cancellationToken: cancellationToken);
                return;
            }

            if (string.Equals(message?.Text, "/cityWeather", StringComparison.OrdinalIgnoreCase))
            {
                for (int i = 0; i < WeatherCity.Length; i++)
                {
                    await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"Узнать погоду в городе: ", replyMarkup: ButtonOnChatTGbot(WeatherCity[i]), cancellationToken: cancellationToken);
                }
                return;
            }

            if (!string.IsNullOrEmpty(message?.Text) && hashWeatherCity.Contains(message.Text))
            {
                _nameofCity = message.Text;
                await Weather(_nameofCity, cancellationToken);
                if (_clouds >= 0 && _clouds <= 5)
                    Cloud = "☀";
                else if (_clouds >= 6 && _clouds <= 40)
                    Cloud = "⛅";
                else if (_clouds >= 41 && _clouds <= 80)
                    Cloud = "☁";
                else if (_clouds >= 81 && _clouds <= 120)
                    Cloud = "🌧";
                await _telegramBotClient.SendTextMessageAsync(message.Chat.Id, $"Температура в {_nameofCity}: {_tempOfCity} °C {Cloud}\nОщущается как { _fellsLikeOfCity} °C\n" +
                    $"Влажность воздуха: {_humidity}%\nСкорость ветра: {_speed} м/с\n" +
                    $"Атмосферное давление: {Math.Round(_pressure * 0.75)} мм рт.ст.\n" +
                    $"Восход: {_sunRiseDate}\nЗакат: {_sunSetDate}", cancellationToken: cancellationToken);
                return;
            }

            await _telegramBotClient.SendTextMessageAsync(message?.Chat?.Id ?? 0, "Я не знаю как ответить на это \U0001F914", cancellationToken: cancellationToken);
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
            var url = $"https://api.openweathermap.org/data/2.5/weather?q={HttpUtility.UrlEncode(cityName)}&appid={HttpUtility.UrlEncode(appid)}&units=metric";
            using var hc = new HttpClient();
            var weather = await hc.GetFromJsonAsync<WeatherResponse>(url, cancellationToken);
            if (weather != null)
            {
                _tempOfCity = Math.Round(weather.main.temp);
                _fellsLikeOfCity = Math.Truncate(weather.main.feels_like);
                _humidity = weather.main.humidity;
                _pressure = weather.main.pressure;
                _clouds = weather.clouds.all;
                _speed = weather.wind.speed;
                _sunRiseDate = DateTime.SpecifyKind(new DateTime(1970, 1, 1).AddSeconds(weather.sys.sunrise), DateTimeKind.Utc).ToLocalTime();
                _sunSetDate = DateTime.SpecifyKind(new DateTime(1970, 1, 1).AddSeconds(weather.sys.sunset), DateTimeKind.Utc).ToLocalTime();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Непредвиденная ошибка :(  {ex.Message}");
        }
    }
}