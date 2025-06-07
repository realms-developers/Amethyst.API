
using Amethyst.Network.Structures;
using Amethyst.Server.Entities.Players;
using Amethyst.Systems.Characters.Base;
using Amethyst.Systems.Characters.Base.Interactions;
using Amethyst.Systems.Users.Players;
using Terraria;
using Amethyst.Network.Packets;
using Amethyst.Systems.Characters.Base.Enums;

namespace Amethyst.Systems.Characters.Clientside.Interactions;

public sealed class ClientsideCharacterHandler : ICharacterHandler
{
    public ClientsideCharacterHandler(ICharacterProvider provider)
    {
        Provider = provider;

        if (provider.User is not PlayerUser)
            throw new InvalidOperationException("Provider user is not a PlayerUser.");
    }

    public ICharacterProvider Provider { get; }

    public bool InReadonlyMode
    {
        get => false;
        set
        {
            if (value)
                throw new InvalidOperationException("Readonly mode is not supported on the clientside.");
        }
    }

    public void HandlePlayerInfo(PlayerInfo packet)
    {
        var edit = Provider.Editor;

        edit.SetHides(null, packet.AccessoryVisiblity, packet.MiscVisiblity);
        edit.SetSkin(null, packet.HairID, packet.HairDyeID, packet.SkinVariant);
        edit.SetColor(null, PlayerColorType.HairColor, packet.HairColor);
        edit.SetColor(null, PlayerColorType.SkinColor, packet.SkinColor);
        edit.SetColor(null, PlayerColorType.ShirtColor, packet.ShirtColor);
        edit.SetColor(null, PlayerColorType.UnderShirtColor, packet.UnderShirtColor);
        edit.SetColor(null, PlayerColorType.PantsColor, packet.PantsColor);
        edit.SetColor(null, PlayerColorType.ShoesColor, packet.ShoeColor);
        edit.SetColor(null, PlayerColorType.EyesColor, packet.EyeColor);
        edit.SetStats(SyncType.Exclude, (PlayerInfo1)packet.Flags, (PlayerInfo2)packet.Flags2, (PlayerInfo3)packet.Flags3);
    }

    public void HandleQuests(PlayerTownNPCQuestsStats packet)
        => Provider.Editor.SetQuests(SyncType.Exclude, packet.AnglerQuests);

    public void HandleSetLife(PlayerLife packet)
        => Provider.Editor.SetLife(SyncType.Exclude, packet.LifeCount, packet.LifeMax);

    public void HandleSetMana(PlayerMana packet)
        => Provider.Editor.SetMana(SyncType.Exclude, packet.ManaCount, packet.ManaMax);

    public void HandleSlot(PlayerSlot packet)
    {
        var edit = Provider.Editor;

        if (packet.SlotIndex < 0 || packet.SlotIndex >= Provider.CurrentModel.Slots.Length)
        {
            return;
        }

        NetItem item = new NetItem
        {
            ID = packet.ItemID,
            Stack = packet.ItemStack,
            Prefix = packet.ItemPrefix
        };

        edit.SetSlot(SyncType.Exclude, packet.SlotIndex, item);
    }
}
