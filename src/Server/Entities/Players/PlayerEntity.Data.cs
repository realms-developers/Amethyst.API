using Amethyst.Network.Packets;
using Amethyst.Network.Structures;
using Amethyst.Server.Entities.Base;
using Microsoft.Xna.Framework;

namespace Amethyst.Server.Entities.Players;

public sealed partial class PlayerEntity : IServerEntity
{
    public PlayerUpdate? PlayerUpdateInfo { get; set; }
    public PlayerInfo TempPlayerInfo { get; set; }
    public int TempLoadoutIndex { get; set; }

    public ref int Life => ref TPlayer.statLife;
    public ref int MaxLife => ref TPlayer.statLifeMax;

    public ref int Mana => ref TPlayer.statMana;
    public ref int MaxMana => ref TPlayer.statManaMax;

    public ref Vector2 Position => ref TPlayer.position;
    public ref Vector2 Velocity => ref TPlayer.velocity;

    /// <summary>
    /// Player difficulty. 0 - Normal, 1 - Expert, 2 - Master, 3 - Journey (Creative).
    /// </summary>
    public ref byte Difficulty => ref TPlayer.difficulty;

    public ref bool IsDead => ref TPlayer.dead;
    public NetDeathReason? LastDeathReason { get; set; }
}
