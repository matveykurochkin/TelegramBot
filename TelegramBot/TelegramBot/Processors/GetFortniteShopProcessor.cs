using Telegram.Bot;
using Telegram.Bot.Types;
using Fortnite_API;
using TelegramBot.Internal;

namespace TelegramBot.Processors;

[TelegramCommand("Магазин Fortnite 🏦", "/shopfortnite")]
internal class GetFortniteShopProcessor : MessageProcessorBase, ITelegramMessageProcessor
{
    public async Task ProcessMessage(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        _logger.Debug("Get shop icon fortnite");

        var apiClient = new FortniteApiClient();
        var shopResponse = await apiClient.V2.Shop.GetBrCombinedAsync(language: Fortnite_API.Objects.GameLanguage.RU, token: cancellationToken);
        var count = _random.Next(ArrDataClass.SticerArr.Length);
        string? image, name, info, price;

        if (shopResponse.Status != 200)
            throw new InvalidOperationException($"Error call api: {shopResponse.Error}");

        await bot.SendTextMessageAsync(update.Message?.Chat.Id ?? 0, $"{update.Message?.From?.FirstName}, держи магазин предметов Королевской Битвы Fortnite! {ArrDataClass.SticerArr[count]}", cancellationToken: cancellationToken);
        for (int i = 0; i < shopResponse.Data.Featured.Entries.Count; i++)
        {
            for (int j = 0; j < shopResponse.Data.Featured.Entries[i].Items.Count; j++)
            {
                if (shopResponse.Data.Featured.Entries[i].Items[j] != null)
                {
                    image = shopResponse.Data.Featured.Entries[i].Items[j].Images.Icon.ToString();
                    name = shopResponse.Data.Featured.Entries[i].Items[j].Name;
                    info = shopResponse.Data.Featured.Entries[i].Items[j].Description;
                    price = shopResponse.Data.Featured.Entries[i + j].FinalPrice.ToString();
                    Thread.Sleep(150);
                    await bot.SendPhotoAsync(update.Message?.Chat.Id ?? 0, $"{image}", $"{name}\nЦена: {price} VB\n{info}", cancellationToken: cancellationToken);
                }
            }
        }
    }
}
