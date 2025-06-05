using Amethyst.Systems.Commands.Base;

namespace Amethyst.Systems.Commands;

public record CompletedCommandInfo(ICommand Command, string CommandArgs, TimeSpan ExecutionDuration, DateTimeOffset ExecutionTimestamp);
