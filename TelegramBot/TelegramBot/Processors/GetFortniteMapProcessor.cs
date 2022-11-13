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
    internal static async Task WarmCache(CancellationToken ct)
    {
        _logger.Info("Start warm image cache");
        try
        {
            var apiClient = new FortniteApiClient();
            var mapResponse = await apiClient.V1.Map.GetAsync(token: ct, language: GameLanguage.RU);

            if (mapResponse.Status != 200)
                throw new InvalidOperationException($"Error call api: {mapResponse.Error}");

            var imageLink = mapResponse.Data.Images.POIs.ToString();
            //скачиваем карту чтобы она попала в кеш но не используем её. Дальнейшие вызовы карты будут брать из кеша
            await ImageCache.GetImage(imageLink, ct);
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error warm image cache");
        }
        _logger.Info("Warm image cache done");
    }

    public async Task ProcessMessage(ITelegramBotClient telegramBotClient, Update update, CancellationToken cancellationToken)
    {
        _logger.Debug("Get map fortnite");
        var count = _random.Next(ArrDataClass.SticerArr.Length);
        var apiClient = new FortniteApiClient();
        var mapResponse = await apiClient.V1.Map.GetAsync(token: cancellationToken, language: GameLanguage.RU);

        if (mapResponse.Status != 200)
            throw new InvalidOperationException($"Error call api: {mapResponse.Error}");

        var imageLink = mapResponse.Data.Images.POIs.ToString();
        var imageData = await ImageCache.GetImage(imageLink, cancellationToken);
        if (imageData == null)
        {
            await telegramBotClient.SendTextMessageAsync(update.Message?.Chat.Id ?? 0, "Пока карты нету", cancellationToken: cancellationToken);
            return;
        }

        await using var ms = new MemoryStream(imageData);
        await telegramBotClient.SendPhotoAsync(update.Message?.Chat.Id ?? 0, new InputOnlineFile(ms), $"{update.Message?.From?.FirstName}, держи карту текущего сезона Fortnite {ArrDataClass.SticerArr[count]}", cancellationToken: cancellationToken);
    }
}