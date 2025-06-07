using Amethyst.Network.Structures;
using Amethyst.Server.Entities.Players;

namespace Amethyst.Systems.Chat.Base.Models;

public sealed class MessageRenderContext
{
    internal MessageRenderContext(
        PlayerEntity player,
        PlayerMessage message)
    {
        Player = player;
        Message = message;
    }

    public PlayerEntity Player { get; }
    public PlayerMessage Message { get; }

    public Dictionary<string, string> Prefix { get; } = new();
    public Dictionary<string, string> Name { get; } = new();
    public Dictionary<string, string> Suffix { get; } = new();
    public Dictionary<string, string> Text { get; } = new();
    public NetColor Color { get; set; } = new NetColor(255, 255, 255);

    public MessageRenderResult Build()
    {
        return new MessageRenderResult(
            Player,
            Prefix.AsReadOnly(),
            Name.AsReadOnly(),
            Suffix.AsReadOnly(),
            Text.AsReadOnly(),
            Color);
    }
}
