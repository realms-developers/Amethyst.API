using Amethyst.Hooks.Autoloading;
using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace Amethyst.Hooks.Args.Chat;

[AutoloadHook(true, true)]
public sealed class BroadcastTextArgs(NetworkText text, Color color, int excludedPlayer, byte messageAuthor)
{
    public NetworkText Text { get; set; } = text;
    public Color Color { get; set; } = color;
    public int ExcludedPlayer { get; set; } = excludedPlayer;
    public byte MessageAuthor { get; set; } = messageAuthor;
}
