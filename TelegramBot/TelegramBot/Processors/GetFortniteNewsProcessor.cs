using Telegram.Bot;
using Telegram.Bot.Types;
using Fortnite_API;
using TelegramBot.Internal;

namespace TelegramBot.Processors;

[TelegramCommand("Новости Fortnite 📰", "/newsfortnite")]
internal class GetFortniteNewsProcessor : MessageProcessorBase, ITelegramMessageProcessor
{
    public async Task ProcessMessage(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        _logger.Debug("Get news Buttle Roual and STW in Fortnite");

        var apiClient = new FortniteApiClient();
        var newsBRResponse = await apiClient.V2.News.GetAsync(token: cancellationToken, language: Fortnite_API.Objects.GameLanguage.RU);
        var newsSTWResponse = await apiClient.V2.News.GetStwAsync(token: cancellationToken, language: Fortnite_API.Objects.GameLanguage.RU);
        var count = _random.Next(ArrDataClass.SticerArr.Length);
        string? title, body, image;

        if (newsBRResponse.Status != 200 && newsSTWResponse.Status != 200)
            throw new InvalidOperationException($"Error call api: {newsBRResponse.Error}");

        await bot.SendTextMessageAsync(update.Message?.Chat.Id ?? 0, $"{update.Message?.From?.FirstName}, держи новости Королевской Битвы Fortnite! {ArrDataClass.SticerArr[count]}", cancellationToken: cancellationToken);
        for (var i = 0; i < newsBRResponse.Data.Br.Motds.Count; i++)
        {
            title = newsBRResponse.Data.Br.Motds[i].Title;
            body = newsBRResponse.Data.Br.Motds[i].Body;
            image = newsBRResponse.Data.Br.Motds[i].Image.ToString();
            Thread.Sleep(150);
            await bot.SendPhotoAsync(update.Message?.Chat.Id ?? 0, $"{image}", caption: $"{title}\n{body}", cancellationToken: cancellationToken);
        }

        await bot.SendTextMessageAsync(update.Message?.Chat.Id ?? 0, $"{update.Message?.From?.FirstName}, держи новости Сражения с Бурей в Fortnite! {ArrDataClass.SticerArr[count]}", cancellationToken: cancellationToken);
        for (var i = 0; i < newsSTWResponse.Data.Messages.Count; i++)
        {
            title = newsSTWResponse.Data.Messages[i].Title;
            body = newsSTWResponse.Data.Messages[i].Body;
            image = newsSTWResponse.Data.Messages[i].Image.ToString();
            Thread.Sleep(150);
            await bot.SendPhotoAsync(update.Message?.Chat.Id ?? 0, $"{image}", caption: $"{title}\n{body}", cancellationToken: cancellationToken);
        }

    }
}
