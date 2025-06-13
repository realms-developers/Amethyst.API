using Amethyst.Hooks;
using Amethyst.Hooks.Args.Players;
using Amethyst.Hooks.Context;
using Amethyst.Network;
using Amethyst.Network.Handling.Packets.Handshake;
using Amethyst.Server.Entities.Base;
using Amethyst.Systems.Chat;
using Amethyst.Systems.Chat.Misc.Context;
using Terraria;

namespace Amethyst.Server.Entities.Players.Tracking;

public sealed class PlayerManager(PlayerTracker tracker) : IEntityManager<PlayerEntity>
{
    public IEntityTracker<PlayerEntity> Tracker => _tracker;

    private readonly PlayerTracker _tracker = tracker;

    private static readonly Func<PlayerTrackerInsertArgs, HookResult<PlayerTrackerInsertArgs>> _insertHook = HookRegistry.GetHook<PlayerTrackerInsertArgs>()!.Invoke;
    private static readonly Func<PlayerTrackerRemoveArgs, HookResult<PlayerTrackerRemoveArgs>> _removeHook = HookRegistry.GetHook<PlayerTrackerRemoveArgs>()!.Invoke;

    public void Insert(int index, PlayerEntity entity)
    {
        if (_tracker._players[index] != null)
        {
            throw new InvalidOperationException($"Player at index {index} already exists.");
        }

        _tracker._players[index] = entity;

        Main.player[index] = new();
        Main.player[index].active = true;
        Main.player[index].whoAmI = index;

        _insertHook.Invoke(new PlayerTrackerInsertArgs(entity));
    }

    public void Remove(int index)
    {
        PlayerEntity plr = _tracker._players[index] ?? throw new InvalidOperationException($"Player at index {index} does not exist.");
        if (plr.Phase == ConnectionPhase.Connected)
        {
            ServerChat.MessagePlayerLeft.Invoke(new PlayerLeftMessageContext(plr));
        }

        if (Main.player[index] != null)
        {
            Main.player[index].active = false;
        }

        _removeHook.Invoke(new PlayerTrackerRemoveArgs(plr));

        plr.Dispose();
        _tracker._players[index] = null!;
    }
}
