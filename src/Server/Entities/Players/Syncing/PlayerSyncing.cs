using Amethyst.Network.Packets;
using Amethyst.Network.Structures;
using Amethyst.Systems.Characters.Base;
using Amethyst.Systems.Characters.Enums;

namespace Amethyst.Server.Entities.Players.Syncing;

public static class PlayerSyncing
{
    public static List<Func<PlayerEntity, byte[]?>> SyncPacketCreators { get; }
        = [SyncActive, SyncPlayerInfo, SyncUpdateInfo, SyncPvP, SyncTeam];
    // TODO: add life, mana, loadout, buffs, and other packets

    public static Func<PlayerEntity, short, NetItem, byte[]?>? SyncSlotPacket { get; set; } = SyncSlot;

    public static IEnumerable<byte[]> CreateSyncPackets(PlayerEntity player)
    {
        foreach (Func<PlayerEntity, byte[]?> creator in SyncPacketCreators)
        {
            byte[]? packet = creator(player);
            if (packet != null)
            {
                yield return packet;
            }
        }

        if (player.User?.Character != null && SyncSlotPacket != null)
        {
            for (short i = 0; i < player.User.Character.CurrentModel.Slots.Length; i++)
            {
                byte[]? packet = SyncSlotPacket(player, i, player.User.Character.CurrentModel.Slots[i]);
                if (packet != null)
                {
                    yield return packet;
                }
            }
        }
    }

    public static byte[]? SyncPvP(PlayerEntity player)
    {
        if (!player.IsInPvP)
        {
            return null;
        }

        return PlayerPvPPacket.Serialize(new PlayerPvP
        {
            PlayerIndex = (byte)player.Index,
            IsInPvP = player.IsInPvP
        });
    }

    public static byte[]? SyncTeam(PlayerEntity player)
    {
        if (player.Team == 0)
        {
            return null;
        }

        return PlayerSetTeamPacket.Serialize(new PlayerSetTeam
        {
            PlayerIndex = (byte)player.Index,
            TeamIndex = (byte)player.Team
        });
    }

    public static byte[]? SyncActive(PlayerEntity player)
    {
        return PlayerActivePacket.Serialize(new PlayerActive
        {
            PlayerIndex = (byte)player.Index,
            State = true
        });
    }

    public static byte[]? SyncUpdateInfo(PlayerEntity player)
    {
        if (player.PlayerUpdateInfo == null)
        {
            return null;
        }

        return PlayerUpdatePacket.Serialize(player.PlayerUpdateInfo.Value);
    }

    public static byte[]? SyncPlayerInfo(PlayerEntity player)
    {
        if (player.User == null || player.User.Character == null)
        {
            return null;
        }

        ICharacterModel character = player.User.Character.CurrentModel;

        PlayerInfo info = new()
        {
            PlayerIndex = (byte)player.Index,
            SkinVariant = character.SkinVariant,
            HairID = character.Hair,
            Name = character.Name,
            HairDyeID = character.HairDye,
            AccessoryVisiblity = character.HideAccessories,
            MiscVisiblity = character.HideMisc,
            HairColor = character.Colors[(byte)PlayerColorType.HairColor],
            SkinColor = character.Colors[(byte)PlayerColorType.SkinColor],
            EyeColor = character.Colors[(byte)PlayerColorType.EyesColor],
            ShirtColor = character.Colors[(byte)PlayerColorType.ShirtColor],
            UnderShirtColor = character.Colors[(byte)PlayerColorType.UnderShirtColor],
            PantsColor = character.Colors[(byte)PlayerColorType.PantsColor],
            ShoeColor = character.Colors[(byte)PlayerColorType.ShoesColor],
            Flags = (byte)character.Info1,
            Flags2 = (byte)character.Info2,
            Flags3 = (byte)character.Info3
        };

        return PlayerInfoPacket.Serialize(info);
    }

    public static unsafe byte[]? SyncSlot(PlayerEntity player, short index, NetItem item)
    {
        return PlayerSlotPacket.Serialize(new PlayerSlot
        {
            PlayerIndex = (byte)player.Index,
            SlotIndex = index,
            ItemID = item.ID,
            ItemPrefix = item.Prefix,
            ItemStack = item.Stack,
        });
    }
}
