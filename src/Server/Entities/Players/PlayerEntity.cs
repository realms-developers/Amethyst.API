using Amethyst.Hooks;
using Amethyst.Hooks.Args.Players;
using Amethyst.Server.Entities.Base;
using Amethyst.Network.Engine;
using Amethyst.Systems.Users.Players;
using Terraria;
using Amethyst.Network.Handling.Handshake;

namespace Amethyst.Server.Entities.Players;

public sealed partial class PlayerEntity : IServerEntity, IDisposable
{
    internal PlayerEntity(int index, NetworkClient client)
    {
        Index = index;
        _client = client;

        Name = "";

        NetworkOperations = new PlayerNetworkOperations();
        ModerationOperations = new PlayerModerationOperations();
        GameplayOperations = new PlayerGameplayOperations();

        IP = client._socket.RemoteEndPoint?.ToString()?.Split(':')[0] ?? "0.0.0.0";
        UUID = "";

        Phase = ConnectionPhase.WaitingProtocol;
    }

    public ConnectionPhase Phase { get; set; }
    public Player TPlayer => Main.player[Index];
    public int Index { get; }
    public bool Active => Phase != ConnectionPhase.Disconnected;
    public string Name { get; set; }

    public string IP { get; set; }
    public string UUID { get; set; }

    public PlayerUser? User { get; private set; }

    internal NetworkClient _client;
    internal Dictionary<string, DateTime> _notifyDelay = [];

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

    public bool CanNotify(string messageType, TimeSpan delay)
    {
        if (!_notifyDelay.TryGetValue(messageType, out DateTime value))
        {
            value = DateTime.UtcNow.Add(delay);
            _notifyDelay.Add(messageType, value);
            return true;
        }

        if (value < DateTime.UtcNow)
        {
            _notifyDelay[messageType] = DateTime.UtcNow.Add(delay);
            return true;
        }

        return false;
    }

    public void CloseSocket()
    {
        _client.Dispose();
        Phase = ConnectionPhase.Disconnected;
        AmethystLog.Network.Info(nameof(PlayerEntity), $"Player #{Index} disconnected from {_client._socket.RemoteEndPoint}");
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
