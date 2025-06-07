using Amethyst.Systems.Commands.Base.Metadata;

namespace Amethyst.Systems.Commands.Base;

public interface ICommand
{
    Guid LoadIdentifier { get; }

    CommandRepository Repository { get; }
    ICommandInvoker Invoker { get; }
    CommandMetadata Metadata { get; set; }

    // IAmethystUser - any
    // PlayerUser - only players
    // ArtificialUser - only artificial users (like console)
    Type PreferredUser { get; }
}
