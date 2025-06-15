using Amethyst.Network.Structures;

namespace Amethyst.Systems.Chat.Base.Misc.Base;

public record MiscRenderedMessage<T>(string Text, NetColor Color, T Context);
