namespace TelegramBot.Internal;

[AttributeUsage(AttributeTargets.Class)]
internal class TelegramCommandAttribute : Attribute
{
    internal HashSet<string> SupportedCommands { get; }

    public TelegramCommandAttribute(params string[] commands)
    {
        SupportedCommands = new HashSet<string>(commands);
    }
}