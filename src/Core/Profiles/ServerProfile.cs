namespace Amethyst.Core.Profiles;

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

    public bool SSCMode { get; set; }

    public string? WorldToLoad { get; set; }

    public bool WorldRecreate { get; set; }

    public int MaxPlayers { get; set; } = 1;

    public int Port { get; set; } = 7777;

    public string DefaultLanguage { get; set; }

    public string SavePath => Path.Combine(Environment.CurrentDirectory, "data", "profiles", Name);

    public ProfileConfigContainer Config { get; }
    public WorldGenerationRules GenerationRules { get; set; }
}
