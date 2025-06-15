namespace Amethyst.Systems.Chat.Base.Misc.Base;

public interface IMiscOutput<T>
{
    string Name { get; }

    void OutputMessage(MiscRenderedMessage<T> message);
}
