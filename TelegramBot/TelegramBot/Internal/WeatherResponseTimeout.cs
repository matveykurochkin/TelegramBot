using Microsoft.Extensions.Configuration;

namespace TelegramBot.Internal;

public static class WeatherResponseTimeout
{
    private static string? timeOut = new ConfigurationBuilder().AddJsonFile("appsettings.json")
        .Build()
        .GetSection("TimeOutWeatherResponse")["timeOut"];
    public static HttpClient TimeOut()
    {
        HttpClient hc;
        if (double.TryParse(timeOut, out var parsedNumber) && parsedNumber > 0)
        {
            hc = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(parsedNumber)
            };
            return hc;
        }

        hc = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(5)
        };
        return hc;
    }
}