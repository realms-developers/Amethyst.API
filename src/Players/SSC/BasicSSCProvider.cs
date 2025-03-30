using Amethyst.Core;
using Amethyst.Network;
using Amethyst.Players.SSC.Interfaces;
using Amethyst.Storages.Mongo;
using Terraria.ID;

namespace Amethyst.Players.SSC;

public sealed class BasicSSCProvider : ISSCProvider
{
    public static SSCConfiguration Configuration => AmethystSession.Profile.Config.Get<SSCConfiguration>().Data;
    public static MongoModels<CharacterModel> Characters { get; } = MongoDatabase.Main.Get<CharacterModel>();

    public void Initialize()
    {
        AmethystSession.Profile.Config.Get<SSCConfiguration>().Load();
        AmethystSession.Profile.Config.Get<SSCConfiguration>().Modify(SetupConfiguration, true);
    }

    private static void SetupConfiguration(ref SSCConfiguration configuration)
    {
        configuration.StartItems ??=
        [
            new NetItem(ItemID.IronShortsword, 1, 0),
            new NetItem(ItemID.IronPickaxe, 1, 0),
            new NetItem(ItemID.IronAxe, 1, 0)
        ];

        configuration.StartLife = configuration.StartLife == 0 ? 100 : configuration.StartLife;
        configuration.StartMana = configuration.StartMana == 0 ? 20 : configuration.StartMana;
    }

    public CharacterModel GetModel(string name)
    {
        CharacterModel? model = Characters.Find(name);
        if (model != null)
        {
            return model;
        }

        SSCConfiguration cfg = Configuration;
        NetItem[] slots = new NetItem[350];
        Configuration.StartItems.CopyTo(slots);

        model = new CharacterModel(name)
        {
            MaxLife = cfg.StartLife,
            MaxMana = cfg.StartMana,
            Slots = slots
        };
        model.Save();

        return model;
    }

    public ICharacterWrapper CreateServersideWrapper(NetPlayer player) => new ServerCharacterWrapper(player);

    public struct SSCConfiguration
    {
        public List<NetItem> StartItems { get; set; }
        public int StartLife { get; set; }
        public int StartMana { get; set; }
    }
}
