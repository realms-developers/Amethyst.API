namespace Amethyst.Infrastructure.CLI.Input;

public delegate Task InputHandler(string args, CancellationToken cancellationToken);
