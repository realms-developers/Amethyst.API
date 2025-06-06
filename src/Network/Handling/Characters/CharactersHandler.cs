using Amethyst.Network.Packets;
using Amethyst.Server.Entities.Players;

namespace Amethyst.Network.Handling.Characters;

public static class CharactersHandler
{
    internal static void Initialize()
    {
        NetworkManager.AddHandler<PlayerInfo>(OnPlayerInfo);
        NetworkManager.AddHandler<PlayerSlot>(OnPlayerSlot);
        NetworkManager.AddHandler<PlayerLife>(OnPlayerLife);
        NetworkManager.AddHandler<PlayerMana>(OnPlayerMana);
        NetworkManager.AddHandler<PlayerTownNPCQuestsStats>(OnPlayerQuests);
    }

    private static void OnPlayerQuests(PlayerEntity plr, ref PlayerTownNPCQuestsStats packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != Handshake.ConnectionPhase.Connected)
            return;

        plr.User?.Character?.Handler.HandleQuests(packet);
    }

    private static void OnPlayerMana(PlayerEntity plr, ref PlayerMana packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != Handshake.ConnectionPhase.Connected)
            return;

        plr.User?.Character?.Handler.HandleSetMana(packet);
    }

    private static void OnPlayerLife(PlayerEntity plr, ref PlayerLife packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != Handshake.ConnectionPhase.Connected)
            return;

        plr.User?.Character?.Handler.HandleSetLife(packet);
    }

    private static void OnPlayerSlot(PlayerEntity plr, ref PlayerSlot packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != Handshake.ConnectionPhase.Connected)
            return;

        plr.User?.Character?.Handler.HandleSlot(packet);
    }

    private static void OnPlayerInfo(PlayerEntity plr, ref PlayerInfo packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != Handshake.ConnectionPhase.Connected)
            return;

        plr.User?.Character?.Handler.HandlePlayerInfo(packet);
    }
}
