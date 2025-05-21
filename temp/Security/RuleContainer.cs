using Amethyst.Network.Managing;

namespace Amethyst.Security;

internal sealed class RuleContainer
{
    internal RuleContainer(ISecurityRule rule)
    {
        Name = rule.Name;
        Rule = rule;
    }

    internal void RequestLoad()
    {
        if (IsLoaded)
        {
            return;
        }

        IsLoaded = true;

        Rule.Load(NetworkManager.Instance);
    }

    internal void RequestUnload()
    {
        if (!IsLoaded)
        {
            return;
        }

        IsLoaded = false;

        Rule.Unload(NetworkManager.Instance);
    }

    internal bool IsLoaded { get; set; }

    internal string Name { get; }
    internal ISecurityRule Rule { get; }
}
