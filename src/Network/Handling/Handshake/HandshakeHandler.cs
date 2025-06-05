using Amethyst.Hooks;
using Amethyst.Hooks.Args.Players;
using Amethyst.Kernel;
using Amethyst.Network.Packets;
using Amethyst.Network.Structures;
using Amethyst.Server.Entities;
using Amethyst.Server.Entities.Players;
using Amethyst.Systems.Users;
using Amethyst.Systems.Users.Players;
using MongoDB.Driver;
using Terraria;

namespace Amethyst.Network.Handling.Handshake;

public static class HandshakeHandler
{
    public static UnconnectedSuspension Suspension { get; } = new();

    internal static void Initialize()
    {
        NetworkManager.SetMainHandler<PlayerConnectRequest>(OnPlayerConnectRequest);
        NetworkManager.SetMainHandler<PlayerUUID>(OnPlayerUUIDRequest);
        NetworkManager.SetMainHandler<PlayerRequestWorldInfo>(OnPlayerRequestWorldInfo);
        NetworkManager.SetMainHandler<PlayerRequestSection>(OnPlayerRequestSection);

        NetworkManager.AddHandler<PlayerInfo>(OnPlayerInfoRequest);
        NetworkManager.AddHandler<PlayerSpawn>(OnPlayerSpawn);
    }

    public static void OnPlayerSpawn(PlayerEntity plr, ref PlayerSpawn packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.WaitingPlayerSpawn)
        {
            return;
        }


        plr.SendPacketBytes(PlayerConnectionPrepareWorldPacket.Serialize(new()));
        plr.Phase = ConnectionPhase.Connected;

