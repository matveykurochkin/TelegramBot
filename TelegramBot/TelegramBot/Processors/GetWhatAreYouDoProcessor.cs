using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Internal;

namespace TelegramBot.Processors;
[TelegramCommand("Что делаешь?", "что делаешь?", "Что делаешь", "чд", "Чд", "Чем занимаешься?", "чем занимаешься?", "чем занимаешься", "Чд?" )]
internal class GetWhatAreYouDoProcessor : MessageProcessorBase, ITelegramMessageProcessor
{
    public async Task ProcessMessage(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        _logger.Debug("Command WhatAreYouDo");
        var count = _random.Next(ArrDataClass.AnswWhatAreYouDoArr.Length);
        await bot.SendTextMessageAsync(update.Message?.Chat.Id ?? 0, $"{ArrDataClass.AnswWhatAreYouDoArr[count]}", cancellationToken: cancellationToken);
    }
}
