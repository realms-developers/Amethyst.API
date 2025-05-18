using Amethyst.Gameplay.Players.SSC.Interfaces;
using Amethyst.Network;
using Amethyst.Network.Packets;
using Amethyst.Systems.Characters.Base.Enums;
using Terraria;

namespace Amethyst.Gameplay.Players.SSC;

public sealed class ServerCharacterWrapper(NetPlayer player, CharacterModel model) : ICharacterWrapper
{
    private CharacterModel _model = model;
    private readonly NetPlayer _owner = player;

    public NetItem this[int slot] => slot >= 0 && slot < _model.Slots.Length ? _model.Slots[slot] : default;

    public NetColor this[PlayerColorType type] => _model.Colors[(int)type];

    public CharacterModel Model => _model;

    public bool SupportsChange => true;

    public bool IsReadonly { get; set; }

    public int Life => _owner.TPlayer.statLife;

    public int LifeMax => _model.MaxLife;

    public int Mana => _owner.TPlayer.statMana;

    public int ManaMax => _model.MaxMana;

    public int QuestsCompleted => _model.QuestsCompleted;

    public PlayerInfo1 Stats1 => _model.Info1;

    public PlayerInfo2 Stats2 => _model.Info2;

    public PlayerInfo3 Stats3 => _model.Info3;

    public bool[] HideAccessories => _model.HideAccessories;

    public byte HideMisc => _model.HideMisc;

    public int HairID => _model.Hair;

    public byte HairColor => _model.HairDye;

    public byte SkinVariant => _model.SkinVariant;

    public void LoadCharacter(CharacterModel model, bool sync)
    {
        _model = model;

        if (sync)
        {
            SyncCharacter();
        }
    }

    public void SyncCharacter()
    {
        NetMessage.SendData(7, _owner.Index);

        for (int i = 0; i < _model.Slots.Length; i++)
        {
            SyncSlot(SyncType.Broadcast, i);
        }

        SyncPlayerInfo(SyncType.Broadcast);
        SyncAngler(SyncType.Broadcast);
        SyncLife(SyncType.Broadcast);
        SyncMana(SyncType.Broadcast);
    }

    public void ReceiveSlot(IncomingPacket packet)
    {
        BinaryReader reader = packet.GetReader();
        reader.ReadByte();

        short slot = reader.ReadInt16();

        if (IsReadonly)
        {
            SyncSlot(SyncType.Local, slot);
            return;
        }

        short stack = reader.ReadInt16();
        byte prefix = reader.ReadByte();
        short id = reader.ReadInt16();

        SetSlot(SyncType.Exclude, slot, new NetItem(id, stack, prefix));
    }

    public void ReceiveSetLife(IncomingPacket packet)
    {
        if (IsReadonly)
        {
            SyncLife(SyncType.Local);
            return;
        }

        BinaryReader reader = packet.GetReader();
        reader.ReadByte();

        short current = reader.ReadInt16();
        short max = reader.ReadInt16();

        SetLife(SyncType.Exclude, current, max);
    }

    public void ReceiveSetMana(IncomingPacket packet)
    {
        if (IsReadonly)
        {
            SyncMana(SyncType.Local);
            return;
        }

        BinaryReader reader = packet.GetReader();
        reader.ReadByte();

        short current = reader.ReadInt16();
        short max = reader.ReadInt16();

        SetMana(SyncType.Exclude, current, max);
    }

    public void ReceivePlayerInfo(IncomingPacket packet)
    {
        if (IsReadonly)
        {
            SyncPlayerInfo(SyncType.Local);
            return;
        }

        BinaryReader reader = packet.GetReader();
        reader.ReadByte();

        byte skinVariant = reader.ReadByte();
        byte hair = reader.ReadByte();
        //string name = reader.ReadString(); // UNUSED?
        byte hairDye = reader.ReadByte();
        MessageBuffer.ReadAccessoryVisibility(reader, _owner.TPlayer.hideVisibleAccessory);
        //var accessoryVisiblity = reader.ReadUInt16();
        byte hideMisc = reader.ReadByte();
        NetColor hairColor = reader.ReadNetColor();
        NetColor skinColor = reader.ReadNetColor();
        NetColor eyeColor = reader.ReadNetColor();
        NetColor shirtColor = reader.ReadNetColor();
        NetColor underShirtColor = reader.ReadNetColor();
        NetColor pantsColor = reader.ReadNetColor();
        NetColor shoesColor = reader.ReadNetColor();
        PlayerInfo1 info1 = reader.Read<PlayerInfo1>();
        PlayerInfo2 info2 = reader.Read<PlayerInfo2>();
        PlayerInfo3 info3 = reader.Read<PlayerInfo3>();

        SetSkin(null, hair, hairDye, skinVariant);
        SetHides(null, _owner.TPlayer.hideVisibleAccessory, hideMisc);

        SetColor(null, PlayerColorType.HairColor, hairColor);
        SetColor(null, PlayerColorType.SkinColor, skinColor);
        SetColor(null, PlayerColorType.EyesColor, eyeColor);
        SetColor(null, PlayerColorType.ShirtColor, shirtColor);
        SetColor(null, PlayerColorType.UndershirtColor, underShirtColor);
        SetColor(null, PlayerColorType.PantsColor, pantsColor);
        SetColor(null, PlayerColorType.ShoesColor, shoesColor);

        SetStats(SyncType.Exclude, info1, info2, info3);
    }

