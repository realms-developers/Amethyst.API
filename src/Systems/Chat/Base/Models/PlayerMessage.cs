using Amethyst.Server.Entities.Players;
using Amethyst.Systems.Users.Base.Permissions;

namespace Amethyst.Systems.Chat;

public sealed class PlayerMessage
{
    public PlayerMessage(PlayerEntity player, string message, DateTimeOffset timestamp)
    {
        Player = player;
        Text = message;
        Timestamp = timestamp;
    }

    public PlayerEntity Player { get; }
    public string Text { get; }
    public DateTimeOffset Timestamp { get; }

    public bool IsCancelled { get; private set; }
    public string? ModifiedText { get; private set; }
    public bool IsModified => ModifiedText != null && ModifiedText != Text;

    public bool IsLocked { get; private set; }

    public void Cancel()
    {
        if (IsLocked)
            throw new InvalidOperationException("Cannot modify a locked message.");

        IsCancelled = true;
        AmethystLog.System.Error("PlayerMessage", $"Message '{Text}' was cancelled.");
    }

    public void Cancel(string reason)
    {
        if (IsLocked)
            throw new InvalidOperationException("Cannot modify a locked message.");

        IsCancelled = true;
        AmethystLog.System.Error("PlayerMessage", $"Message '{Text}' was cancelled: {reason}");
        Player.User?.Messages.ReplyError(reason);
    }

    public void Modify(string? newMessage)
    {
        if (IsLocked)
            throw new InvalidOperationException("Cannot modify a locked message.");

        if (Text.StartsWith('!') && Player.User?.Permissions.HasPermission("chat.ignore-modify") == PermissionAccess.HasPermission)
        {
            return;
        }

        ModifiedText = newMessage;

        if (newMessage != null)
            AmethystLog.System.Debug("PlayerMessage", $"{Player.Name} -> Message '{Text}' was modified to '{newMessage}'.");
        else
            AmethystLog.System.Debug("PlayerMessage", $"{Player.Name} -> Message '{Text}' was modified to previous value.");
    }

    internal void Lock()
    {
        IsLocked = true;
    }

    public override string ToString()
    {
        return Text;
    }
}
