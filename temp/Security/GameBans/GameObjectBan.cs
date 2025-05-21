using Amethyst.Storages.Mongo;

namespace Amethyst.Security.GameBans;

public sealed class GameObjectBan(string name) : DataModel(name)
{
    public override void Save() => throw new InvalidOperationException("GameObjectBan does not support Save().");
    public override void Remove() => throw new InvalidOperationException("GameObjectBan does not support Save().");
}
