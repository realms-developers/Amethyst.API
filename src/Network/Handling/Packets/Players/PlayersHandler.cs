using Amethyst.Hooks;
using Amethyst.Hooks.Args.Players;
using Amethyst.Hooks.Context;
using Amethyst.Network.Handling.Base;
using Amethyst.Network.Handling.Packets.Handshake;
using Amethyst.Network.Packets;
using Amethyst.Network.Structures;
using Amethyst.Network.Utilities;
using Amethyst.Server.Entities;
using Amethyst.Server.Entities.Players;
using Amethyst.Systems.Chat;
using Terraria;

namespace Amethyst.Network.Handling.Packets.Players;

public sealed class PlayersHandler : INetworkHandler
{
    public string Name => "net.amethyst.PlayersHandler";

    public void Load()
    {
        NetworkManager.SetMainHandler<PlayerUpdate>(OnPlayerUpdate);
        NetworkManager.SetMainHandler<PlayerItemRotation>(OnPlayerItemRotation);
        NetworkManager.SetMainHandler<PlayerSpawn>(OnPlayerSpawn);
        NetworkManager.SetMainHandler<PlayerHurt>(OnPlayerHurt);
        NetworkManager.SetMainHandler<PlayerDeath>(OnPlayerDeath);
        NetworkManager.SetMainHandler<PlayerAddBuff>(OnPlayerAddBuff);
        NetworkManager.SetMainHandler<PlayerDodge>(OnPlayerDodge);
        NetworkManager.SetMainHandler<PlayerLifeHealEffect>(OnPlayerHeal);
        NetworkManager.SetMainHandler<PlayerManaHealEffect>(OnPlayerMana);
        NetworkManager.SetMainHandler<PlayerMinionAttackNPC>(OnPlayerMinionAttackNPC);
        NetworkManager.SetMainHandler<PlayerMinionRestPoint>(OnPlayerMinionRestPoint);
        NetworkManager.SetMainHandler<PlayerPlayItemSound>(OnPlayerPlayItemSound);
        NetworkManager.SetMainHandler<PlayerPvP>(OnPlayerPvP);
        NetworkManager.SetMainHandler<PlayerSetTeam>(OnPlayerSetTeam);
        NetworkManager.SetMainHandler<PlayerStealth>(OnPlayerStealth);
        NetworkManager.SetMainHandler<PlayerSyncBuffs>(OnPlayerSyncBuffs);
        NetworkManager.SetMainHandler<PlayerTalkNPC>(OnPlayerTalkNPC);
        NetworkManager.SetMainHandler<PlayerZone>(OnPlayerZone);

        HookRegistry.GetHook<PlayerFullyJoinedArgs>()
            ?.Register(SyncPlayer);
    }

    private void OnPlayerZone(PlayerEntity plr, ref PlayerZone packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
        {
            return;
        }

        plr.Zone1 = packet.Zone1;
        plr.Zone2 = packet.Zone2;
        plr.Zone3 = packet.Zone3;
        plr.Zone4 = packet.Zone4;
        plr.Zone5 = packet.Zone5;

        if (HandlersConfiguration.Instance.SyncPlayers)
        {
            PacketSendingUtility.ExcludeBroadcastConnected(plr.Index, PlayerZonePacket.Serialize(packet with { PlayerIndex = (byte)plr.Index }));
        }
    }

    private void OnPlayerTalkNPC(PlayerEntity plr, ref PlayerTalkNPC packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
        {
            return;
        }

        plr.TalkNPC = packet.NPCIndex;

        if (HandlersConfiguration.Instance.SyncPlayers)
        {
            PacketSendingUtility.ExcludeBroadcastConnected(plr.Index, PlayerTalkNPCPacket.Serialize(packet with { PlayerIndex = (byte)plr.Index }));
        }
    }

    private void OnPlayerSyncBuffs(PlayerEntity plr, ref PlayerSyncBuffs packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
        {
            return;
        }

        if (packet.PlayerIndex != plr.Index)
        {
            ignore = true;
            return;
        }

        if (HandlersConfiguration.Instance.SyncPlayers)
        {
            PacketSendingUtility.ExcludeBroadcastConnected(plr.Index, PlayerSyncBuffsPacket.Serialize(packet));
        }
    }

