namespace Amethyst.Kernel.Profiles;

public sealed class ServerProfile
{
    internal ServerProfile(string name) => Name = name;

    public string Name { get; }

    public bool DebugMode { get; set; }

    public bool DisableFrameDebug { get; set; }

    public bool ForceUpdate { get; set; }

    public bool SSCMode { get; set; }

    public string? WorldToLoad { get; set; }

    public bool WorldRecreate { get; set; }

    public int MaxPlayers { get; set; }

    public int Port { get; set; }

    public char CommandPrefix { get; set; } = '/';

    public string DefaultLanguage { get; set; } = "en-US";

    public string SavePath => Path.Combine(Environment.CurrentDirectory, "data", "profiles", Name);

    public WorldGenerationRules GenerationRules { get; set; } = new();
}
