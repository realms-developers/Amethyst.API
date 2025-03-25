using Amethyst.Network;
using Amethyst.Network.Packets;
using Amethyst.Players.SSC.Enums;

namespace Amethyst.Players.SSC.Interfaces;

public interface ICharacterWrapper
{
    /// <summary>
    /// Direct CharacterModel instance. Can be changed and used in LoadCharacter(..);
    /// </summary>
    public CharacterModel Model { get; }

    /// <summary>
    /// Indicates, that server supports SSC and player is able to receive SSC packets.
    /// </summary>
    public bool SupportsChange { get; }
    
    /// <summary>
    /// Manages read-only inventory state. If enabled, then only plugins/modules/core can edit character.
    /// Player cannot edit character settings in that mode.
    /// </summary>
    public bool IsReadonly { get; set; }

    public NetItem this[int slot] { get; }
    public NetColor this[PlayerColorType type] { get; }

    public int Life { get; }
    public int LifeMax { get; }
    public int Mana { get; }
    public int ManaMax { get; }
    public int QuestsCompleted { get; }
    public PlayerInfo1 Stats1 { get; }
    public PlayerInfo2 Stats2 { get; }
    public PlayerInfo3 Stats3 { get; }
    public bool[] HideAccessories { get; }
    public byte HideMisc { get; }
    public int HairID { get; }
    public byte HairColor { get; }
    public byte SkinVariant { get; }

    /// <summary>
    /// Loads character model
    /// </summary>
    /// <param name="model">Model instance</param>
    /// <param name="sync">Is model needed for network sync</param>
    public void LoadCharacter(CharacterModel model, bool sync);

    /// <summary>
    /// Synchronises server character with player. 
    /// </summary>
    public void SyncCharacter();

    public void ReceiveSlot(IncomingPacket packet);
    public void ReceiveSetLife(IncomingPacket packet);
    public void ReceiveSetMana(IncomingPacket packet);
    public void ReceivePlayerInfo(IncomingPacket packet);
    public void ReceiveQuests(IncomingPacket packet);

    public bool SetSlot(SyncType? sync, int slot, NetItem item);
    public bool SetLife(SyncType? sync, int? current, int? max);
    public bool SetMana(SyncType? sync, int? current, int? max);
    public bool SetColor(SyncType? sync, PlayerColorType colorType, NetColor color);
    public bool SetQuests(SyncType? sync, int completed);
    public bool SetStats(SyncType? sync, PlayerInfo1? stats1 = null, PlayerInfo2? stats2 = null, PlayerInfo3? stats3 = null);
    public bool SetHides(SyncType? sync, bool[]? hideAccessories = null, byte? hideMisc = null);
    public bool SetSkin(SyncType? sync, byte? hairId = null, byte? hairColor = null, byte? skinVariant = null);

    public void SyncSlot(SyncType sync, int slot);
    public void SyncLife(SyncType sync);
    public void SyncMana(SyncType sync);
    public void SyncPlayerInfo(SyncType sync);
    public void SyncAngler(SyncType sync);

    public void SaveUpdate();
}