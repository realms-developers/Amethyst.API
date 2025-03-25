using Amethyst.Core;
using Amethyst.Players.Extensions;
using Amethyst.Players.SSC;
using Amethyst.Players.SSC.Interfaces;
using Amethyst.Storages.Mongo;

using Timer = System.Timers.Timer;

namespace Amethyst.Players;

public static class PlayerManager
{
    public static PlayerTracker Tracker { get; } = new PlayerTracker();
    public static MongoModels<CharacterModel> Characters { get; } = MongoDatabase.Main.Get<CharacterModel>();

    public static ISSCProvider SSCProvider { get; set; } = new BasicSSCProvider();
    public static bool IsSSCEnabled => AmethystSession.Profile.SSCMode;

    private static Timer? _UpdateTimer;

    internal static void Initialize()
    {
        SSCProvider.Initialize();
        PlayerNetworking.Initialize();

        if (IsSSCEnabled)
        {
            _UpdateTimer = new Timer(1000)
            {
                AutoReset = true,
                Enabled = true
            };
            _UpdateTimer.Elapsed += OnElapsed;
        }
    }

    private static void OnElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        foreach (var plr in Tracker)
            plr.Character?.SaveUpdate();
    }
}