    private void OnPlayerStealth(PlayerEntity plr, ref PlayerStealth packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
        {
            return;
        }

        if (packet.PlayerIndex != plr.Index)
        {
            ignore = true;
            return;
        }

        packet.Stealth = Math.Clamp(packet.Stealth, 0f, 1f);

        if (HandlersConfiguration.Instance.SyncPlayers)
        {
            PacketSendingUtility.ExcludeBroadcastConnected(plr.Index, PlayerStealthPacket.Serialize(packet));
        }
    }

    private void OnPlayerSetTeam(PlayerEntity plr, ref PlayerSetTeam packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
        {
            return;
        }

        if (packet.PlayerIndex != plr.Index)
        {
            ignore = true;
            return;
        }

        plr.Team = packet.TeamIndex;
        ServerChat.MessagePlayerTeam.Invoke(new(plr, (byte)plr.Team));

        if (HandlersConfiguration.Instance.SyncPlayers)
        {
            PacketSendingUtility.ExcludeBroadcastConnected(plr.Index, PlayerSetTeamPacket.Serialize(packet));
        }
    }

    private void OnPlayerPvP(PlayerEntity plr, ref PlayerPvP packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
        {
            return;
        }

        if (packet.PlayerIndex != plr.Index)
        {
            ignore = true;
            return;
        }

        plr.IsInPvP = packet.IsInPvP;
        ServerChat.MessagePlayerPvP.Invoke(new(plr, plr.IsInPvP));

        if (HandlersConfiguration.Instance.SyncPlayers)
        {
            PacketSendingUtility.ExcludeBroadcastConnected(plr.Index, PlayerPvPPacket.Serialize(packet));
        }
    }

    private void OnPlayerPlayItemSound(PlayerEntity plr, ref PlayerPlayItemSound packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
        {
            return;
        }

        if (HandlersConfiguration.Instance.SyncPlayers)
        {
            PacketSendingUtility.ExcludeBroadcastConnected(plr.Index, PlayerPlayItemSoundPacket.Serialize(packet with { PlayerIndex = (byte)plr.Index }));
        }
    }

    private void OnPlayerMinionRestPoint(PlayerEntity plr, ref PlayerMinionRestPoint packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
        {
            return;
        }

        plr.TPlayer.MinionRestTargetPoint = packet.Position;

        if (HandlersConfiguration.Instance.SyncPlayers)
        {
            PacketSendingUtility.ExcludeBroadcastConnected(plr.Index, PlayerMinionRestPointPacket.Serialize(packet with { PlayerIndex = (byte)plr.Index }));
        }
    }

    private void OnPlayerMinionAttackNPC(PlayerEntity plr, ref PlayerMinionAttackNPC packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
        {
            return;
        }

        plr.TPlayer.MinionAttackTargetNPC = packet.NPCIndex;

        if (HandlersConfiguration.Instance.SyncPlayers)
        {
            PacketSendingUtility.ExcludeBroadcastConnected(plr.Index, PlayerMinionAttackNPCPacket.Serialize(packet with { PlayerIndex = (byte)plr.Index }));
        }
    }

    private void OnPlayerMana(PlayerEntity plr, ref PlayerManaHealEffect packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
        {
            return;
        }

        PlayerEntity victim = EntityTrackers.Players[packet.PlayerIndex];
        if (victim == null || victim.IsGodModeEnabled)
        {
            return;
        }
        if (packet.Count < 0 || packet.Count > 200)
        {
            ignore = true;
            return;
        }

        victim.TPlayer.statMana = Math.Clamp(victim.TPlayer.statMana + packet.Count, 0, victim.TPlayer.statManaMax2);

        if (HandlersConfiguration.Instance.SyncPlayers)
        {
            PacketSendingUtility.ExcludeBroadcastConnected(plr.Index, PlayerManaHealEffectPacket.Serialize(packet));
        }
    }

