using Amethyst.Hooks.Autoloading;
using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace Amethyst.Hooks.Args.Chat;

[AutoloadHook(true, true)]
public sealed class BroadcastTextArgs
{
    public BroadcastTextArgs(NetworkText text, Color color, int excludedPlayer, byte messageAuthor)
    {
        Text = text;
        Color = color;
        ExcludedPlayer = excludedPlayer;
        MessageAuthor = messageAuthor;
    }

    public NetworkText Text { get; set; }
    public Color Color { get; set; }
    public int ExcludedPlayer { get; set; }
    public byte MessageAuthor { get; set; }
}
