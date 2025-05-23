namespace Amethyst.Server.Entities.Base;

public interface IServerEntity
{
    public int Index { get; }

    public bool Active { get; }

    public string Name { get; }
}