    private void OnPlayerHeal(PlayerEntity plr, ref PlayerLifeHealEffect packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
        {
            return;
        }

        PlayerEntity victim = EntityTrackers.Players[packet.PlayerIndex];
        if (victim == null || victim.IsGodModeEnabled)
        {
            return;
        }
        if (packet.Count < 0 || packet.Count > 200)
        {
            ignore = true;
            return;
        }

        victim.TPlayer.statLife = Math.Clamp(victim.TPlayer.statLife + packet.Count, 0, victim.TPlayer.statLifeMax2);

        if (HandlersConfiguration.Instance.SyncPlayers)
        {
            PacketSendingUtility.ExcludeBroadcastConnected(plr.Index, PlayerLifeHealEffectPacket.Serialize(packet));
        }
    }

    private void OnPlayerDodge(PlayerEntity plr, ref PlayerDodge packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
        {
            return;
        }

        switch (packet.DodgeType)
        {
            case 1:
                plr.TPlayer.NinjaDodge();
                break;
            case 2:
                plr.TPlayer.ShadowDodge();
                break;
            case 4:
                plr.TPlayer.BrainOfConfusionDodge();
                break;
        }

        if (HandlersConfiguration.Instance.SyncPlayers)
        {
            PacketSendingUtility.ExcludeBroadcastConnected(plr.Index, PlayerDodgePacket.Serialize(packet with { PlayerIndex = (byte)plr.Index }));
        }
    }

    private void OnPlayerAddBuff(PlayerEntity plr, ref PlayerAddBuff packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
        {
            return;
        }

        PlayerEntity victim = EntityTrackers.Players[packet.PlayerIndex];
        if (victim == null)
        {
            return;
        }
        victim.TPlayer.AddBuff(packet.BuffType, packet.BuffTime);

        if (HandlersConfiguration.Instance.SyncPlayers)
        {
            PacketSendingUtility.ExcludeBroadcastConnected(plr.Index, PlayerAddBuffPacket.Serialize(packet with { PlayerIndex = (byte)plr.Index }));
        }
    }

    private void OnPlayerDeath(PlayerEntity plr, ref PlayerDeath packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected || plr.User == null || plr.User.Suspensions?.IsSuspended == true)
        {
            return;
        }

        if (packet.PlayerIndex != plr.Index)
        {
            ignore = true;
            return;
        }

        if (plr.IsGodModeEnabled)
        {
            return;
        }

        plr.IsDead = true;

        if (HandlersConfiguration.Instance.DropTombstones)
        {
            Main.rand = new(DateTime.Now.Second);
            plr.TPlayer.DropTombstone(0, new("", Terraria.Localization.NetworkText.Mode.Literal), packet.HitDirection - 1);
        }

        if (HandlersConfiguration.Instance.AutoRespawnSeconds > 0)
        {
            _ = Task.Run(async () =>
            {
                await Task.Delay(HandlersConfiguration.Instance.AutoRespawnSeconds * 1000);
                if (plr.Phase == ConnectionPhase.Connected && !plr.IsDead)
                {
                    return;
                }

                plr.SendPacketBytes(PlayerSpawnPacket.Serialize(new PlayerSpawn
                {
                    PlayerIndex = (byte)plr.Index,
                    SpawnX = (short)Main.spawnTileX,
                    SpawnY = (short)Main.spawnTileY,
                    RespawnTimer = 0,
                    SpawnContext = 0
                }));
            });
        }