    public void ReceiveQuests(IncomingPacket packet)
    {
        if (IsReadonly)
        {
            SyncAngler(SyncType.Local);
            return;
        }

        SetQuests(SyncType.Exclude, _model.QuestsCompleted + 1);
    }


    public bool SetSlot(SyncType? sync, int slot, NetItem item)
    {
        if (slot >= 59 && slot <= 88)
        {
            int fixedSlot = 260 + 30 * _owner.TPlayer.CurrentLoadoutIndex + (slot - 59);
            _model.Slots[fixedSlot] = item;

            if (sync != null)
            {
                SyncSlot(sync.Value, fixedSlot);
            }
        }

        _model.Slots[slot] = item;

        _needsToSave = true;

        if (sync != null)
        {
            SyncSlot(sync.Value, slot);
        }

        return true;
    }

    public bool SetLife(SyncType? sync, int? current, int? max)
    {
        _owner.TPlayer.statLife = current ?? _owner.TPlayer.statLife;
        _model.MaxLife = max ?? _model.MaxLife;

        _needsToSave = true;

        if (sync != null)
        {
            SyncLife(sync.Value);
        }

        return true;
    }

    public bool SetMana(SyncType? sync, int? current, int? max)
    {
        _owner.TPlayer.statMana = current ?? _owner.TPlayer.statMana;
        _model.MaxMana = max ?? _model.MaxMana;

        _needsToSave = true;

        if (sync != null)
        {
            SyncMana(sync.Value);
        }

        return true;
    }

    public bool SetColor(SyncType? sync, PlayerColorType colorType, NetColor color)
    {
        _model.Colors[(byte)colorType] = color;

        _owner.TPlayer.hairColor = this[PlayerColorType.HairColor].ToXNA();
        _owner.TPlayer.skinColor = this[PlayerColorType.SkinColor].ToXNA();
        _owner.TPlayer.eyeColor = this[PlayerColorType.EyesColor].ToXNA();
        _owner.TPlayer.shirtColor = this[PlayerColorType.ShirtColor].ToXNA();
        _owner.TPlayer.underShirtColor = this[PlayerColorType.UndershirtColor].ToXNA();
        _owner.TPlayer.pantsColor = this[PlayerColorType.PantsColor].ToXNA();
        _owner.TPlayer.shoeColor = this[PlayerColorType.ShoesColor].ToXNA();

        _needsToSave = true;
        // man pohui nahui

        DirectPlayerInfoSync(sync);

        return true;
    }

    public bool SetQuests(SyncType? sync, int completed) // 76
    {
        _model.QuestsCompleted = completed;

        _needsToSave = true;

        if (sync != null)
        {
            SyncAngler(sync.Value);
        }

        return true;
    }

