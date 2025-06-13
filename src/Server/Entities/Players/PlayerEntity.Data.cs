using Amethyst.Network.Packets;
using Amethyst.Network.Structures;
using Amethyst.Server.Entities.Base;
using Microsoft.Xna.Framework;
using Terraria;

namespace Amethyst.Server.Entities.Players;

public sealed partial class PlayerEntity : IServerEntity
{
    public PlayerUpdate? PlayerUpdateInfo { get; set; }
    public PlayerInfo TempPlayerInfo { get; set; }
    public int TempLoadoutIndex { get; set; }

    public ref float Stealth => ref TPlayer.stealth;

    public ref int Life => ref TPlayer.statLife;
    public ref int MaxLife => ref TPlayer.statLifeMax;

    public ref int Mana => ref TPlayer.statMana;
    public ref int MaxMana => ref TPlayer.statManaMax;

    public ref Vector2 Position => ref TPlayer.position;
    public ref Vector2 Velocity => ref TPlayer.velocity;

    public ref Item HeldItem => ref TPlayer.inventory[TPlayer.selectedItem];

    /// <summary>
    /// Player difficulty. 0 - Normal, 1 - Expert, 2 - Master, 3 - Journey (Creative).
    /// </summary>
    public ref byte Difficulty => ref TPlayer.difficulty;

    public ref bool IsDead => ref TPlayer.dead;
    public NetDeathReason? LastDeathReason { get; set; }

    public ref bool IsInPvP => ref TPlayer.hostile;
    public ref int Team => ref TPlayer.team;

    public int TalkNPC
    {
        get => TPlayer.talkNPC;
        set => TPlayer.talkNPC = value;
    }

    public NetBitsByte Zone1
    {
        get => (byte)TPlayer.zone1;
        set => TPlayer.zone1 = (byte)value;
    }
    public NetBitsByte Zone2
    {
        get => (byte)TPlayer.zone2;
        set => TPlayer.zone2 = (byte)value;
    }
    public NetBitsByte Zone3
    {
        get => (byte)TPlayer.zone3;
        set => TPlayer.zone3 = (byte)value;
    }
    public NetBitsByte Zone4
    {
        get => (byte)TPlayer.zone4;
        set => TPlayer.zone4 = (byte)value;
    }
    public NetBitsByte Zone5
    {
        get => (byte)TPlayer.zone5;
        set => TPlayer.zone5 = (byte)value;
    }
}
