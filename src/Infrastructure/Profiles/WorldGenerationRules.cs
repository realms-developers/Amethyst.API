namespace Amethyst.Infrastructure.Profiles;

public sealed class WorldGenerationRules
{
    public string? Name { get; set; }

    public string? Seed { get; set; }

    public int? Width { get; set; }
    public int? Height { get; set; }

    public int GameMode { get; set;  }

    public int Evil { get; set; } = -1;
}
