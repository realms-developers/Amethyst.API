using Amethyst.Network.Structures;

namespace Amethyst.Systems.Chat.Misc.Base;

public record MiscRenderedMessage<T>(string Text, NetColor Color, T Context);
