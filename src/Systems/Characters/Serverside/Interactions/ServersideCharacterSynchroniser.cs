using Amethyst.Network.Structures;
using Amethyst.Server.Entities.Players;
using Amethyst.Systems.Characters.Base;
using Amethyst.Systems.Characters.Base.Enums;
using Amethyst.Systems.Characters.Base.Interactions;
using Amethyst.Systems.Characters.Utilities;
using Amethyst.Systems.Users.Players;
using Terraria;
using Amethyst.Network.Engine.Packets;
using Amethyst.Network.Packets;

namespace Amethyst.Systems.Characters.Serverside.Interactions;

public sealed class ServersideCharacterSynchroniser : ICharacterSynchroniser
{
    public ServersideCharacterSynchroniser(ICharacterProvider provider)
    {
        Provider = provider;

        if (provider.User is not PlayerUser)
            throw new InvalidOperationException("Provider user is not a PlayerUser.");

        PlayerUser user = (PlayerUser)provider.User;

        Player = user.Player;
        TPlayer = user.Player.TPlayer;
    }

    public ICharacterProvider Provider { get; }

    public PlayerEntity Player { get; }
    public Player TPlayer { get; }

    public void SyncSlot(SyncType sync, int slot)
    {
        var model = Provider.CurrentModel;

        if (slot < 0 || slot >= model.Slots.Length)
        {
            return;
        }

        NetItem item = model.Slots[slot];

        if (slot >= 59 && slot <= 88)
        {
            int fixedSlot = 260 + 30 * TPlayer.CurrentLoadoutIndex + (slot - 59);
            item = model.Slots[fixedSlot];
            CharacterUtilities.TerrarifySlot(Player, item, fixedSlot);
        }

        CharacterUtilities.TerrarifySlot(Player, item, slot);

        byte[] packet = PlayerSlotPacket.Serialize(new PlayerSlot
        {
            PlayerIndex = (byte)Player.Index,
            SlotIndex = (short)slot,
            ItemID = (short)item.ID,
            ItemStack = item.Stack,
            ItemPrefix = item.Prefix
        });
        SendPacket(sync, packet);
    }

    public void SyncLife(SyncType sync)
    {
        var model = Provider.CurrentModel;

        Player.TPlayer.statLifeMax = model.MaxLife;
        Player.TPlayer.statLifeMax2 = model.MaxLife;

        byte[] packet = PlayerLifePacket.Serialize(new PlayerLife
        {
            PlayerIndex = (byte)Player.Index,
            LifeCount = (short)Player.TPlayer.statLife,
            LifeMax = (short)model.MaxLife
        });
        SendPacket(sync, packet);
    }

    public void SyncMana(SyncType sync)
    {
        var model = Provider.CurrentModel;

        Player.TPlayer.statManaMax = model.MaxMana;
        Player.TPlayer.statManaMax2 = model.MaxMana;

        byte[] packet = PlayerManaPacket.Serialize(new PlayerMana
        {
            PlayerIndex = (byte)Player.Index,
            ManaCount = (short)Player.TPlayer.statMana,
            ManaMax = (short)model.MaxMana
        });
        SendPacket(sync, packet);
    }

