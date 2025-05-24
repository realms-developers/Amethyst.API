using Amethyst.Hooks;
using Amethyst.Hooks.Args.Players;
using Amethyst.Server.Entities.Base;
using Amethyst.Server.Entities.Players.Modules;
using Amethyst.Systems.Users.Players;

namespace Amethyst.Server.Entities.Players;

public sealed class PlayerEntity : IServerEntity
{
    public PlayerEntity(int index)
    {
        Index = index;

        Name = "";

        AddModule(new PlayerNetworkModule(this, new()));
    }

    public int Index { get; }
    public bool Active { get; set; } = true;
    public string Name { get; }

    public PlayerUser? User { get; private set; }

    private Dictionary<Type, object> _modules = new();

    public void AddModule<TOperations>(IEntityModule<PlayerEntity, TOperations> module)
        where TOperations : class
    {
        ArgumentNullException.ThrowIfNull(module);

        if (module.BaseEntity != this)
            throw new InvalidOperationException("Module entity does not match player entity.");

        if (_modules.ContainsKey(module.GetType()))
            throw new InvalidOperationException($"Module of type {module.GetType().Name} already exists.");

        _modules.Add(module.GetType(), module);
    }

    public void RemoveModule<TOperations>()
        where TOperations : class
    {
        if (!_modules.Remove(typeof(TOperations)))
        {
            throw new KeyNotFoundException($"Module of type {typeof(TOperations).Name} not found.");
        }
    }

    public TModule GetModule<TModule>()
    {
        if (_modules.TryGetValue(typeof(TModule), out var module))
        {
            return (TModule)module;
        }

        throw new KeyNotFoundException($"Module of type {typeof(TModule).Name} not found.");
    }

    public void SetUser(PlayerUser? user)
    {
        var result = HookRegistry.GetHook<PlayerSetUserArgs>()
            ?.Invoke(new PlayerSetUserArgs(this, User, user));

        if (result == null)
            throw new InvalidOperationException("PlayerSetUserArgs hook not found.");

        if (result.IsCancelled == true)
        {
            return;
        }

        User = result.IsModified == true ? result.Args?.New : user;

        HookRegistry.GetHook<PlayerPostSetUserArgs>()
            ?.Invoke(new PlayerPostSetUserArgs(this, User));
    }

    public PlayerNetworkModule GetNetwork() => GetModule<PlayerNetworkModule>();
}
