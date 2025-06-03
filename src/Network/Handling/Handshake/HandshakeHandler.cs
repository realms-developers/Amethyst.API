using Amethyst.Kernel;
using Amethyst.Network.Packets;
using Amethyst.Network.Structures;
using Amethyst.Server.Entities;
using Amethyst.Server.Entities.Players;
using Amethyst.Systems.Users;
using Amethyst.Systems.Users.Players;
using Terraria;
using Terraria.ID;

namespace Amethyst.Network.Handling.Handshake;

public static class HandshakeHandler
{
    public static UnconnectedSuspension Suspension { get; } = new();

    internal static void Initialize()
    {
        NetworkManager.SetMainHandler<PlayerConnectRequest>(OnPlayerConnectRequest);
        NetworkManager.SetMainHandler<PlayerInfo>(OnPlayerInfoRequest);
        NetworkManager.SetMainHandler<PlayerUUID>(OnPlayerUUIDRequest);
        NetworkManager.SetMainHandler<PlayerRequestWorldInfo>(OnPlayerRequestWorldInfo);
    }

    private static void OnPlayerRequestWorldInfo(PlayerEntity plr, ref PlayerRequestWorldInfo packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.ConnectionPhase != ConnectionPhase.WaitingWorldInfoRequest)
        {
            return;
        }

        byte[] data = PacketSendingUtility.CreateWorldInfoPacket();
        plr.SendPacketBytes(data);

        plr.ConnectionPhase = ConnectionPhase.WaitingSectionRequest;
    }

    private static void OnPlayerUUIDRequest(PlayerEntity plr, ref PlayerUUID packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.ConnectionPhase != ConnectionPhase.WaitingUUID)
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
        plr.ConnectionPhase = ConnectionPhase.WaitingWorldInfoRequest;

        PlayerUserMetadata userMetadata = new PlayerUserMetadata(plr.Name, plr.IP, plr.UUID, plr.Index);
        PlayerUser user = UsersOrganizer.PlayerUsers.CreateUser(userMetadata);

        user.Suspensions!.Suspend(Suspension);
    }

    private static void OnPlayerInfoRequest(PlayerEntity plr, ref PlayerInfo packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.ConnectionPhase != ConnectionPhase.WaitingPlayerInfo)
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
        plr.ConnectionPhase = ConnectionPhase.WaitingUUID;
        AmethystLog.Network.Info(nameof(HandshakeHandler), $"Player #{plr.Index} was identified as '{packet.Name}'");
    }

    public static void OnPlayerConnectRequest(PlayerEntity plr, ref PlayerConnectRequest packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.ConnectionPhase != ConnectionPhase.WaitingProtocol)
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
        AmethystLog.Network.Info(nameof(HandshakeHandler), $"Player #{plr.Index} connected with protocol: {packet.Protocol}");
        plr.ConnectionPhase = ConnectionPhase.WaitingPlayerInfo;
    }
}
