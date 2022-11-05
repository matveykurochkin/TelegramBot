using System.Collections.Concurrent;
using System.Reflection;
using NLog;
using TelegramBot.Internal;

namespace TelegramBot;

public class MessageProcessorFactory
{
    // ReSharper disable once InconsistentNaming
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    
    // ReSharper disable once InconsistentNaming
    private static readonly ConcurrentDictionary<Type, ITelegramMessageProcessor> _processorsCache = new();

    private class ProcessorConfigItem
    {
        public ProcessorConfigItem(HashSet<string> messages, Type processorType)
        {
            Messages = messages;
            ProcessorType = processorType;
        }

        internal HashSet<string> Messages { get; }
        internal Type ProcessorType { get; }
    }

    private readonly List<ProcessorConfigItem> _processors;

    public MessageProcessorFactory()
    {
        var currAssembly = Assembly.GetExecutingAssembly();
        var processorTypes = currAssembly.GetTypes().Where(t => t.IsClass && t.IsAssignableTo(typeof(ITelegramMessageProcessor)));
        
        _processors = new List<ProcessorConfigItem>();
        
        _logger.Debug("Try to find message processors");
        foreach (var processorType in processorTypes)
        {
            var tca = processorType.GetCustomAttribute<TelegramCommandAttribute>();
            if (tca != null)
            {
                _logger.Debug("Founded processor [{processor}], supports commands: [{commands}]", processorType, tca.SupportedCommands);
                _processors.Add(new (tca.SupportedCommands, processorType));
            }
        }
    }

    public ITelegramMessageProcessor? GetProcessor(string? message)
    {
        if (string.IsNullOrEmpty(message)) return null;

        ITelegramMessageProcessor? processor = null;

        foreach (var processorConfigItem in _processors)
        {
            if (!processorConfigItem.Messages.Contains(message)) continue;
            
            var createdProcessor = _processorsCache.GetOrAdd(processorConfigItem.ProcessorType, processorType =>
            {
                var rv = Activator.CreateInstance(processorType) as ITelegramMessageProcessor;
                // ReSharper disable once NotResolvedInText
                if (rv == null) throw new ArgumentNullException("ProcessorType", $"Can't create processor with type [{processorType}]");
                return rv;
            });

            processor = createdProcessor;
            break;
        }

        return processor;
    }
}