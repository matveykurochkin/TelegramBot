using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
{
    public class BotButtons
    {
        public static IReplyMarkup ButtonOnTGbot()
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
            },
            new[]
            {
                new KeyboardButton("Что сегодня надеть?")
            }
        });
            tgButton.ResizeKeyboard = true;
            return tgButton;
        }
        public static IReplyMarkup ButtonCityOnTGbotForChannel()
        {
            var tgButton = new ReplyKeyboardMarkup(new[]
            {
            new[]
            {
                new KeyboardButton($"{ArrDataClass.WeatherCity[0]}"),
                new KeyboardButton($"{ArrDataClass.WeatherCity[3]}"),
            },
            new[]
            {
                new KeyboardButton($"Игра «Угадай число»")
            },
            new[]
            {
                new KeyboardButton("⬅")
            }
        });
            tgButton.ResizeKeyboard = true;
            return tgButton;
        }

        public static IReplyMarkup ButtonOnGameEasy()
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
                new KeyboardButton($"Закончить игру"),
                new KeyboardButton($"Подсказка")
            }
        });
            tgButton.ResizeKeyboard = true;
            return tgButton;
        }
        public static IReplyMarkup ButtonOnGameMedium()
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
                new KeyboardButton($"Закончить игру"),
                new KeyboardButton($"Подсказка")
            }
        });
            tgButton.ResizeKeyboard = true;
            return tgButton;
        }

        public static IReplyMarkup ButtonOnGameHard()
        {
            var tgButton = new ReplyKeyboardMarkup(new[]
                        {
            new[]
            {
                new KeyboardButton($"0"),
                new KeyboardButton($"1"),
                new KeyboardButton($"2"),
                new KeyboardButton($"3")
            },
            new[]
            {
                new KeyboardButton($"4"),
                new KeyboardButton($"5"),
                new KeyboardButton($"6"),
                new KeyboardButton($"7")
            },
            new[]
            {
                new KeyboardButton($"8"),
                new KeyboardButton($"9"),
                new KeyboardButton($"10"),
                new KeyboardButton($"11")
            },
            new[]
            {
                new KeyboardButton($"12"),
                new KeyboardButton($"13"),
                new KeyboardButton($"14"),
                new KeyboardButton($"15")
            },
            new[]
            {
                new KeyboardButton($"Закончить игру"),
                new KeyboardButton($"Подсказка")
            }
        });
            tgButton.ResizeKeyboard = true;
            return tgButton;
        }

        public static IReplyMarkup difficultySelectionButton()
        {
            var tgButton = new ReplyKeyboardMarkup(new[]
            {
            new[]
            {
                new KeyboardButton($"Легко"),
                new KeyboardButton($"Средне"),
                new KeyboardButton($"Сложно"),
            },
            new[]
            {
                new KeyboardButton($"Закончить игру")
            }
        });
            tgButton.ResizeKeyboard = true;
            return tgButton;
        }


        public static IReplyMarkup ButtonOnChatTGbot(string City)
        {
            return new InlineKeyboardMarkup(new[]
            {
        new []
        {
            InlineKeyboardButton.WithSwitchInlineQueryCurrentChat(text: City,$"{City}"),
        }
        });
        }
        public static IReplyMarkup ButtonOnRequest()
        {
            return new InlineKeyboardMarkup(new[]
            {
        new []
        {
            InlineKeyboardButton.WithSwitchInlineQueryCurrentChat(text: "Отменить поиск","Отменить поиск"),
        }
        });
        }
    }
}
