using Amethyst.Hooks;
using Amethyst.Server.Entities.Base;
using Amethyst.Server.Entities.Players.Hooks;
using Amethyst.Systems.Users.Players;

namespace Amethyst.Server.Entities.Players;

public sealed class PlayerEntity : IServerEntity
{
    public PlayerEntity(int index)
    {
        Index = index;

        Name = "";

        Network = new PlayerNetworkUtils(this);
    }

    public int Index { get; }
    public bool Active { get; set; } = true;
    public string Name { get; }

    public PlayerUser? User { get; private set; }

    public PlayerNetworkUtils Network { get; }

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
