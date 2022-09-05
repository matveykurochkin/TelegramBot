using System.Net.Http.Json;
using System.Web;
using NLog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot;

class MyTelegramBot
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    private readonly ITelegramBotClient _telegramBotClient;

    private string? _nameofCity, Cloud;
    private int _clouds, count, _pressure, _humidity, _randomCountGame = 11;
    private double _tempOfCity, _fellsLikeOfCity, _speed;
    DateTime _sunRiseDate, _sunSetDate;
    Random _random = new Random();
    const string IgnoredText = "@TGbobbot";
    private bool isRequest = false, isRequestYT = false, isExitGame = false, isRunGame = false;

    public MyTelegramBot(ITelegramBotClient telegramBotClient)
    {
        _telegramBotClient = telegramBotClient;
    }

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
                new KeyboardButton("Скинуть стикос😉"),
            },
            new[]
            {
                new KeyboardButton("Посмотреть погоду⛅"),
                new KeyboardButton("Найти в интернете🔎"),
            }
        });
        tgButton.ResizeKeyboard = true;
        return tgButton;
    }

    public IReplyMarkup ButtonCityOnTGbotForChannel()
    {
        var tgButton = new ReplyKeyboardMarkup(new[]
        {
            new[]
            {
                new KeyboardButton($"{ArrDataClass.WeatherCity[0]}"),
                new KeyboardButton($"{ArrDataClass.WeatherCity[1]}"),
            },
            new[]
            {
                new KeyboardButton($"{ArrDataClass.WeatherCity[2]}"),
                new KeyboardButton($"{ArrDataClass.WeatherCity[3]}"),
            },
            new[]
            {
                new KeyboardButton($"Load game")
            },
            new[]
            {
                new KeyboardButton("⬅")
            }
        });
        tgButton.ResizeKeyboard = true;
        return tgButton;
    }

    public IReplyMarkup ButtonOnGame()
    {
        var tgButton = new ReplyKeyboardMarkup(new[]
        {
            new[]
            {
                new KeyboardButton($"0"),
                new KeyboardButton($"1"),
                new KeyboardButton($"2"),
            },
            new[]
            {
                new KeyboardButton($"3"),
                new KeyboardButton($"4"),
                new KeyboardButton($"5")
            },
            new[]
            {
                new KeyboardButton($"6"),
                new KeyboardButton($"7"),
                new KeyboardButton($"8")
            },
            new[]
            {
                new KeyboardButton($"9"),
                new KeyboardButton($"10")
            },
            new[]
            {
                new KeyboardButton($"Закончить игру")
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
        _logger.Debug("Update received: {@update}", update);
        if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
        {
            var message = update.Message;

            string? adminID = Convert.ToString(message?.From?.Id);
            var hashAdminID = new HashSet<string>(ArrDataClass.AdminId);

            _logger.Info($"Пользователь {message?.From?.FirstName} {message?.From?.LastName} написал боту данное сообщение: {message?.Text}\nid Пользователя: {message?.From?.Id}");

            var hashHelloArr = new HashSet<string>(ArrDataClass.HelloArr);
            var hashWhatsUpArr = new HashSet<string>(ArrDataClass.WhatsUpArr);
            var hashWeatherCity = new HashSet<string>(ArrDataClass.WeatherCity);
            var hashWhatAreYouDoArr = new HashSet<string>(ArrDataClass.WhatAreYouDoArr);

            if (!string.IsNullOrEmpty(message?.Text) && message.Text.StartsWith(IgnoredText))
                message.Text = message.Text.Remove(0, 10);

            if (string.Equals(message?.Text, "/request", StringComparison.OrdinalIgnoreCase)
                 || string.Equals(message?.Text, "Найти в интернете🔎", StringComparison.OrdinalIgnoreCase)
                 || string.Equals(message?.Text, $"/request{IgnoredText}", StringComparison.OrdinalIgnoreCase))
            {
                _logger.Debug("Get request");
                count = _random.Next(ArrDataClass.AnswSearchArr.Length);
                await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"{ArrDataClass.AnswSearchArr[count]}", replyMarkup: ButtonOnRequest(), cancellationToken: cancellationToken);
                isRequest = true;
                return;
            }

            if ((string.Equals(message?.Text, "/requestYouTube", StringComparison.OrdinalIgnoreCase)
                || string.Equals(message?.Text, $"/requestYouTube{IgnoredText}", StringComparison.OrdinalIgnoreCase))
                && adminID != null && hashAdminID.Contains(adminID))
            {
                _logger.Debug("Get request YouTube");
                count = _random.Next(ArrDataClass.AnswSearchYTArr.Length);
                await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"{ArrDataClass.AnswSearchYTArr[count]}", replyMarkup: ButtonOnRequest(), cancellationToken: cancellationToken);
                isRequestYT = true;
                return;
            }

            if (isExitGame == true)
            {
                if (string.Equals(message?.Text, "Отменить поиск", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.Debug("Cancel get request YouTube");
                    await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"Поиск отменен! Можно продолжать общение с ботом!", cancellationToken: cancellationToken);
                    isExitGame = false;
                    return;
                }
            }

            if (isRunGame == true)
            {
                if (string.Equals(message?.Text, "Закончить игру", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.Debug("Exit game");
                    await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"Игра закончена! Можно продолжать общение с ботом!", replyMarkup: ButtonCityOnTGbotForChannel(), cancellationToken: cancellationToken);
                    isRunGame = false;
                    return;
                }
                if (int.TryParse(message?.Text, out int _randomCountGame))
                {
                    _randomCountGame = _random.Next(0, 11);               
                    if (Convert.ToInt32(message?.Text) == _randomCountGame)
                    {
                        _logger.Debug("Win and close game");
                        await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"Поздравляю ты победил!\nХочешь еще раз? Тогда нажми сюда: /game",replyMarkup: ButtonCityOnTGbotForChannel(), cancellationToken: cancellationToken);
                        isRunGame = false;
                        return;
                    }
                    else
                    {
                        _logger.Debug("lose and close game");
                        await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"Ты проиграл! Загаданное число: {_randomCountGame}\nХочешь еще раз? Тогда нажми сюда: /game", replyMarkup: ButtonCityOnTGbotForChannel(), cancellationToken: cancellationToken);
                        isRunGame = false;
                        return;
                    }
                }
                else
                {
                    _logger.Debug("Incorrect data in the game");
                    await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"Пожалуйста введите корректные данные!", cancellationToken: cancellationToken);
                    return;
                }
            }

            if (isRequestYT == true)
            {
                if (string.Equals(message?.Text, "Отменить поиск", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.Debug("Cancel get request YouTube");
                    await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"Поиск отменен! Можно продолжать общение с ботом!", cancellationToken: cancellationToken);
                    isRequestYT = false;
                    return;
                }
                _logger.Debug("Request YouTube send");
                var url = $"https://www.youtube.com/results?search_query={message?.Text?.Replace(" ", "+")}";
                await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"{url}", cancellationToken: cancellationToken);
                isRequestYT = false;
                return;
            }

            if (isRequest == true)
            {
                if (string.Equals(message?.Text, "Отменить поиск", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.Debug("Cancel get request");
                    await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"Поиск отменен! Можно продолжать общение с ботом!", cancellationToken: cancellationToken);
                    isRequest = false;
                    return;
                }
                _logger.Debug("Request send");
                var url = $"https://www.google.ru/search?q={message?.Text?.Replace(" ", "+")}";
                await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"{url}", cancellationToken: cancellationToken);
                isRequest = false;
                return;
            }

            if ((string.Equals(message?.Text, "Load game", StringComparison.OrdinalIgnoreCase) || string.Equals(message?.Text, "/game", StringComparison.OrdinalIgnoreCase))
                && adminID != null && hashAdminID.Contains(adminID))
            {
                _logger.Debug("Game run");
                _randomCountGame = _random.Next(0, 11);
                await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"Загадано число от 0 до 10! Попробуй отгадать!", replyMarkup: ButtonOnGame(), cancellationToken: cancellationToken);
                isRunGame = true;
                return;
            }

            if (!string.IsNullOrEmpty(message?.Text) && hashHelloArr.Contains(message.Text))
            {
                _logger.Debug("Command hello");
                count = _random.Next(ArrDataClass.AnswHelloArr.Length);
                await _telegramBotClient.SendTextMessageAsync(message.Chat.Id, $"{ArrDataClass.AnswHelloArr[count]} {message.From?.FirstName}! 🙂", cancellationToken: cancellationToken);
                return;
            }

            if (string.Equals(message?.Text, "/command", StringComparison.OrdinalIgnoreCase)
                || string.Equals(message?.Text, "Список команд", StringComparison.OrdinalIgnoreCase)
                || string.Equals(message?.Text, $"/command{IgnoredText}", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(message?.Text) && adminID != null && hashAdminID.Contains(adminID))
                {
                    _logger.Debug("Request list of commands for Admin");
                    await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"{ArrDataClass.CommandArr[0]} {ArrDataClass.CommandArrAdmin[0]}", cancellationToken: cancellationToken);
                    return;
                }
                _logger.Debug("Request list of commands");
                await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"{ArrDataClass.CommandArr[0]}", cancellationToken: cancellationToken);
                return;
            }

            if ((string.Equals(message?.Text, "/testFunction", StringComparison.OrdinalIgnoreCase)
                || string.Equals(message?.Text, $"/testFunction{IgnoredText}", StringComparison.OrdinalIgnoreCase))
                && adminID != null && hashAdminID.Contains(adminID))
            {
                _logger.Debug("Request list of commands for channel");
                count = _random.Next(ArrDataClass.SticerArr.Length);
                await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"Держи доступные тестовые функции!{ArrDataClass.SticerArr[count]}", replyMarkup: ButtonCityOnTGbotForChannel(), cancellationToken: cancellationToken);
                return;
            }

            if (string.Equals(message?.Text, "/getSticer", StringComparison.OrdinalIgnoreCase)
                || string.Equals(message?.Text, "Скинуть стикос😉", StringComparison.OrdinalIgnoreCase)
                || string.Equals(message?.Text, $"/getsticer{IgnoredText}", StringComparison.OrdinalIgnoreCase))
            {
                _logger.Debug("Get sticker");
                count = _random.Next(ArrDataClass.SticerArr.Length);
                await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"{ArrDataClass.SticerArr[count]}", cancellationToken: cancellationToken);
                return;
            }

            if (string.Equals(message?.Text, "/start", StringComparison.OrdinalIgnoreCase)
                || string.Equals(message?.Text, "Старт", StringComparison.OrdinalIgnoreCase)
                || string.Equals(message?.Text, $"/start{IgnoredText}", StringComparison.OrdinalIgnoreCase)
                || string.Equals(message?.Text, $"⬅", StringComparison.OrdinalIgnoreCase))
            {
                _logger.Debug("Start");
                count = _random.Next(ArrDataClass.SticerArr.Length);
                await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"Смотри что я умею! {ArrDataClass.SticerArr[count]}", replyMarkup: ButtonOnTGbot(), cancellationToken: cancellationToken);
                return;
            }

            if (!string.IsNullOrEmpty(message?.Text) && hashWhatsUpArr.Contains(message.Text))
            {
                _logger.Debug("Command WhatsUp");
                count = _random.Next(ArrDataClass.AnswWhatsUpArr.Length);
                await _telegramBotClient.SendTextMessageAsync(message.Chat.Id, $"{ArrDataClass.AnswWhatsUpArr[count]}", cancellationToken: cancellationToken);
                return;
            }

            if (string.Equals(message?.Text, "/getimage", StringComparison.OrdinalIgnoreCase)
                || string.Equals(message?.Text, "Скинуть пикчу🗿", StringComparison.OrdinalIgnoreCase)
                || string.Equals(message?.Text, $"/getimage{IgnoredText}", StringComparison.OrdinalIgnoreCase))
            {
                _logger.Debug("Get image");
                count = _random.Next(ArrDataClass.PicArr.Length);
                await _telegramBotClient.SendPhotoAsync(message?.Chat.Id ?? 0, $"{ArrDataClass.PicArr[count]}", $"{ArrDataClass.SticerArr[count]}", cancellationToken: cancellationToken);
                return;
            }

            if (string.Equals(message?.Text, "Посмотреть погоду⛅", StringComparison.OrdinalIgnoreCase))
            {
                _logger.Debug("Get weather");
                await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, "Для того, чтобы бот показал погоду, напишите название города!\nДля того, чтобы узнать какие города доступны, нажмите на это: /cityWeather", cancellationToken: cancellationToken);
                return;
            }

            if (!string.IsNullOrEmpty(message?.Text) && hashWhatAreYouDoArr.Contains(message.Text))
            {
                _logger.Debug("Command WhatAreYouDo");
                count = _random.Next(ArrDataClass.AnswWhatAreYouDoArr.Length);
                await _telegramBotClient.SendTextMessageAsync(message.Chat.Id, $"{ArrDataClass.AnswWhatAreYouDoArr[count]}", cancellationToken: cancellationToken);
                return;
            }

            if (string.Equals(message?.Text, "/cityWeather", StringComparison.OrdinalIgnoreCase)
                || string.Equals(message?.Text, $"/cityweather{IgnoredText}", StringComparison.OrdinalIgnoreCase))
            {
                _logger.Debug("List of the city");
                foreach (var city in ArrDataClass.WeatherCity)
                {
                    await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"Узнать погоду в городе: ", replyMarkup: ButtonOnChatTGbot(city), cancellationToken: cancellationToken);
                }
                return;
            }

            if (!string.IsNullOrEmpty(message?.Text) && hashWeatherCity.Contains(message.Text))
            {
                _logger.Debug("Weather response");
                _nameofCity = message.Text;
                await Weather(_nameofCity, cancellationToken);
                if (_clouds >= 0 && _clouds <= 14)
                    Cloud = "☀";
                else if (_clouds >= 15 && _clouds <= 40)
                    Cloud = "⛅";
                else if (_clouds >= 41 && _clouds <= 120)
                    Cloud = "☁";
                await _telegramBotClient.SendTextMessageAsync(message.Chat.Id, $"Температура в {_nameofCity}: {_tempOfCity} °C {Cloud}\nОщущается как { _fellsLikeOfCity} °C\n" +
                    $"Влажность воздуха: {_humidity}%\nСкорость ветра: {_speed} м/с\n" +
                    $"Атмосферное давление: {Math.Round(_pressure * 0.75)} мм рт.ст.\n" +
                    $"Восход: {_sunRiseDate}\nЗакат: {_sunSetDate}", cancellationToken: cancellationToken);
                return;
            }
            count = _random.Next(ArrDataClass.AnswOther.Length);
            await _telegramBotClient.SendTextMessageAsync(message?.Chat?.Id ?? 0, $"{ArrDataClass.AnswOther[count]} {"\n\nХочешь я это загуглю? Нажми: /request и напиши слово заново или просто перешли сообщение!"}", cancellationToken: cancellationToken);
        }
    }

    internal Task ErrorHandler(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        _logger.Error(exception, "Error received in telegram bot");
        return Task.CompletedTask;
    }

    private async Task Weather(string cityName, CancellationToken cancellationToken)
    {
        _logger.Debug("Try to get weather");
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
            _logger.Error(ex, "Error response weather");
        }
    }
}