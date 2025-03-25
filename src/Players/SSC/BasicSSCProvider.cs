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

    private SSCConfiguration SetupConfiguration(SSCConfiguration configuration)
    {
        configuration.StartItems ??= new List<NetItem>()
        {
            new NetItem(ItemID.IronShortsword, 1, 0),
            new NetItem(ItemID.IronPickaxe, 1, 0),
            new NetItem(ItemID.IronAxe, 1, 0)
        };

        configuration.StartLife = configuration.StartLife == 0 ? 120 : configuration.StartLife;
        configuration.StartMana = configuration.StartMana == 0 ? 60 : configuration.StartMana;

        return configuration;
    }

    public CharacterModel GetModel(string name)
    {
        var model = Characters.Find(name);
        if (model != null) return model;

        var cfg = Configuration;
        var slots = new NetItem[350];
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
    
    public ICharacterWrapper CreateServersideWrapper(NetPlayer player)
    {
        return new ServerCharacterWrapper(player);
    }

    public struct SSCConfiguration
    {
        public List<NetItem> StartItems;
        public int StartLife;
        public int StartMana;
    }
}