using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Web;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Configuration;
using TelegramBot.Internal;

namespace TelegramBot.Processors;

[TelegramCommand("Владимир", "Головино", "Москва", "Санкт-Петербург", "Боголюбово", "Гусь-Хрустальный", "Дубай", "Сочи" )]
internal class GetWeatherProcessor : MessageProcessorBase, ITelegramMessageProcessor
{
    private string? _nameofCity, Cloud;
    private int _clouds, _pressure, _humidity;
    private double _tempOfCity, _fellsLikeOfCity, _speed;
    DateTime _sunRiseDate, _sunSetDate;
    WeatherOptions _weatherOptions = new WeatherOptions();
    public async Task ProcessMessage(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        var AppName = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                        .Build()
                        .GetSection("APIWeather")["TokenWeatherID"];

        if (!string.IsNullOrEmpty(AppName))
        {
            _logger.Debug("Weather response");
            _nameofCity = update.Message?.Text;
            await Weather(cityName: _nameofCity, cancellationToken);
            if (_clouds is >= 0 and <= 14)
                Cloud = "☀";
            else if (_clouds is >= 15 and <= 40)
                Cloud = "⛅";
            else if (_clouds is >= 41 and <= 120)
                Cloud = "☁";
            await bot.SendTextMessageAsync(update.Message?.Chat.Id ?? 0, $"Температура в {_nameofCity}: {_tempOfCity} °C {Cloud}\nОщущается как {_fellsLikeOfCity} °C\n" +
                                                                           $"Влажность воздуха: {_humidity}%\nСкорость ветра: {_speed} м/с\n" +
                                                                           $"Атмосферное давление: {Math.Round(_pressure * 0.75)} мм рт.ст.\n" +
                                                                           $"Восход: {_sunRiseDate}\nЗакат: {_sunSetDate}", cancellationToken: cancellationToken);
            return;
        }

        async Task Weather(string? cityName, CancellationToken cancellationToken)
        {
            _logger.Debug("Try to get weather");

            string appid = _weatherOptions.EsureValidTokenWeather(AppName);

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
}
