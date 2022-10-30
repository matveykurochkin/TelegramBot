using System.Net.Http.Json;
using System.Web;
using Fortnite_API;
using Microsoft.Extensions.Configuration;
using NLog;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Configuration;

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
    WeatherOptions _weatherOptions = new WeatherOptions();
    const string _nameBot = "@TGbobbot";
    private bool isRequest = false, isRequestYT = false, isExitGame = false, isRunGame = false, isEasy = false, isMedium = false, isHard = false;

    public string CreateConfig()
    {
        var AppName = new ConfigurationBuilder().AddJsonFile("appsettings.json")
            .Build()
            .GetSection("APIWeather")["TokenWeatherID"];
        return AppName;
    }

    public async Task<string> FortniteMap(CancellationToken cancellationToken)
    {
        var apiClient = new FortniteApiClient();
        var mapResponse = await apiClient.V1.Map.GetAsync(token: cancellationToken);
        if (mapResponse.Status != 200)
        {
            throw new InvalidOperationException($"Error call api: {mapResponse.Error}");
        }
        var imageLink = mapResponse.Data.Images.POIs.ToString();
        return imageLink;
    }

    public MyTelegramBot(ITelegramBotClient telegramBotClient)
    {
        _telegramBotClient = telegramBotClient;
    }

    internal async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        _logger.Debug($"Update received: {update}");
        if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
        {
            var message = update.Message;

            string? adminID = Convert.ToString(message?.From?.Id);
            var hashAdminID = new HashSet<string>(ArrDataClass.AdminId);

            bool isAdmin()
            {
                if (adminID != null && hashAdminID.Contains(adminID))
                    return true;
                return false;
            }

            _logger.Info($"Пользователь {message?.From?.FirstName} {message?.From?.LastName} написал боту данное сообщение: {message?.Text}\nid Пользователя: {message?.From?.Id}");

            var hashHelloArr = new HashSet<string>(ArrDataClass.HelloArr);
            var hashWhatsUpArr = new HashSet<string>(ArrDataClass.WhatsUpArr);
            var hashWeatherCity = new HashSet<string>(ArrDataClass.WeatherCity);
            var hashWhatAreYouDoArr = new HashSet<string>(ArrDataClass.WhatAreYouDoArr);

            if (!string.IsNullOrEmpty(message?.Text) && message.Text.StartsWith(_nameBot))
                message.Text = message.Text.Remove(0, _nameBot.Length + 1);

            if (string.Equals(message?.Text, "/request", StringComparison.OrdinalIgnoreCase)
                || string.Equals(message?.Text, "Найти в интернете🔎", StringComparison.OrdinalIgnoreCase)
                || string.Equals(message?.Text, $"/request{_nameBot}", StringComparison.OrdinalIgnoreCase))
            {
                _logger.Debug("Get request");
                count = _random.Next(ArrDataClass.AnswSearchArr.Length);
                await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"{ArrDataClass.AnswSearchArr[count]}", replyMarkup: BotButtons.ButtonOnRequest(), cancellationToken: cancellationToken);
                isRequest = true;
                return;
            }

            if ((string.Equals(message?.Text, "/requestYouTube", StringComparison.OrdinalIgnoreCase)
                 || string.Equals(message?.Text, $"/requestYouTube{_nameBot}", StringComparison.OrdinalIgnoreCase))
                && isAdmin())
            {
                _logger.Debug("Get request YouTube");
                count = _random.Next(ArrDataClass.AnswSearchYTArr.Length);
                await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"{ArrDataClass.AnswSearchYTArr[count]}", replyMarkup: BotButtons.ButtonOnRequest(), cancellationToken: cancellationToken);
                isRequestYT = true;
                return;
            }

            if (isExitGame)
            {
                if (string.Equals(message?.Text, "Отменить поиск", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.Debug("Cancel get request YouTube");
                    await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"Поиск отменен! Можно продолжать общение с ботом!", cancellationToken: cancellationToken);
                    isExitGame = false;
                    return;
                }
            }

            if (isRunGame)
            {
                if (string.Equals(message?.Text, "Закончить игру", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.Debug("Exit game");
                    await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"Игра закончена! Можно продолжать общение с ботом!", replyMarkup: BotButtons.ButtonCityOnTGbotForChannel(), cancellationToken: cancellationToken);
                    isRunGame = false;
                    return;
                }

                if (string.Equals(message?.Text, "Легко", StringComparison.OrdinalIgnoreCase)
                    && isAdmin())
                {
                    _randomCountGame = _random.Next(0, 6);
                    await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"Выбран легкий режим игры! Удачи!", replyMarkup: BotButtons.ButtonOnGameEasy(), cancellationToken: cancellationToken);
                    isEasy = true;
                    return;
                }
                else if (string.Equals(message?.Text, "Средне", StringComparison.OrdinalIgnoreCase)
                         && isAdmin())
                {
                    _randomCountGame = _random.Next(0, 11);
                    await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"Выбран средний режим игры! Удачи!", replyMarkup: BotButtons.ButtonOnGameMedium(), cancellationToken: cancellationToken);
                    isMedium = true;
                    return;
                }
                else if (string.Equals(message?.Text, "Сложно", StringComparison.OrdinalIgnoreCase)
                         && isAdmin())
                {
                    _randomCountGame = _random.Next(0, 16);
                    await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"Выбран сложный режим игры! Удачи!", replyMarkup: BotButtons.ButtonOnGameHard(), cancellationToken: cancellationToken);
                    isHard = true;
                    return;
                }

                if (string.Equals(message?.Text, "Подсказка", StringComparison.OrdinalIgnoreCase)
                    && isAdmin())
                {
                    _logger.Debug("help game");
                    if (isEasy)
                    {
                        _logger.Debug("help easy game");
                        if (_randomCountGame <= 2)
                            await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"Число в диапазоне от 0 до 2!", cancellationToken: cancellationToken);
                        else
                            await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"Число в диапазоне от 3 до 5!", cancellationToken: cancellationToken);
                        return;
                    }
                    else if (isMedium)
                    {
                        _logger.Debug("help medium game");
                        if (_randomCountGame <= 5)
                            await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"Число в диапазоне от 0 до 5!", cancellationToken: cancellationToken);
                        else
                            await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"Число в диапазоне от 6 до 10!", cancellationToken: cancellationToken);
                        return;
                    }
                    else if (isHard)
                    {
                        _logger.Debug("help hard game");
                        if (_randomCountGame <= 8)
                            await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"Число в диапазоне от 0 до 8!", cancellationToken: cancellationToken);
                        else
                            await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"Число в диапазоне от 9 до 15!", cancellationToken: cancellationToken);
                        return;
                    }

                    return;
                }
            }

            if (isRunGame && (isEasy || isMedium || isHard))
            {
                void ExitGame()
                {
                    isRunGame = false;
                    isEasy = false;
                    isMedium = false;
                    isHard = false;
                }

                if (int.TryParse(message?.Text, out int _inputGame))
                {
                    if (_inputGame == _randomCountGame)
                    {
                        _logger.Debug("Win and close game");
                        await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"Поздравляю ты победил!\nХочешь еще раз? Тогда нажми сюда: /game", replyMarkup: BotButtons.ButtonCityOnTGbotForChannel(), cancellationToken: cancellationToken);
                        ExitGame();
                        return;
                    }

                    _logger.Debug("lose and close game");
                    await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"Ты проиграл! Загаданное число: {_randomCountGame}\nХочешь еще раз? Тогда нажми сюда: /game", replyMarkup: BotButtons.ButtonCityOnTGbotForChannel(), cancellationToken: cancellationToken);
                    ExitGame();
                    return;
                }

                _logger.Debug("Incorrect data in the game");
                await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"Пожалуйста введите корректные данные!", replyMarkup: BotButtons.ButtonCityOnTGbotForChannel(), cancellationToken: cancellationToken);
                isRunGame = false;
                return;
            }

            if (isRequestYT)
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

            if (isRequest)
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

            if (!string.IsNullOrEmpty(message?.Text) && (string.Equals(message?.Text, "Карта Fortnite🗺", StringComparison.OrdinalIgnoreCase)
                                                     || string.Equals(message?.Text, "/mapfortnite", StringComparison.OrdinalIgnoreCase)))
            {
                _logger.Debug("Get map fortnite");
                count = _random.Next(ArrDataClass.SticerArr.Length);
                var mapLink = await FortniteMap(cancellationToken);
                await _telegramBotClient.SendPhotoAsync(message?.Chat.Id ?? 0, $"{mapLink}", $"Map Fortnite {ArrDataClass.SticerArr[count]}", cancellationToken: cancellationToken);
                return;
            }

            if ((string.Equals(message?.Text, "Игра «Угадай число»", StringComparison.OrdinalIgnoreCase) || string.Equals(message?.Text, "/game", StringComparison.OrdinalIgnoreCase))
                && isAdmin())
            {
                _logger.Debug("Game run");
                _randomCountGame = _random.Next(0, 11);
                await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"Игра «Угадай число»!\nВыбери сложность игры, где:\n" +
                                                                                     $"1) Легко (диапазон от 0 до 5)\n2) Средне (диапазон от 0 до 10)\n3) Сложно (диапазон от 0 до 15)", replyMarkup: BotButtons.difficultySelectionButton(), cancellationToken: cancellationToken);
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
                || string.Equals(message?.Text, $"/command{_nameBot}", StringComparison.OrdinalIgnoreCase))
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
                 || string.Equals(message?.Text, $"/testFunction{_nameBot}", StringComparison.OrdinalIgnoreCase))
                && isAdmin())
            {
                _logger.Debug("Request list of commands for channel");
                count = _random.Next(ArrDataClass.SticerArr.Length);
                await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"Держи доступные тестовые функции!{ArrDataClass.SticerArr[count]}", replyMarkup: BotButtons.ButtonCityOnTGbotForChannel(), cancellationToken: cancellationToken);
                return;
            }

            if (string.Equals(message?.Text, "/getSticer", StringComparison.OrdinalIgnoreCase)
                || string.Equals(message?.Text, "Скинуть стикос😉", StringComparison.OrdinalIgnoreCase)
                || string.Equals(message?.Text, $"/getsticer{_nameBot}", StringComparison.OrdinalIgnoreCase))
            {
                _logger.Debug("Get sticker");
                count = _random.Next(ArrDataClass.SticerArr.Length);
                await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"{ArrDataClass.SticerArr[count]}", cancellationToken: cancellationToken);
                return;
            }

            if (string.Equals(message?.Text, "/start", StringComparison.OrdinalIgnoreCase)
                || string.Equals(message?.Text, "Старт", StringComparison.OrdinalIgnoreCase)
                || string.Equals(message?.Text, $"/start{_nameBot}", StringComparison.OrdinalIgnoreCase)
                || string.Equals(message?.Text, $"⬅", StringComparison.OrdinalIgnoreCase))
            {
                _logger.Debug("Start");
                count = _random.Next(ArrDataClass.SticerArr.Length);
                await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"Смотри что я умею! {ArrDataClass.SticerArr[count]}", replyMarkup: BotButtons.ButtonOnTGbot(), cancellationToken: cancellationToken);
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
                || string.Equals(message?.Text, $"/getimage{_nameBot}", StringComparison.OrdinalIgnoreCase))
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
                || string.Equals(message?.Text, $"/cityweather{_nameBot}", StringComparison.OrdinalIgnoreCase))
            {
                _logger.Debug("List of the city");
                foreach (var city in ArrDataClass.WeatherCity)
                {
                    await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"Узнать погоду в городе: ", replyMarkup: BotButtons.ButtonOnChatTGbot(city), cancellationToken: cancellationToken);
                }

                return;
            }

            if (string.Equals(message?.Text, "/whatwear", StringComparison.OrdinalIgnoreCase)
                || string.Equals(message?.Text, $"Что сегодня надеть?", StringComparison.OrdinalIgnoreCase))
            {
                _logger.Debug("What to wear answer");
                count = _random.Next(ArrDataClass.SticerArr.Length);
                await _telegramBotClient.SendTextMessageAsync(message?.Chat.Id ?? 0, $"Функция в разработке! {ArrDataClass.SticerArr[count]}", cancellationToken: cancellationToken);
                return;
            }

            if (!string.IsNullOrEmpty(message?.Text) && hashWeatherCity.Contains(message.Text) && !string.IsNullOrEmpty(CreateConfig()))
            {
                _logger.Debug("Weather response");
                _nameofCity = message.Text;
                await Weather(_nameofCity, cancellationToken);
                if (_clouds is >= 0 and <= 14)
                    Cloud = "☀";
                else if (_clouds is >= 15 and <= 40)
                    Cloud = "⛅";
                else if (_clouds is >= 41 and <= 120)
                    Cloud = "☁";
                await _telegramBotClient.SendTextMessageAsync(message.Chat.Id, $"Температура в {_nameofCity}: {_tempOfCity} °C {Cloud}\nОщущается как {_fellsLikeOfCity} °C\n" +
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

        string appid = _weatherOptions.EsureValidTokenWeather(CreateConfig());

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