namespace Amethyst.Systems.Users.Base.Extensions;

public interface IUserExtension
{
    string Name { get; }

    void Load(IAmethystUser user);
    void Unload(IAmethystUser user);
}
