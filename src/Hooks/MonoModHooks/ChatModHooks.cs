using Amethyst.Hooks.Args.Chat;
using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace Amethyst.Hooks.MonoModHooks;

public static class ChatModHooks
{
    public static void AttachHooks()
    {
        On.Terraria.Chat.ChatHelper.BroadcastChatMessageAs += ChatHelperBroadcastChatMessageAs;
    }

    public static void DeattachHooks()
    {
        On.Terraria.Chat.ChatHelper.BroadcastChatMessageAs -= ChatHelperBroadcastChatMessageAs;
    }

    private static void ChatHelperBroadcastChatMessageAs(On.Terraria.Chat.ChatHelper.orig_BroadcastChatMessageAs orig, byte messageAuthor, NetworkText text, Color color, int excludedPlayer)
    {
        var args = new BroadcastTextArgs(text, color, excludedPlayer, messageAuthor);
        var result = HookRegistry.GetHook<BroadcastTextArgs>()?.Invoke(args);

        if (result == null)
        {
            throw new InvalidOperationException("No hook found for BroadcastTextArgs.");
        }

        if (result.IsCancelled == true)
        {
            return;
        }

        if (result.IsModified == true && result.Args != null)
        {
            text = result.Args.Text;
            color = result.Args.Color;
            excludedPlayer = result.Args.ExcludedPlayer;
            messageAuthor = result.Args.MessageAuthor;
        }

        orig(messageAuthor, text, color, excludedPlayer);
    }

}
