using Amethyst.Network.Structures;
using Amethyst.Storages.Config;
using Terraria.ID;

namespace Amethyst.Systems.Characters;

public sealed class CharactersConfiguration
{
    static CharactersConfiguration() => Configuration.Load();

    public static Configuration<CharactersConfiguration> Configuration { get; } = new("CharactersConfiguration", new());
    public static CharactersConfiguration Instance => Configuration.Data;

    public string? MongoConnection { get; set; }
    public string? MongoDatabaseName { get; set; }
    public string MongoCollectionName { get; set; } = "CharactersCollection";

    public List<NetItem> DefaultItems { get; set; } =
    [
        new NetItem(ItemID.CopperShortsword, 1, 0),
        new NetItem(ItemID.CopperPickaxe, 1, 0),
        new NetItem(ItemID.CopperAxe, 1, 0),
    ];

    public int DefaultLife { get; set; } = 100;
    public int DefaultMana { get; set; } = 20;
}
