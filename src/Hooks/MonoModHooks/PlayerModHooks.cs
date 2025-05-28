using Amethyst.Hooks.Args.Players;
using Amethyst.Server.Entities;
using Terraria;
using Terraria.Net.Sockets;

namespace Amethyst.Hooks.MonoModHooks;

public static class PlayerModHooks
{
    public static void AttachHooks()
    {
        On.Terraria.Netplay.OnConnectionAccepted += NetplayOnConnectionAccepted;
        On.Terraria.NetMessage.SyncDisconnectedPlayer += NetMessageSyncDisconnectedPlayer;
        On.Terraria.NetMessage.greetPlayer += NetMessageGreetPlayer;
    }

    public static void DeattachHooks()
    {
        On.Terraria.Netplay.OnConnectionAccepted -= NetplayOnConnectionAccepted;
        On.Terraria.NetMessage.SyncDisconnectedPlayer -= NetMessageSyncDisconnectedPlayer;
        On.Terraria.NetMessage.greetPlayer -= NetMessageGreetPlayer;
    }

    public static void NetMessageGreetPlayer(On.Terraria.NetMessage.orig_greetPlayer orig, int plr)
    {
        orig(plr);

        HookRegistry.GetHook<PlayerGreetArgs>()
            ?.Invoke(new PlayerGreetArgs(EntityTrackers.Players[plr]));
    }

    public static void NetMessageSyncDisconnectedPlayer(On.Terraria.NetMessage.orig_SyncDisconnectedPlayer orig, int plr)
    {
        orig(plr);

        HookRegistry.GetHook<PlayerSocketDisconnectArgs>()
            ?.Invoke(new PlayerSocketDisconnectArgs(plr));
    }

    public static void NetplayOnConnectionAccepted(On.Terraria.Netplay.orig_OnConnectionAccepted orig, ISocket client)
    {
        int plr = Netplay.FindNextOpenClientSlot();
        if (plr == -1)
        {
            return;
        }

        HookRegistry.GetHook<PlayerSocketConnectArgs>()
            ?.Invoke(new PlayerSocketConnectArgs(plr));

        Netplay.Clients[plr].Reset();
        Netplay.Clients[plr].Socket = client;
    }
}
