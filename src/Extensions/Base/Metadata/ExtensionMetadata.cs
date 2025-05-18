namespace Amethyst.Extensions.Base.Metadata;

public record ExtensionMetadata(
    string Name,
    string Author,
    string? Description,
    Version Version
);
