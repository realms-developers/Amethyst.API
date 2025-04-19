using System.Reflection;
using Amethyst.Core;
using Amethyst.Network;

namespace Amethyst.Security;

public static class SecurityManager
{
    internal static Dictionary<string, RuleContainer> Rules = [];
    internal static SecurityConfiguration Configuration => AmethystSession.Profile.Config.Get<SecurityConfiguration>().Data;

    internal static void Initialize()
    {
        AmethystSession.Profile.Config.Get<SecurityConfiguration>().Load();
        AmethystSession.Profile.Config.Get<SecurityConfiguration>().Modify(SetupConfiguration, true);
    }

    internal static void LoadFrom(Assembly assembly)
    {
        foreach (Type type in assembly.GetTypes())
        {
            if (!type.IsSubclassOf(typeof(ISecurityRule)))
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
    }

    public static void RegisterRule(ISecurityRule rule)
    {
        if (Rules.ContainsKey(rule.Name))
        {
            throw new ArgumentException($"Security rule with name {rule.Name} is already registered.");
        }

        RuleContainer ruleContainer = new RuleContainer(rule);

        Rules.Add(rule.Name, ruleContainer);

        if (!Configuration.DisabledRules.Contains(rule.Name))
            ruleContainer.RequestLoad();
    }

    public static void UnregisterRule(string name)
    {
        if (!Rules.TryGetValue(name, out RuleContainer? value))
        {
            throw new ArgumentException($"Security rule with name {name} is not registered.");
        }

        value.RequestLoad();

        Rules.Remove(name);
    }
}