    public void SyncPlayerInfo(SyncType sync)
    {
        var model = Provider.CurrentModel;

        Player.TPlayer.skinVariant = model.SkinVariant;
        Player.TPlayer.hair = model.Hair;
        Player.TPlayer.hairDye = model.HairDye;
        Player.TPlayer.hideMisc = model.HideMisc;
        Player.TPlayer.hideVisibleAccessory = model.HideAccessories ?? new bool[10];
        Player.TPlayer.hairColor = model.Colors[(byte)PlayerColorType.HairColor];
        Player.TPlayer.skinColor = model.Colors[(byte)PlayerColorType.SkinColor];
        Player.TPlayer.eyeColor = model.Colors[(byte)PlayerColorType.EyesColor];
        Player.TPlayer.shirtColor = model.Colors[(byte)PlayerColorType.ShirtColor];
        Player.TPlayer.underShirtColor = model.Colors[(byte)PlayerColorType.UnderShirtColor];
        Player.TPlayer.pantsColor = model.Colors[(byte)PlayerColorType.PantsColor];
        Player.TPlayer.shoeColor = model.Colors[(byte)PlayerColorType.ShoesColor];

        Player.TPlayer.difficulty = 0;
        Player.TPlayer.extraAccessory = model.Info1.HasFlag(PlayerInfo1.ExtraAccessory);

        Player.TPlayer.UsingBiomeTorches = model.Info2.HasFlag(PlayerInfo2.UsingBiomeTorches);
        Player.TPlayer.happyFunTorchTime = model.Info2.HasFlag(PlayerInfo2.HappyTorchTime);
        Player.TPlayer.unlockedBiomeTorches = model.Info2.HasFlag(PlayerInfo2.UnlockedBiomeTorches);
        Player.TPlayer.unlockedSuperCart = model.Info2.HasFlag(PlayerInfo2.UnlockedSuperCart);
        Player.TPlayer.enabledSuperCart = model.Info2.HasFlag(PlayerInfo2.EnabledSuperCart);

        Player.TPlayer.usedAegisCrystal = model.Info3.HasFlag(PlayerInfo3.UsedAegisCrystal);
        Player.TPlayer.usedAegisFruit = model.Info3.HasFlag(PlayerInfo3.UsedAegisFruit);
        Player.TPlayer.usedArcaneCrystal = model.Info3.HasFlag(PlayerInfo3.UsedArcaneCrystal);
        Player.TPlayer.usedGalaxyPearl = model.Info3.HasFlag(PlayerInfo3.UsedGalaxyPearl);
        Player.TPlayer.usedGummyWorm = model.Info3.HasFlag(PlayerInfo3.UsedGummyWorm);
        Player.TPlayer.usedAmbrosia = model.Info3.HasFlag(PlayerInfo3.UsedAmbrosia);
        Player.TPlayer.ateArtisanBread = model.Info3.HasFlag(PlayerInfo3.AteArtisanBread);

        byte[] packet = PlayerInfoPacket.Serialize(new PlayerInfo
        {
            PlayerIndex = (byte)Player.Index,
            HairID = model.Hair,
            HairDyeID = model.HairDye,
            SkinVariant = model.SkinVariant,
            HairColor = model.Colors[(byte)PlayerColorType.HairColor],
            SkinColor = model.Colors[(byte)PlayerColorType.SkinColor],
            ShirtColor = model.Colors[(byte)PlayerColorType.ShirtColor],
            UnderShirtColor = model.Colors[(byte)PlayerColorType.UnderShirtColor],
            PantsColor = model.Colors[(byte)PlayerColorType.PantsColor],
            ShoeColor = model.Colors[(byte)PlayerColorType.ShoesColor],
            AccessoryVisiblity = model.HideAccessories ?? new bool[10],
            MiscVisiblity = model.HideMisc,
            Flags = (byte)model.Info1,
            Flags2 = (byte)model.Info2,
            Flags3 = (byte)model.Info3
        });
        SendPacket(sync, packet);
    }

    public void SyncQuests(SyncType sync)
    {
        Player.TPlayer.anglerQuestsFinished = Provider.CurrentModel.QuestsCompleted;

        byte[] packet = PlayerTownNPCQuestsStatsPacket.Serialize(new PlayerTownNPCQuestsStats
        {
            PlayerIndex = (byte)Player.Index,
            AnglerQuests = Player.TPlayer.anglerQuestsFinished
        });
        SendPacket(sync, packet);
    }

    private void SendPacket(SyncType syncType, byte[] packet)
    {
        switch (syncType)
        {
            case SyncType.Local:
                Player.SendPacketBytes(packet);
                break;

            case SyncType.Exclude:
                PlayerUtils.BroadcastPacketBytes(packet, Player.Index);
                break;

            case SyncType.Broadcast:
                PlayerUtils.BroadcastPacketBytes(packet);
                break;
        }
    }
}
