using Amethyst.Hooks;
using Amethyst.Hooks.Args.Players;
using Amethyst.Server.Entities.Base;
using Amethyst.Systems.Users.Players;
using Microsoft.Xna.Framework;
using Terraria;

namespace Amethyst.Server.Entities.Players;

public sealed partial class PlayerEntity : IServerEntity
{
    public PlayerEntity(int index)
    {
        Index = index;

        Name = "";

        NetworkOperations = new PlayerNetworkOperations();
        ModerationOperations = new PlayerModerationOperations();
    }

    public Player TPlayer => Main.player[Index];
    public int Index { get; }
    public bool Active { get; set; } = true;
    public string Name { get; }

    public Vector2 Position
    {
        get => TPlayer.position;
        set => TPlayer.position = value;
    }

    public PlayerUser? User { get; private set; }

    public void SetUser(PlayerUser? user)
    {
        var result = HookRegistry.GetHook<PlayerSetUserArgs>()
            ?.Invoke(new PlayerSetUserArgs(this, User, user));

        if (result == null)
            throw new InvalidOperationException("PlayerSetUserArgs hook not found.");

        if (result.IsCancelled == true)
        {
            return;
        }

        User = result.IsModified == true ? result.Args?.New : user;

        HookRegistry.GetHook<PlayerPostSetUserArgs>()
            ?.Invoke(new PlayerPostSetUserArgs(this, User));
    }
}
