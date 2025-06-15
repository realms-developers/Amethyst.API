namespace Amethyst.Systems.Chat.Base.Misc.Base;

public interface IMiscMessageRenderer<T>
{
    public string Name { get; }

    public MiscRenderedMessage<T>? Render(T ctx);
}
