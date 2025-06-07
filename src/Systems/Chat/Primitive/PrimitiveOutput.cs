using Amethyst.Server.Entities.Players;
using Amethyst.Systems.Chat.Base;
using Amethyst.Systems.Chat.Base.Models;
using Amethyst.Text;

namespace Amethyst.Systems.Chat;

public sealed class PrimitiveOutput : IChatMessageOutput
{
    public string Name => "PrimitiveOutput";

    public void OutputMessage(MessageRenderResult message)
    {
        string Join(IReadOnlyDictionary<string, string> dict)
        {
            return string.Join(" ", dict.Select(kv => kv.Value));
        }

        string text = $"{Join(message.Prefix)}{Join(message.Name)}{Join(message.Suffix)}: {Join(message.Text)}";
        AmethystLog.Main.Info("Chat", text.RemoveColorTags());
        PlayerUtils.BroadcastText(text, message.Color.R, message.Color.G, message.Color.B);
    }
}
