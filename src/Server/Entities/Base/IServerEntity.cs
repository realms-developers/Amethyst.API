namespace Amethyst.Server.Entities.Base;

public interface IServerEntity
{
    int Index { get; }

    bool Active { get; }

    string Name { get; }
}
