using Amethyst.Players;
using Microsoft.Xna.Framework;
using On.Terraria.Chat;
using Terraria;
using Terraria.Localization;
using Terraria.Net.Sockets;

namespace Amethyst.Infrastructure;

public static class AmethystHooks
{
    internal static void Initialize()
    {
        Players.Initialize();
        Chat.Initialize();
    }

    internal static void SafeInvoke(string name, Action invokeAction)
    {
        try
        {
            invokeAction();
        }
        catch (Exception ex)
        {
            AmethystLog.Main.Error(nameof(AmethystHooks), $"Exception in handling '{name}' hook:");
            AmethystLog.Main.Error(nameof(AmethystHooks), ex.ToString());
        }
    }

    public static class Chat
    {
        internal static void Initialize() => ChatHelper.BroadcastChatMessageAs += ChatHelperBroadcastChatMessageAs;

        private static void ChatHelperBroadcastChatMessageAs(ChatHelper.orig_BroadcastChatMessageAs orig, byte messageAuthor, NetworkText text, Color color, int excludedPlayer)
        {
            BroadcastArgs args = new(messageAuthor, text, color, excludedPlayer);
            InvokeOnBroadcast(args);

            if (args.Ignore)
            {
                return;
            }

            orig(args.Author, args.Text, args.Color, args.ExcludedPlayer);
        }

        public static event BroadcastHook? OnBroadcast;
        public static void InvokeOnBroadcast(BroadcastArgs args)
            => SafeInvoke("OnBroadcast", () => OnBroadcast?.Invoke(args));

        public delegate void BroadcastHook(BroadcastArgs args);
        public sealed class BroadcastArgs(byte author, NetworkText text, Color color, int excludedPlayer)
        {
            public byte Author { get; set; } = author;
            public NetworkText Text { get; set; } = text;
            public Color Color { get; set; } = color;
            public int ExcludedPlayer { get; set; } = excludedPlayer;

            public bool Ignore { get; set; }
        }
    }

    public static class Players
    {
        internal static void Initialize()
        {
            On.Terraria.Netplay.OnConnectionAccepted += NetplayOnConnectionAccepted;
            On.Terraria.NetMessage.SyncDisconnectedPlayer += NetMessageSyncDisconnectedPlayer;
            On.Terraria.NetMessage.greetPlayer += NetMessageGreetPlayer;
        }

        private static void NetMessageGreetPlayer(On.Terraria.NetMessage.orig_greetPlayer orig, int plr)
        {
            InvokeOnPlayerGreet(PlayerManager.Tracker[plr]);
        }

        private static void NetMessageSyncDisconnectedPlayer(On.Terraria.NetMessage.orig_SyncDisconnectedPlayer orig, int plr)
        {
            orig(plr);

            InvokeOnPlayerDisconnected(PlayerManager.Tracker[plr]);
        }
        private static void NetplayOnConnectionAccepted(On.Terraria.Netplay.orig_OnConnectionAccepted orig, ISocket client)
        {
            int plr = Netplay.FindNextOpenClientSlot();
            if (plr == -1)
            {
                return;
            }

            PlayerManager.Tracker.CreateInstance(plr);
            InvokeOnPlayerConnected(PlayerManager.Tracker[plr]);

            Netplay.Clients[plr].Reset();
            Netplay.Clients[plr].Socket = client;
        }

        public static event PlayerConnectedHook? OnPlayerConnected;
        public static void InvokeOnPlayerConnected(NetPlayer plr)
            => SafeInvoke("OnPlayerConnected", () => OnPlayerConnected?.Invoke(plr));

        public static event PlayerDisconnectedHook? OnPlayerDisconnected;
        public static void InvokeOnPlayerDisconnected(NetPlayer plr)
            => SafeInvoke("OnPlayerDisconnected", () => OnPlayerDisconnected?.Invoke(plr));

        public static event PlayerGreetHook? OnPlayerGreet;
        public static void InvokeOnPlayerGreet(NetPlayer plr)
            => SafeInvoke("OnPlayerGreet", () => OnPlayerGreet?.Invoke(plr));

        public delegate void PlayerConnectedHook(NetPlayer player);
        public delegate void PlayerDisconnectedHook(NetPlayer player);
        public delegate void PlayerGreetHook(NetPlayer player);
    }
}
