using Amethyst.Network.Packets;
using Amethyst.Server.Entities.Base;
using Microsoft.Xna.Framework;

namespace Amethyst.Server.Entities.Players;

public sealed partial class PlayerEntity : IServerEntity
{
    public PlayerInfo TempPlayerInfo { get; set; }
    public int TempLoadoutIndex { get; set; }

    public ref int Life => ref TPlayer.statLife;
    public ref int MaxLife => ref TPlayer.statLifeMax;

    public ref int Mana => ref TPlayer.statMana;
    public ref int MaxMana => ref TPlayer.statManaMax;

    public ref Vector2 Position => ref TPlayer.position;
    public ref Vector2 Velocity => ref TPlayer.velocity;
}
