using Amethyst.Storages.Mongo;

namespace Amethyst.Security.GameBans;

public sealed class GameObjectBan : DataModel
{
    public GameObjectBan(string name) : base(name)
    {
    }

    public override void Save() => throw new InvalidOperationException("GameObjectBan does not support Save().");
    public override void Remove() => throw new InvalidOperationException("GameObjectBan does not support Save().");
}