    public bool SetStats(SyncType? sync, PlayerInfo1? stats1 = null, PlayerInfo2? stats2 = null, PlayerInfo3? stats3 = null)
    {
        if (stats1 != null)
        {
            _model.Info1 = stats1.Value;
            _owner.TPlayer.extraAccessory = _model.Info1.HasFlag(PlayerInfo1.ExtraAccessory);
        }

        if (stats2 != null)
        {
            _model.Info2 = stats2.Value;
            _owner.TPlayer.UsingBiomeTorches = _model.Info2.HasFlag(PlayerInfo2.UsingBiomeTorches);
            _owner.TPlayer.happyFunTorchTime = _model.Info2.HasFlag(PlayerInfo2.HappyTorchTime);
            _owner.TPlayer.unlockedBiomeTorches = _model.Info2.HasFlag(PlayerInfo2.UnlockedBiomeTorches);
            _owner.TPlayer.unlockedSuperCart = _model.Info2.HasFlag(PlayerInfo2.UnlockedSuperCart);
            _owner.TPlayer.enabledSuperCart = _model.Info2.HasFlag(PlayerInfo2.EnabledSuperCart);
        }

        if (stats3 != null)
        {
            _model.Info3 = stats3.Value;
            _owner.TPlayer.usedAegisCrystal = _model.Info3.HasFlag(PlayerInfo3.UsedAegisCrystal);
            _owner.TPlayer.usedAegisFruit = _model.Info3.HasFlag(PlayerInfo3.UsedAegisFruit);
            _owner.TPlayer.usedArcaneCrystal = _model.Info3.HasFlag(PlayerInfo3.UsedArcaneCrystal);
            _owner.TPlayer.usedGalaxyPearl = _model.Info3.HasFlag(PlayerInfo3.UsedGalaxyPearl);
            _owner.TPlayer.usedGummyWorm = _model.Info3.HasFlag(PlayerInfo3.UsedGummyWorm);
            _owner.TPlayer.usedAmbrosia = _model.Info3.HasFlag(PlayerInfo3.UsedAmbrosia);
            _owner.TPlayer.ateArtisanBread = _model.Info3.HasFlag(PlayerInfo3.AteArtisanBread);
        }

        _needsToSave = true;

        DirectPlayerInfoSync(sync);

        return true;
    }

    public bool SetHides(SyncType? sync, bool[]? hideAccessories = null, byte? hideMisc = null)
    {
        _model.HideMisc = hideMisc ?? _model.HideMisc;
        _model.HideAccessories = hideAccessories ?? _model.HideAccessories;

        _owner.TPlayer.hideMisc = _model.HideMisc;
        _owner.TPlayer.hideVisibleAccessory = _model.HideAccessories ?? new bool[10];

        _needsToSave = true;

        DirectPlayerInfoSync(sync);

        return true;
    }

    public bool SetSkin(SyncType? sync, byte? hairId = null, byte? hairColor = null, byte? skinVariant = null)
    {
        _model.Hair = hairId ?? _model.Hair;
        _model.HairDye = hairColor ?? _model.HairDye;
        _model.SkinVariant = skinVariant ?? _model.SkinVariant;

        _owner.TPlayer.skinVariant = _model.SkinVariant;
        _owner.TPlayer.hair = _model.Hair;
        _owner.TPlayer.hairDye = _model.HairDye;

        _needsToSave = true;

        DirectPlayerInfoSync(sync);

        return true;
    }

    public void SyncSlot(SyncType sync, int slot)
    {
        if (slot < 0 || slot >= _model.Slots.Length)
        {
            return;
        }

        NetItem item = this[slot];

        if (slot >= 59 && slot <= 88)
        {
            int fixedSlot = 260 + 30 * _owner.TPlayer.CurrentLoadoutIndex + (slot - 59);
            item = this[fixedSlot];
            PlayerUtilities.TerrarifySlot(_owner, item, fixedSlot);
        }

        PlayerUtilities.TerrarifySlot(_owner, item, slot);

        byte[] packet = new PacketWriter().SetType(5)
            .PackByte((byte)_owner.Index)
            .PackInt16((short)slot) // index
            .PackInt16(item.Stack)
            .PackByte(item.Prefix)
            .PackInt16((short)item.ID).BuildPacket();

        int remote = sync == SyncType.Local ? _owner.Index : -1;
        int ignore = sync == SyncType.Exclude ? _owner.Index : -1;

        switch (sync)
        {
            case SyncType.Local:
                _owner.Socket.SendPacket(packet);
                break;

            case SyncType.Exclude:
                PlayerUtilities.BroadcastPacket(packet, p => p.Index != _owner.Index);
                break;

            case SyncType.Broadcast:
                PlayerUtilities.BroadcastPacket(packet);
                break;
        }
    }

    private void DirectPlayerInfoSync(SyncType? sync)
    {
        if (sync == SyncType.Broadcast)
        {
            NetMessage.TrySendData(4, -1, -1, Terraria.Localization.NetworkText.Empty, _owner.Index);
        }
        else if (sync == SyncType.Local || sync == SyncType.Exclude)
        {
            NetMessage.TrySendData(4, sync == SyncType.Local ? _owner.Index : -1, sync == SyncType.Exclude ? _owner.Index : -1, Terraria.Localization.NetworkText.Empty, _owner.Index);
        }
    }

