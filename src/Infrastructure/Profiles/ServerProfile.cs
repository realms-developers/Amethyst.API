namespace Amethyst.Infrastructure.Core.Profiles;

public sealed class ServerProfile
{
    internal ServerProfile(string name)
    {
        Name = name;
        DefaultLanguage = "en-US";
        Config = new ProfileConfigContainer();
        GenerationRules = new WorldGenerationRules();
    }

    public string Name { get; }

    public bool DebugMode { get; set; }

    public bool DisableFrameDebug { get; set; }

    public bool ForceUpdate { get; set; }

    public bool SSCMode { get; set; }

    public string? WorldToLoad { get; set; }

    public bool WorldRecreate { get; set; }

    public int MaxPlayers { get; set; }

    public int Port { get; set; }

    public string DefaultLanguage { get; set; }

    public string SavePath => Path.Combine(Environment.CurrentDirectory, "data", "profiles", Name);

    public ProfileConfigContainer Config { get; }
    public WorldGenerationRules GenerationRules { get; set; }
}
