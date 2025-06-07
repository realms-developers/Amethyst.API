namespace Amethyst.Systems.Chat.Misc.Base;

public interface IMiscMessageRenderer<T>
{
    public string Name { get; }

    public MiscRenderedMessage<T>? Render(T ctx);
}