        if (HandlersConfiguration.Instance.SyncPlayers)
        {
            PacketSendingUtility.ExcludeBroadcastConnected(plr.Index, PlayerDeathPacket.Serialize(packet)); // Serialize, because rawPacket can be shitty
        }
    }

    private void OnPlayerHurt(PlayerEntity plr, ref PlayerHurt packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected || plr.User == null || plr.User.Suspensions?.IsSuspended == true)
        {
            return;
        }

        PlayerEntity victim = EntityTrackers.Players[packet.PlayerIndex];
        if (victim == null || victim.IsGodModeEnabled)
        {
            return;
        }

        NetBitsByte flags = packet.Flags;
        bool crit = flags[0];
        bool pvp = flags[1];

        if (pvp)
        {
            if (plr.Index == victim.Index || (victim.Team == plr.Team && victim.Team != 0))
            {
                ignore = true;
                return;
            }

            crit = false;
        }

        Main.rand = new(DateTime.Now.Second);
        victim.TPlayer.Hurt(packet.Reason, packet.Damage, packet.HitDirection - 1, pvp, false, crit, packet.CooldownCounter);

        if (HandlersConfiguration.Instance.SyncPlayers)
        {
            PacketSendingUtility.ExcludeBroadcastConnected(plr.Index, PlayerHurtPacket.Serialize(packet)); // Serialize, because rawPacket can be shitty
        }
    }

    private void OnPlayerSpawn(PlayerEntity plr, ref PlayerSpawn packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected && plr.Phase != ConnectionPhase.WaitingPlayerSpawn)
        {
            return;
        }

        if (packet.PlayerIndex != plr.Index)
        {
            ignore = true;
            return;
        }

        plr.IsDead = false;

        if (!HandlersConfiguration.Instance.SyncPlayers)
        {
            return;
        }

        PlayerSpawn packetToResend = packet with
        {
            SpawnX = packet.SpawnX.IsInWorldX() ? packet.SpawnX : (short)Main.spawnTileX,
            SpawnY = packet.SpawnY.IsInWorldY() ? packet.SpawnY : (short)Main.spawnTileY,
        };

        PacketSendingUtility.ExcludeBroadcastConnected(plr.Index, PlayerSpawnPacket.Serialize(packetToResend)); // Serialize, because rawPacket can be shitty
    }

    private void OnPlayerItemRotation(PlayerEntity plr, ref PlayerItemRotation packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
        {
            return;
        }

        if (packet.PlayerIndex != plr.Index)
        {
            ignore = true;
            return;
        }

        if (HandlersConfiguration.Instance.SyncPlayers)
        {
            PacketSendingUtility.ExcludeBroadcastConnected(plr.Index, PlayerItemRotationPacket.Serialize(packet)); // Serialize, because rawPacket can be shitty
        }
    }

    private void SyncPlayer(in PlayerFullyJoinedArgs args, HookResult<PlayerFullyJoinedArgs> result)
    {
        if (HandlersConfiguration.Instance.SyncPlayers)
        {
            args.Player.SyncFromAll();
            args.Player.SyncToAll();
        }
    }

    private void OnPlayerUpdate(PlayerEntity plr, ref PlayerUpdate packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
        {
            return;
        }

        if (packet.PlayerIndex != plr.Index)
        {
            ignore = true;
            return;
        }

        plr.Position = packet.Position;
        plr.Velocity = packet.Velocity ?? new(0, 0);

        plr.PlayerUpdateInfo = packet;

        if (HandlersConfiguration.Instance.SyncPlayers)
        {
            PacketSendingUtility.ExcludeBroadcastConnected(plr.Index, PlayerUpdatePacket.Serialize(packet)); // Serialize, because rawPacket can be shitty
        }
    }

    public void Unload()
    {
        HookRegistry.GetHook<PlayerFullyJoinedArgs>()
            ?.Unregister(SyncPlayer);

        NetworkManager.SetMainHandler<PlayerSpawn>(null);
        NetworkManager.SetMainHandler<PlayerHurt>(null);
        NetworkManager.SetMainHandler<PlayerDeath>(null);
        NetworkManager.SetMainHandler<PlayerAddBuff>(null);
        NetworkManager.SetMainHandler<PlayerDodge>(null);
        NetworkManager.SetMainHandler<PlayerLifeHealEffect>(null);
        NetworkManager.SetMainHandler<PlayerManaHealEffect>(null);
        NetworkManager.SetMainHandler<PlayerMinionAttackNPC>(null);
        NetworkManager.SetMainHandler<PlayerMinionRestPoint>(null);
        NetworkManager.SetMainHandler<PlayerPlayItemSound>(null);
        NetworkManager.SetMainHandler<PlayerPvP>(null);
        NetworkManager.SetMainHandler<PlayerSetTeam>(null);
        NetworkManager.SetMainHandler<PlayerStealth>(null);
        NetworkManager.SetMainHandler<PlayerSyncBuffs>(null);
        NetworkManager.SetMainHandler<PlayerTalkNPC>(null);
        NetworkManager.SetMainHandler<PlayerZone>(null);
        NetworkManager.SetMainHandler<PlayerItemRotation>(null);
        NetworkManager.SetMainHandler<PlayerUpdate>(null);
    }
}
