using System.Reflection;
using Amethyst.Core;
using Amethyst.Network;

namespace Amethyst.Security;

public static class SecurityManager
{
    public const string IgnorePermission = "security.ignore";
    public const string ModeratorPermission = "security.moderator";

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
        configuration.OneTimePackets ??= [ 1, 6, 8 ];
        configuration.DisabledPackets ??= [ 136 ];

        configuration.PerSecondLimitModules ??= [];
        configuration.OneTimeModules ??= [];
        configuration.DisabledModules ??= [];

        configuration.NotifyModerators ??= true;

        configuration.EnableNicknameFilter = true;
        configuration.NicknameFilter ??= " ~!@#$%^&*()_+`1234567890-=ё\"№;:?\\|qwertyuiopasdfghjklzxcvbnm{}[];'<>,./ёйцукенгшщзхъфывапролджэячсмитьбю";

        configuration.MaxAllowedLife ??= 500;
        configuration.MaxAllowedMana ??= 200;
    }

    public static void RegisterRule(ISecurityRule rule)
    {
        if (Rules.ContainsKey(rule.Name))
        {
            throw new ArgumentException($"Security rule with name {rule.Name} is already registered.");
        }

        RuleContainer ruleContainer = new RuleContainer(rule);

        Rules.Add(rule.Name, ruleContainer);

        AmethystLog.Security.Info("Security", $"Registered security rule '{rule.Name}'!");

        if (!Configuration.DisabledRules.Contains(rule.Name))
            ruleContainer.RequestLoad();
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
