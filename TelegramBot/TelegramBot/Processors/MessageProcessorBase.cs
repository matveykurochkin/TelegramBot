using NLog;
namespace TelegramBot.Processors;

public abstract class MessageProcessorBase
{

    protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    protected static readonly Random _random = new Random();

}