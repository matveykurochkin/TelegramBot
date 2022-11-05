using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Web;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Configuration;
using TelegramBot.Internal;

namespace TelegramBot.Processors;

[TelegramCommand("Что надеть?", "/whatwear")]
public class GetWhatWearProcessor : MessageProcessorBase, ITelegramMessageProcessor
{
    private double _tempOfCity;
    private string? _nameofCity;
    WeatherOptions _weatherOptions = new WeatherOptions();
    public async Task ProcessMessage(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        _logger.Debug("What to wear answer");
        var count = _random.Next(ArrDataClass.SticerArr.Length);
        var AppName = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                                                .Build()
                                                .GetSection("APIWeather")["TokenWeatherID"];

        if (!string.IsNullOrEmpty(AppName))
        {
            _nameofCity = "Владимир";
            await WhatWear(cityName: _nameofCity, cancellationToken);


            if (_tempOfCity is <= 5 and <= 20)
            {
                await bot.SendTextMessageAsync(update.Message?.Chat.Id ?? 0, $"В {_nameofCity} сейчас {_tempOfCity}°C, это холодновато, советую надеть вещи потеплее! {ArrDataClass.SticerArr[count]}", cancellationToken: cancellationToken);
            }
            else if (_tempOfCity is >= 6 and <= 15)
            {
                await bot.SendTextMessageAsync(update.Message?.Chat.Id ?? 0, $"В {_nameofCity} сейчас {_tempOfCity}°C, это конечно тепло, но кофточку лучше накинуть! {ArrDataClass.SticerArr[count]}", cancellationToken: cancellationToken);
            }
            else if (_tempOfCity is >= 16 and <= 100)
            {
                await bot.SendTextMessageAsync(update.Message?.Chat.Id ?? 0, $"В {_nameofCity} сейчас {_tempOfCity}°C, это жарко, можешь расслабиться и надеть свою любимую футболочку с шортами! {ArrDataClass.SticerArr[count]}", cancellationToken: cancellationToken);
            }
        }

        async Task WhatWear(string? cityName, CancellationToken cancellationToken)
        {
            _logger.Debug("Try to get What Wear");

            string appid = _weatherOptions.EsureValidTokenWeather(AppName);

            try
            {
                var url = $"https://api.openweathermap.org/data/2.5/weather?q={HttpUtility.UrlEncode(cityName)}&appid={HttpUtility.UrlEncode(appid)}&units=metric";
                using var hc = new HttpClient();
                var weather = await hc.GetFromJsonAsync<WeatherResponse>(url, cancellationToken);
                if (weather != null)
                {
                    _tempOfCity = Math.Round(weather.main.temp);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error response weather");
            }
        }
    }
}
