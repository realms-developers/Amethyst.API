using Amethyst.Network.Structures;
using Amethyst.Storages.Config;
using Terraria.ID;

namespace Amethyst.Storages;

public sealed class CharactersConfiguration
{
    static CharactersConfiguration() => Configuration.Load();

    public static Configuration<CharactersConfiguration> Configuration { get; } = new("Characters", new());
    public static CharactersConfiguration Instance => Configuration.Data;

    public string? MongoConnection { get; set; }
    public string? MongoDatabaseName { get; set; }
    public string MongoCollectionName { get; set; } = "CharactersCollection";

    public bool EnableCaching { get; set; } = true;

    public List<NetItem> DefaultItems { get; set; } = new()
    {
        new NetItem(ItemID.CopperShortsword, 1, 0),
        new NetItem(ItemID.CopperPickaxe, 1, 0),
        new NetItem(ItemID.CopperAxe, 1, 0),
    };

    public int DefaultLife { get; set; } = 100;
    public int DefaultMana { get; set; } = 20;
}
