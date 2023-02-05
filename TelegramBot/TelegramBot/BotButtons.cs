using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot;
public class BotButtons
{
    public static IReplyMarkup MainButtonOnBot()
    {
        var tgButton = new ReplyKeyboardMarkup(new[]
        {
        new[]
        {
            new KeyboardButton("Посмотреть погоду ⛅"),
        },
        new[]
        {
            new KeyboardButton("Fortnite меню 👾")
        },
        new[]
        {
            new KeyboardButton("Посмотреть другие фишки 😉")
        }
    });
        tgButton.ResizeKeyboard = true;
        return tgButton;
    }

    public static IReplyMarkup ButtonBR()
    {
        var tgButton = new ReplyKeyboardMarkup(new[]
        {
        new[]
        {
           new KeyboardButton("Карта Fortnite 🗺"),
           new KeyboardButton("Магазин Fortnite 🏦")
        },
        new[]
        {
          new KeyboardButton("Наборы Fortnite 🏯"),
          new KeyboardButton("Новости Fortnite 📰")
        },
        new[]
        {
            new KeyboardButton($"Назад ⬅")
        }
    });
        tgButton.ResizeKeyboard = true;
        return tgButton;
    }

    public static IReplyMarkup OtherButtonOnBot()
    {
        var tgButton = new ReplyKeyboardMarkup(new[]
        {
        new[]
        {
            new KeyboardButton("Привет!"),
            new KeyboardButton("Как дела?")
        },
        new[]
        {
           new KeyboardButton("Что делаешь?"),
           new KeyboardButton("Что надеть?")
        },
        new[]
        {
            new KeyboardButton("Скинуть пикчу 🗿"),
            new KeyboardButton("Скинуть стикос 😉"),
        },
        new[]
        {
            new KeyboardButton($"Назад ⬅")
        }
    });
        tgButton.ResizeKeyboard = true;
        return tgButton;
    }

    public static IReplyMarkup ButtonWeather()
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
            new KeyboardButton($"{ArrDataClass.WeatherCity[4]}"),
            new KeyboardButton($"{ArrDataClass.WeatherCity[5]}"),
        },
        new[]
        {
            new KeyboardButton($"{ArrDataClass.WeatherCity[6]}"),
            new KeyboardButton($"{ArrDataClass.WeatherCity[7]}"),
        },
        new[]
        {
            new KeyboardButton($"Назад ⬅")
        }
     });
        tgButton.ResizeKeyboard = true;
        return tgButton;
    }
}
