using Telegram.Bot;
using Telegram.Bot.Types;
using Fortnite_API;
using TelegramBot.Internal;

namespace TelegramBot.Processors;

[TelegramCommand("Наборы Fortnite 🏯", "/bundlefortnite")]
internal class GetFortniteBundleShopProcessor : MessageProcessorBase, ITelegramMessageProcessor
{
    public async Task ProcessMessage(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        _logger.Debug("Get shop bundle icon fortnite");

        var apiClient = new FortniteApiClient();
        var shopResponse = await apiClient.V2.Shop.GetBrCombinedAsync(language: Fortnite_API.Objects.GameLanguage.RU);
        var count = _random.Next(ArrDataClass.SticerArr.Length);
        string? imageLink, name, info, price, salePrice;

        if (shopResponse.Status != 200)
            throw new InvalidOperationException($"Error call api: {shopResponse.Error}");

        await bot.SendTextMessageAsync(update.Message?.Chat.Id ?? 0, $"{update.Message?.From?.FirstName}, держи наборы из магазина предметов Королевской Битвы Fortnite! {ArrDataClass.SticerArr[count]}", cancellationToken: cancellationToken);
        for (int i = 0; i < shopResponse.Data.Featured.Entries.Count; i++)
        {
            if (shopResponse.Data.Featured.Entries[i].Bundle != null)
            {
                //imageLink = shopResponse.Data.Featured.Entries[i].NewDisplayAsset.MaterialInstances[i].Images.ElementAt(i).Values[i].ToString();
                imageLink = shopResponse.Data.Featured.Entries[i].Bundle.Image.ToString();
                name = shopResponse.Data.Featured.Entries[i].Bundle.Name;
                info = shopResponse.Data.Featured.Entries[i].Bundle.Info;
                price = shopResponse.Data.Featured.Entries[i].RegularPrice.ToString();
                salePrice = shopResponse.Data.Featured.Entries[i].FinalPrice.ToString();
                Thread.Sleep(150);
                await bot.SendPhotoAsync(update.Message?.Chat.Id ?? 0, $"{imageLink}", $"{name}\nЦена без скидки: {price} VB\nЦена со скидкой: {salePrice} VB\n{info}", cancellationToken: cancellationToken);
            }
        }
    }
}