        HookRegistry.GetHook<PlayerFullyJoinedArgs>()
            ?.Invoke(new PlayerFullyJoinedArgs(plr));
    }

    public static void OnPlayerRequestSection(PlayerEntity plr, ref PlayerRequestSection packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.WaitingSectionRequest)
        {
            return;
        }

        PacketSendingUtility.SendFullWorld(plr, packet.TileX, packet.TileY);

        plr.SendPacketBytes(PlayerFinishedConnectionPacket.Serialize(new()));
        plr.Phase = ConnectionPhase.WaitingPlayerSpawn;
    }

    public static void OnPlayerRequestWorldInfo(PlayerEntity plr, ref PlayerRequestWorldInfo packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.WaitingWorldInfoRequest)
        {
            return;
        }

        plr.SendPacketBytes(PacketSendingUtility.CreateWorldInfoPacket());

        // TODO: sync invasion

        plr.Phase = ConnectionPhase.WaitingSectionRequest;
    }

    public static void OnPlayerUUIDRequest(PlayerEntity plr, ref PlayerUUID packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.WaitingUUID)
        {
            return;
        }

        var cfg = HandshakeConfiguration.Instance;
        string uuid = packet.UUID;

        if (string.IsNullOrEmpty(packet.UUID))
        {
            plr.Kick("network.invalidUUID");
            AmethystLog.Network.Error(nameof(HandshakeHandler), $"Player #{plr.Index} tried to connect with an empty UUID");
            return;
        }

        if (EntityTrackers.Players.Count(p => p.UUID != "" && p.UUID == uuid) > cfg.MaxUsersWithSameUUID)
        {
            plr.Kick("network.tooManyUsersWithSameUUID");
            AmethystLog.Network.Error(nameof(HandshakeHandler), $"Player #{plr.Index} tried to connect with a UUID that is already in use: {uuid}");
            return;
        }

        if (cfg.DiscardInvalidGUIDs && !Guid.TryParse(uuid, out _))
        {
            plr.Kick("network.invalidUUID");
            AmethystLog.Network.Error(nameof(HandshakeHandler), $"Player #{plr.Index} tried to connect with an invalid UUID: {uuid}");
            return;
        }

        plr.UUID = uuid;

        plr.Phase = ConnectionPhase.WaitingWorldInfoRequest;

        PlayerUserMetadata userMetadata = new PlayerUserMetadata(plr.Name, plr.IP, plr.UUID, plr.Index);
        PlayerUser user = UsersOrganizer.PlayerUsers.CreateUser(userMetadata);

        user.Suspensions!.Suspend(Suspension);
    }

    public static void OnPlayerInfoRequest(PlayerEntity plr, ref PlayerInfo packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.WaitingPlayerInfo)
        {
            return;
        }

        var cfg = HandshakeConfiguration.Instance;

        string name = packet.Name;
        if (EntityTrackers.Players.Any(p => p.Name == name) && cfg.AllowDuplicateNicknames == false)
        {
            plr.Kick("network.duplicateNickname");
            AmethystLog.Network.Error(nameof(HandshakeHandler), $"Player #{plr.Index} tried to connect with a duplicate nickname: {name}");
            return;
        }

        foreach (var banword in cfg.NicknameBanwords)
        if (name.Contains(banword, StringComparison.OrdinalIgnoreCase))
        {
            plr.Kick("network.bannedNickname");
            AmethystLog.Network.Error(nameof(HandshakeHandler), $"Player #{plr.Index} tried to connect with a banned nickname: {name}");
            return;
        }

        IEnumerable<char> invalidChars = name.Where(c => !cfg.NicknameFilter.Contains(c));
        if (cfg.EnableNicknameFilter && invalidChars.Any())
        {
            plr.Kick(Localization.Get("network.invalidCharsNickname", AmethystSession.Profile.DefaultLanguage)
                + $" ({string.Join(", ", invalidChars)})");

            AmethystLog.Network.Error(nameof(HandshakeHandler), $"Player #{plr.Index} tried to connect with an invalid nickname: {name}");
            return;
        }

        if (name.Length < cfg.MinNicknameLength || name.Length > cfg.MaxNicknameLength)
        {
            plr.Kick(Localization.Get("network.invalidLengthNickname", AmethystSession.Profile.DefaultLanguage)
                + $" ({cfg.MinNicknameLength}-{cfg.MaxNicknameLength})");

            AmethystLog.Network.Error(nameof(HandshakeHandler), $"Player #{plr.Index} tried to connect with an invalid nickname length: {name}");
            return;
        }

        int difficulty = 0;
        NetBitsByte difficultyFlags = packet.Flags;
        difficulty = difficultyFlags[0] ? 1 : difficultyFlags[1] ? 2 : difficultyFlags[2] ? 3 : 0;

        if (!cfg.AllowCreativeInNormalWorld && !Main.GameModeInfo.IsJourneyMode && difficulty == 3)
        {
            plr.Kick("network.creativeInNormalWorld");
            AmethystLog.Network.Error(nameof(HandshakeHandler), $"Player #{plr.Index} tried to connect with Creative mode in a normal world: {name}");
            return;
        }
        if (!cfg.AllowNormalInCreativeWorld && Main.GameModeInfo.IsJourneyMode && difficulty != 3)
        {
            plr.Kick("network.normalInCreativeWorld");
            AmethystLog.Network.Error(nameof(HandshakeHandler), $"Player #{plr.Index} tried to connect with Normal mode in a Creative world: {name}");
            return;
        }

        plr.Name = packet.Name;
        plr.Phase = ConnectionPhase.WaitingUUID;
    }

    public static void OnPlayerConnectRequest(PlayerEntity plr, ref PlayerConnectRequest packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.WaitingProtocol)
        {
            return;
        }

        var cfg = HandshakeConfiguration.Instance;

        if ((cfg.AllowedProtocols.Length > 0 && !cfg.AllowedProtocols.Contains(packet.Protocol)) ||
            (cfg.AllowedProtocols.Length == 0 && packet.Protocol != "Terraria" + Main.curRelease))
        {
            AmethystLog.Network.Error(nameof(HandshakeHandler), $"Player #{plr.Index} tried to connect with an unsupported protocol: {packet.Protocol}");
            return;
        }

        if (cfg.BannedProtocols.Contains(packet.Protocol))
        {
            AmethystLog.Network.Error(nameof(HandshakeHandler), $"Player #{plr.Index} tried to connect with a banned protocol: {packet.Protocol}");
            return;
        }

        if ((cfg.DiscardUnwhitelistedIPs && !cfg.IPWhitelist.Contains(plr.IP)) ||
            cfg.IPBlacklist.Contains(plr.IP))
        {
            AmethystLog.Network.Error(nameof(HandshakeHandler), $"Player #{plr.Index} tried to connect from a non-allowed IP: {plr.IP}");
            return;
        }

        byte[] data = PlayerNetIndexPacket.Serialize(new PlayerNetIndex()
        {
            PlayerIndex = (byte)plr.Index
        });

        plr.SendPacketBytes(data);
        plr.Phase = ConnectionPhase.WaitingPlayerInfo;
    }
}
