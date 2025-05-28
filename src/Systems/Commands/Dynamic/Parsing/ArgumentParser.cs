using Amethyst.Systems.Users.Base;

namespace Amethyst.Systems.Commands.Dynamic.Parsing;

public delegate object? ArgumentParser(
    IAmethystUser user,
    string inputText,
    out string? errorMessage
);
