namespace Amethyst.Systems.Chat.Misc.Base;

public interface IMiscOutput<T>
{
    string Name { get; }

    void OutputMessage(MiscRenderedMessage<T> message);
}
