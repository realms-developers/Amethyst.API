using System.Reflection;
using Amethyst.Core;
using Amethyst.Security.GameBans;

namespace Amethyst.Security;

public static class SecurityManager
{
    public const string IgnorePermission = "security.ignore";
    public const string ModeratorPermission = "security.moderator";

    public static GameObjectBlocker ItemBans { get; } = new GameObjectBlocker("ItemBanCollection");
    public static GameObjectBlocker ProjectileBans { get; } = new GameObjectBlocker("ProjectileBanCollection");

    public static GameObjectBlocker TileBans { get; } = new GameObjectBlocker("TileBanCollection");
    public static GameObjectBlocker WallBans { get; } = new GameObjectBlocker("WallBanCollection");

    public static GameObjectBlocker TileSafety { get; } = new GameObjectBlocker("TileSafetyCollection");
    public static GameObjectBlocker WallSafety { get; } = new GameObjectBlocker("TileSafetyCollection");

    internal static Dictionary<string, RuleContainer> Rules = [];
    internal static SecurityConfiguration Configuration => AmethystSession.Profile.Config.Get<SecurityConfiguration>().Data;

    internal static void Initialize()
    {
        AmethystSession.Profile.Config.Get<SecurityConfiguration>().Load();
        AmethystSession.Profile.Config.Get<SecurityConfiguration>().Modify(SetupConfiguration, true);

        LoadFrom(typeof(SecurityManager).Assembly);
    }

    internal static void LoadFrom(Assembly assembly)
    {
        foreach (Type type in assembly.GetExportedTypes())
        {
            if (type.GetInterface(nameof(ISecurityRule)) == null)
            {
                continue;
            }

            ISecurityRule rule = (Activator.CreateInstance(type) as ISecurityRule)!;

            RegisterRule(rule);
        }
    }

    private static void SetupConfiguration(ref SecurityConfiguration configuration)
    {
        configuration.DisabledRules ??= [];

        configuration.PerSecondLimitPackets ??= [];
        configuration.OneTimePackets ??= [1, 6, 8];
        configuration.DisabledPackets ??= [136];

        configuration.PerSecondLimitModules ??= [];
        configuration.OneTimeModules ??= [];
        configuration.DisabledModules ??= [];

        configuration.NotifyModerators ??= true;

        configuration.EnableNicknameFilter = true;
        configuration.NicknameFilter ??= " ~!@#$%^&*()_+`1234567890-=ё\"№;:?\\|qwertyuiopasdfghjklzxcvbnm{}[];'<>,./ёйцукенгшщзхъфывапролджэячсмитьбю";

        configuration.MaxAllowedLife ??= 500;
        configuration.MaxAllowedMana ??= 200;

        configuration.KillTileRange ??= 64;
        configuration.PlaceTileRange ??= 32;
        configuration.ReplaceTileRange ??= 32;

        configuration.KillTileThreshold ??= 80;
        configuration.PlaceTileThreshold ??= 20;
        configuration.ReplaceTileThreshold ??= 20;

        configuration.KillWallRange ??= 32;
        configuration.PlaceWallRange ??= 32;
        configuration.ReplaceWallRange ??= 32;

        configuration.KillWallThreshold ??= 50;
        configuration.PlaceWallThreshold ??= 50;
        configuration.ReplaceWallThreshold ??= 50;

        configuration.ItemDropThreshold ??= 8;
        configuration.ReturnDroppedItemInThreshold ??= true;

        configuration.MaxProjectilesPerUser ??= 60;
        configuration.ProjectileCreateThreshold ??= 45;

        configuration.AllowedMessages ??=
        [
            "security_tile_safety",
            "security_tile_bans"
        ];
    }

    public static void RegisterRule(ISecurityRule rule)
    {
        if (Rules.ContainsKey(rule.Name))
        {
            throw new ArgumentException($"Security rule with name {rule.Name} is already registered.");
        }

        RuleContainer ruleContainer = new(rule);

        Rules.Add(rule.Name, ruleContainer);

        //AmethystLog.Security.Info("Security", $"Registered security rule '{rule.Name}'!");

        if (!Configuration.DisabledRules.Contains(rule.Name))
        {
            ruleContainer.RequestLoad();
        }
    }

    public static void UnregisterRule(string name)
    {
        if (!Rules.TryGetValue(name, out RuleContainer? rule))
        {
            throw new ArgumentException($"Security rule with name {name} is not registered.");
        }

        rule.RequestLoad();

        AmethystLog.Security.Info("Security", $"Unregistered security rule '{rule.Name}'!");

        Rules.Remove(name);
    }
}
