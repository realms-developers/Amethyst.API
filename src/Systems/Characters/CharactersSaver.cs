using Amethyst.Server.Entities;
using Amethyst.Server.Entities.Players;

namespace Amethyst.Systems.Characters;

public static class CharactersSaver
{
    private static Timer? _autoSaveTimer;

    public static bool IsSetup => _autoSaveTimer != null;

    public static void Setup(TimeSpan autoSaveInterval)
    {
        if (_autoSaveTimer != null)
        {
            throw new InvalidOperationException("CharactersSaver is already set up.");
        }

        _autoSaveTimer = new Timer(AutoSaveCallback, null, TimeSpan.Zero, autoSaveInterval);
    }
    public static void Stop()
    {
        if (_autoSaveTimer == null)
        {
            throw new InvalidOperationException("CharactersSaver is not set up.");
        }

        _autoSaveTimer.Dispose();
        _autoSaveTimer = null;
    }

    private static void AutoSaveCallback(object? state)
    {
        try
        {
            foreach (PlayerEntity player in EntityTrackers.Players)
            {
                if (player.User != null && player.User.Character != null && player.User.Character.CanSaveModel)
                {
                    player.User.Character.Save();
                }
            }
        }
        catch (Exception ex)
        {
            AmethystLog.System.Error(nameof(CharactersSaver), $"Auto-save failed: {ex.ToString()}");
        }
    }
}
