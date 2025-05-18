
namespace Amethyst.Gameplay.Players;

public sealed class PlayerRules
{
    internal PlayerRules(NetPlayer plr)
    {
        Player = plr;

        _rules = new List<string>[1];

        for (int i = 0; i < _rules.Length; i++)
        {
            _rules[i] = [];
        }
    }

    public NetPlayer Player { get; }

    public bool this[InteractRuleType ruleType]
    {
        get
        {
            return Player.IsCapable && _rules[(int)ruleType].Count == 0;
        }
    }

    private readonly List<string>[] _rules = new List<string>[1];
    private readonly Lock _lock = new();

    public void Block(InteractRuleType type, string reason)
    {
        lock (_lock)
        {
            if (!_rules[(int)type].Contains(reason))
            {
                _rules[(int)type].Add(reason);
            }
        }
    }

    public void Unblock(InteractRuleType type, string reason)
    {
        lock (_lock)
        {
            _rules[(int)type].Remove(reason);
        }
    }

    public enum InteractRuleType
    {
        Items,
        World
    }
}
