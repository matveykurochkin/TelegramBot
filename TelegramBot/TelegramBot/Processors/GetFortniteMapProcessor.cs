using Fortnite_API;
using Fortnite_API.Objects;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using TelegramBot.Internal;

namespace TelegramBot.Processors;

[TelegramCommand("Карта Fortnite 🗺", "/mapfortnite")]
public class GetFortniteMapProcessor : MessageProcessorBase, ITelegramMessageProcessor
{
    public async Task ProcessMessage(ITelegramBotClient telegramBotClient, Update update, CancellationToken cancellationToken)
    {
        _logger.Debug("Get map fortnite");
        var count = _random.Next(ArrDataClass.SticerArr.Length);
        var apiClient = new FortniteApiClient();
        var mapResponse = await apiClient.V1.Map.GetAsync(token: cancellationToken,language: GameLanguage.RU);

        if (mapResponse.Status != 200)
            throw new InvalidOperationException($"Error call api: {mapResponse.Error}");

        var imageLink = mapResponse.Data.Images.POIs.ToString();
        
        using var hc = new HttpClient();
        await using var photoStream = await hc.GetStreamAsync(imageLink, cancellationToken);
        await telegramBotClient.SendPhotoAsync(update.Message?.Chat.Id ?? 0, new InputOnlineFile(photoStream), $"Map Fortnite {ArrDataClass.SticerArr[count]}", cancellationToken: cancellationToken);
    }
}