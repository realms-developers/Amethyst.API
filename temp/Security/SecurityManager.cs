using System.Reflection;
using Amethyst.Security.GameBans;
using Amethyst.Storages.Config;

namespace Amethyst.Security;

public static class SecurityManager
{
    internal static readonly Configuration<SecurityConfiguration> _securityCfg = new(typeof(SecurityConfiguration).FullName!, new());

    public const string IgnorePermission = "security.ignore";
    public const string ModeratorPermission = "security.moderator";

    public static GameObjectBlocker ItemBans { get; } = new GameObjectBlocker("ItemBanCollection");
    public static GameObjectBlocker ProjectileBans { get; } = new GameObjectBlocker("ProjectileBanCollection");

    public static GameObjectBlocker TileBans { get; } = new GameObjectBlocker("TileBanCollection");
    public static GameObjectBlocker WallBans { get; } = new GameObjectBlocker("WallBanCollection");

    public static GameObjectBlocker TileSafety { get; } = new GameObjectBlocker("TileSafetyCollection");
    public static GameObjectBlocker WallSafety { get; } = new GameObjectBlocker("TileSafetyCollection");

    internal static Dictionary<string, RuleContainer> Rules = [];
    internal static SecurityConfiguration Configuration => _securityCfg.Data;

    public static void Initialize()
    {
        _securityCfg.Load();

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
