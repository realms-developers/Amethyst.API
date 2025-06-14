using Amethyst.Hooks.Autoloading;
using Amethyst.Server.Entities.Players;

namespace Amethyst.Hooks.Args.Players;

[AutoloadHook(true, false)]
public sealed class PlayerIdentifiedArgs
{
    public PlayerIdentifiedArgs(PlayerEntity player)
    {
        Player = player;
        Name = player.Name;
        UUID = player.UUID;
        HashedUUID = player.HashedUUID;
        IPAddress = player.IP;
    }

    public PlayerEntity Player { get; }
    public string Name { get; }
    public string UUID { get; }
    public string HashedUUID { get; }
    public string IPAddress { get; }
}
