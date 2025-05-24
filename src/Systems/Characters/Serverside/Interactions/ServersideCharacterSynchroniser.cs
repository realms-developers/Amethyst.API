using Amethyst.Gameplay.Players;
using Amethyst.Network;
using Amethyst.Systems.Characters.Base;
using Amethyst.Systems.Characters.Base.Enums;
using Amethyst.Systems.Characters.Base.Interactions;
using Amethyst.Systems.Characters.Utilities;
using Amethyst.Systems.Users.Players;
using Terraria;

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

    public NetPlayer Player { get; }
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

        byte[] packet = new PacketWriter().SetType(5)
            .PackByte((byte)Player.Index)
            .PackInt16((short)slot) // index
            .PackInt16(item.Stack)
            .PackByte(item.Prefix)
            .PackInt16((short)item.ID).BuildPacket();

        int remote = sync == SyncType.Local ? Player.Index : -1;
        int ignore = sync == SyncType.Exclude ? Player.Index : -1;

        switch (sync)
        {
            case SyncType.Local:
                Player.Socket.SendPacket(packet);
                break;

            case SyncType.Exclude:
                PlayerUtilities.BroadcastPacket(packet, p => p.Index != Player.Index);
                break;

            case SyncType.Broadcast:
                PlayerUtilities.BroadcastPacket(packet);
                break;
        }
    }

    public void SyncLife(SyncType sync)
    {
        var model = Provider.CurrentModel;

        Player.TPlayer.statLifeMax = model.MaxLife;
        Player.TPlayer.statLifeMax2 = model.MaxLife;

        NetMessage.TrySendData(16, sync == SyncType.Local ? Player.Index : -1, sync == SyncType.Exclude ? Player.Index : -1, Terraria.Localization.NetworkText.Empty, Player.Index);
    }

    public void SyncMana(SyncType sync)
    {
        var model = Provider.CurrentModel;

        Player.TPlayer.statManaMax = model.MaxMana;
        Player.TPlayer.statManaMax2 = model.MaxMana;

        NetMessage.TrySendData(42, sync == SyncType.Local ? Player.Index : -1, sync == SyncType.Exclude ? Player.Index : -1, Terraria.Localization.NetworkText.Empty, Player.Index);
    }

    public void SyncPlayerInfo(SyncType sync)
    {
        var model = Provider.CurrentModel;

        Player.TPlayer.skinVariant = model.SkinVariant;
        Player.TPlayer.hair = model.Hair;
        Player.TPlayer.hairDye = model.HairDye;
        Player.TPlayer.hideMisc = model.HideMisc;
        Player.TPlayer.hideVisibleAccessory = model.HideAccessories ?? new bool[10];
        Player.TPlayer.hairColor = model.Colors[(byte)PlayerColorType.HairColor].ToXNA();
        Player.TPlayer.skinColor = model.Colors[(byte)PlayerColorType.SkinColor].ToXNA();
        Player.TPlayer.eyeColor = model.Colors[(byte)PlayerColorType.EyesColor].ToXNA();
        Player.TPlayer.shirtColor = model.Colors[(byte)PlayerColorType.ShirtColor].ToXNA();
        Player.TPlayer.underShirtColor = model.Colors[(byte)PlayerColorType.UndershirtColor].ToXNA();
        Player.TPlayer.pantsColor = model.Colors[(byte)PlayerColorType.PantsColor].ToXNA();
        Player.TPlayer.shoeColor = model.Colors[(byte)PlayerColorType.ShoesColor].ToXNA();

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

        NetMessage.TrySendData(4, sync == SyncType.Local ? Player.Index : -1, sync == SyncType.Exclude ? Player.Index : -1, Terraria.Localization.NetworkText.Empty, Player.Index);
    }

    public void SyncQuests(SyncType sync)
    {
        Player.TPlayer.anglerQuestsFinished = Provider.CurrentModel.QuestsCompleted;

        NetMessage.TrySendData(76, sync == SyncType.Local ? Player.Index : -1, sync == SyncType.Exclude ? Player.Index : -1, Terraria.Localization.NetworkText.Empty, Player.Index);
    }
}
