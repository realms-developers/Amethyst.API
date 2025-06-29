using Amethyst.Network.Enums;
using Amethyst.Network.Packets;
using Amethyst.Network.Structures;
using Amethyst.Server.Entities.Base;
using Amethyst.Server.Entities.Items;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;

namespace Amethyst.Server.Entities.Players;

public sealed partial class PlayerEntity : IServerEntity
{
    public void GiveItem(int id, int stack, byte prefix) => ItemUtils.CreateItem(Position.X, Position.Y, Index, id, stack, prefix);

    public void GiveItem(NetItem item) => GiveItem(item.ID, item.Stack, item.Prefix);

    public void GiveItem(Item item) => GiveItem(item.netID, item.stack, item.prefix);

    public void AddBuff(int type, TimeSpan duration)
    {
        int time = (int)duration.TotalSeconds * 60;
        NetMessage.SendData(55, -1, -1, NetworkText.Empty, Index, type, number3: time);
    }

    public void Heal(int amount) => NetMessage.SendData(66, -1, -1, NetworkText.Empty, Index, amount);

    public void Hurt(int damage, string text, bool pvp = false) => Hurt(damage, PlayerDeathReason.ByCustomReason(text), pvp);

    public void Hurt(int damage, PlayerDeathReason? reason = null, bool pvp = false) =>
        NetMessage.SendPlayerHurt(Index, reason ?? PlayerDeathReason.LegacyDefault(), damage, -1, false, pvp, 0);

    public void Kill(string text, bool pvp = false) => Kill(PlayerDeathReason.ByCustomReason(text), pvp);

    public void Kill(PlayerDeathReason? reason = null, bool pvp = false) =>
        NetMessage.SendPlayerDeath(Index, reason ?? PlayerDeathReason.LegacyDefault(), short.MaxValue, -1, pvp);

    public void Teleport(float x, float y)
    {
        RequestSendSection(Netplay.GetSectionX((int)x / 16), Netplay.GetSectionY((int)y / 16));

        PlayerOrEntityTeleport packet = new()
        {
            TargetPosition = new NetVector2(x, y),
            EntityIndex = (byte)Index
        };

        byte[] data = PlayerOrEntityTeleportPacket.Serialize(packet);
        PlayerUtils.BroadcastPacketBytes(data, -1);
    }

    public void TeleportTile(int x, int y) => Teleport(x * 16, y * 16);

    public void Teleport(NetVector2 position) => Teleport(position.X, position.Y);

    public void SetPvP(bool enabled) =>
        NetMessage.SendData((byte)PacketID.PlayerPvP, -1, -1, NetworkText.Empty, Index, enabled ? 1 : 0);

    public void SetTeam(int teamId) =>
        NetMessage.SendData((byte)PacketID.PlayerSetTeam, -1, -1, NetworkText.Empty, Index, teamId);

    public void OpenSign(int signId)
    {
        if (signId < 0 || signId >= Main.sign.Length)
        {
            return;
        }

        Sign sign = Main.sign[signId];
        if (sign == null)
        {
            return;
        }

        NetMessage.SendData((byte)PacketID.SignSync, Index, -1, NetworkText.Empty, Index, signId, Index);
    }

    public void OpenChest(int chestId)
    {
        if (chestId < 0 || chestId >= Main.chest.Length)
        {
            return;
        }

        Chest chest = Main.chest[chestId];
        if (chest == null)
        {
            return;
        }

        for (int num222 = 0; num222 < 40; num222++)
        {
            NetMessage.TrySendData(32, Index, -1, NetworkText.Empty, chestId, num222);
        }
        NetMessage.TrySendData(33, Index, -1, NetworkText.Empty, chestId);
        TPlayer.chest = chestId;
        NetMessage.TrySendData(80, -1, Index, NetworkText.Empty, Index, chestId);
    }