    public void SyncLife(SyncType sync)
    {
        if (IsReadonly)
        {
            _owner.TPlayer.statLife = _model.MaxLife;
        }

        _owner.TPlayer.statLifeMax = _model.MaxLife;
        _owner.TPlayer.statLifeMax2 = _model.MaxLife;

        NetMessage.TrySendData(16, sync == SyncType.Local ? _owner.Index : -1, sync == SyncType.Exclude ? _owner.Index : -1, Terraria.Localization.NetworkText.Empty, _owner.Index);
    }

    public void SyncMana(SyncType sync)
    {
        if (IsReadonly)
        {
            _owner.TPlayer.statMana = _model.MaxMana;
        }

        _owner.TPlayer.statManaMax = _model.MaxMana;
        _owner.TPlayer.statManaMax2 = _model.MaxMana;

        NetMessage.TrySendData(42, sync == SyncType.Local ? _owner.Index : -1, sync == SyncType.Exclude ? _owner.Index : -1, Terraria.Localization.NetworkText.Empty, _owner.Index);
    }

    public void SyncPlayerInfo(SyncType sync)
    {
        _owner.TPlayer.skinVariant = _model.SkinVariant;
        _owner.TPlayer.hair = _model.Hair;
        _owner.TPlayer.hairDye = _model.HairDye;
        _owner.TPlayer.hideMisc = _model.HideMisc;
        _owner.TPlayer.hideVisibleAccessory = _model.HideAccessories ?? new bool[10];
        _owner.TPlayer.hairColor = this[PlayerColorType.HairColor].ToXNA();
        _owner.TPlayer.skinColor = this[PlayerColorType.SkinColor].ToXNA();
        _owner.TPlayer.eyeColor = this[PlayerColorType.EyesColor].ToXNA();
        _owner.TPlayer.shirtColor = this[PlayerColorType.ShirtColor].ToXNA();
        _owner.TPlayer.underShirtColor = this[PlayerColorType.UndershirtColor].ToXNA();
        _owner.TPlayer.pantsColor = this[PlayerColorType.PantsColor].ToXNA();
        _owner.TPlayer.shoeColor = this[PlayerColorType.ShoesColor].ToXNA();

        _owner.TPlayer.difficulty = 0;
        _owner.TPlayer.extraAccessory = _model.Info1.HasFlag(PlayerInfo1.ExtraAccessory);

        _owner.TPlayer.UsingBiomeTorches = _model.Info2.HasFlag(PlayerInfo2.UsingBiomeTorches);
        _owner.TPlayer.happyFunTorchTime = _model.Info2.HasFlag(PlayerInfo2.HappyTorchTime);
        _owner.TPlayer.unlockedBiomeTorches = _model.Info2.HasFlag(PlayerInfo2.UnlockedBiomeTorches);
        _owner.TPlayer.unlockedSuperCart = _model.Info2.HasFlag(PlayerInfo2.UnlockedSuperCart);
        _owner.TPlayer.enabledSuperCart = _model.Info2.HasFlag(PlayerInfo2.EnabledSuperCart);

        _owner.TPlayer.usedAegisCrystal = _model.Info3.HasFlag(PlayerInfo3.UsedAegisCrystal);
        _owner.TPlayer.usedAegisFruit = _model.Info3.HasFlag(PlayerInfo3.UsedAegisFruit);
        _owner.TPlayer.usedArcaneCrystal = _model.Info3.HasFlag(PlayerInfo3.UsedArcaneCrystal);
        _owner.TPlayer.usedGalaxyPearl = _model.Info3.HasFlag(PlayerInfo3.UsedGalaxyPearl);
        _owner.TPlayer.usedGummyWorm = _model.Info3.HasFlag(PlayerInfo3.UsedGummyWorm);
        _owner.TPlayer.usedAmbrosia = _model.Info3.HasFlag(PlayerInfo3.UsedAmbrosia);
        _owner.TPlayer.ateArtisanBread = _model.Info3.HasFlag(PlayerInfo3.AteArtisanBread);

        NetMessage.TrySendData(4, sync == SyncType.Local ? _owner.Index : -1, sync == SyncType.Exclude ? _owner.Index : -1, Terraria.Localization.NetworkText.Empty, _owner.Index);
    }

    public void SyncAngler(SyncType sync)
    {
        _owner.TPlayer.anglerQuestsFinished = _model.QuestsCompleted;

        NetMessage.TrySendData(76, sync == SyncType.Local ? _owner.Index : -1, sync == SyncType.Exclude ? _owner.Index : -1, Terraria.Localization.NetworkText.Empty, _owner.Index);
    }

    public void Save()
    {
        Model.Save();
    }

    private bool _needsToSave;
    public void SaveUpdate()
    {
        if (_needsToSave)
        {
            Model.Save();

            _needsToSave = false;
        }
    }
}
