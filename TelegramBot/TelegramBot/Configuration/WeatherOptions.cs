namespace TelegramBot.Configuration;
internal class WeatherOptions
{
    public string? TokenWeatherID { get; set; }
    public string EsureValidTokenWeather(string tokenWeatherID)
    {
        TokenWeatherID = tokenWeatherID;
        if (!string.IsNullOrEmpty(TokenWeatherID))
            return TokenWeatherID;
        return "-1";
    }
}