    public void Spawn(short x, short y, int respawnTimer, byte style = 0)
    {
        PlayerSpawn packet = new()
        {
            SpawnX = x,
            SpawnY = y,
            RespawnTimer = respawnTimer,
            SpawnContext = style,
            PlayerIndex = (byte)Index,
            DeathsPvE = (short)TPlayer.numberOfDeathsPVE,
            DeathsPvP = (short)TPlayer.numberOfDeathsPVP,
        };

        byte[] data = PlayerSpawnPacket.Serialize(packet);
        PlayerUtils.BroadcastPacketBytes(data, -1);
    }

    public void RemoveHeldItem() => RemoveItem(HeldItem);

    public void RemoveItem(NetItem item) => RemoveItem(item.ID, item.Stack);

    public void RemoveItem(Item item) => RemoveItem((short)item.netID, (short)item.stack);

    public void RemoveItem(short netId, short stack)
    {
        PlayerConsumeItem packet = new()
        {
            ItemType = netId,
            ItemCount = stack,
            PlayerIndex = (byte)Index
        };

        byte[] data = PlayerConsumeItemPacket.Serialize(packet);
        PlayerUtils.BroadcastPacketBytes(data, -1);
    }

    public bool RemoveProjectileByIndex(short index, bool broadcast)
    {
        if (index < 0 || index >= Main.projectile.Length)
        {
            return false;
        }

        Projectile? proj = Main.projectile[index];
        if (proj == null || !proj.active || proj.owner != Index)
        {
            return false;
        }

        ProjectileUpdate packet = new()
        {
            ProjectileIdentity = (short)proj.identity,
            OwnerID = (byte)Index,
        };

        Main.projectile[index] = new Projectile();

        byte[] data = ProjectileUpdatePacket.Serialize(packet);
        if (broadcast)
        {
            PlayerUtils.BroadcastPacketBytes(data, -1);
        }
        else
        {
            SendPacketBytes(data);
        }

        return true;
    }

    public int RemoveProjectileByIdentity(short identity, bool broadcast = true)
    {
        int? projIndex = Main.projectile.FirstOrDefault(p => p != null && p.active && p.identity == identity && p.owner == Index)?.whoAmI;
        if (projIndex == null)
        {
            return -1;
        }

        Main.projectile[projIndex.Value] = new Projectile();

        ProjectileUpdate packet = new()
        {
            ProjectileIdentity = identity,
            OwnerID = (byte)Index,
        };

        byte[] data = ProjectileUpdatePacket.Serialize(packet);
        if (broadcast)
        {
            PlayerUtils.BroadcastPacketBytes(data, -1);
        }
        else
        {
            SendPacketBytes(data);
        }

        return projIndex.Value;
    }

    public void SendStatusText(string text, bool padding = false)
    {
        if (padding)
        {
            text = new string('\n', 10) + text;
        }

        SendStatusText(new NetText() { Mode = 0, Text = text });
    }

    public void SendStatusText(NetText text)
    {
        ServerStatus status = new ServerStatus()
        {
            StatusText = text,
            StatusFlags = 0,
        };

        SendPacketBytes(ServerStatusPacket.Serialize(status));
    }

    public void CreateCombatText(string text, NetColor color, NetVector2? position = null, bool broadcast = true)
        => CreateCombatText(new NetText() { Mode = 0, Text = text }, color, position, broadcast);

    public void CreateCombatText(NetText text, NetColor color, NetVector2? position = null, bool broadcast = true)
    {
        position ??= Position with { Y = Position.Y - 32f };

        VisualCreateCombatTextText packet = new()
        {
            Text = text,
            NetColor = color,
            Position = position.Value,
        };

        byte[] data = VisualCreateCombatTextTextPacket.Serialize(packet);
        if (broadcast)
        {
            PlayerUtils.BroadcastPacketBytes(data, -1);
        }
        else
        {
            SendPacketBytes(data);
        }
    }
}
