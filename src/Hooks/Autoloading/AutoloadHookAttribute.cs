namespace Amethyst.Hooks.Autoloading;

[AttributeUsage(AttributeTargets.Class)]
public sealed class AutoloadHookAttribute : Attribute
{
    public AutoloadHookAttribute()
    {
        CanBeIgnored = false;
        CanBeChanged = false;
    }

    public AutoloadHookAttribute(bool canBeIgnored, bool canBeChanged)
    {
        CanBeIgnored = canBeIgnored;
        CanBeChanged = canBeChanged;
    }

    public bool CanBeIgnored { get; }
    public bool CanBeChanged { get; }
}
