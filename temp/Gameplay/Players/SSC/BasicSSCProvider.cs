using Amethyst.Gameplay.Players.SSC.Interfaces;
using Amethyst.Network;
using Amethyst.Storages.Config;
using Amethyst.Storages.Mongo;
using Terraria.ID;

namespace Amethyst.Gameplay.Players.SSC;

public sealed class BasicSSCProvider : ISSCProvider
{
    internal static readonly Configuration<SSCConfiguration> _sscCfg = new(typeof(SSCConfiguration).FullName!, new());

    public static SSCConfiguration Configuration => _sscCfg.Data;
    public static MongoModels<CharacterModel> Characters { get; } = MongoDatabase.Main.Get<CharacterModel>();

    public BasicSSCProvider() => _sscCfg.Load();

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
            Slots = slots,
        };
        model.Save();

        return model;
    }

    public static CharacterModel GetModelByPlayer(NetPlayer player)
    {
        CharacterModel? model = Characters.Find(player.Name);
        if (model != null)
        {
            return model;
        }

        SSCConfiguration cfg = Configuration;
        NetItem[] slots = new NetItem[350];
        Configuration.StartItems.CopyTo(slots);

        model = new CharacterModel(player.Name)
        {
            MaxLife = cfg.StartLife,
            MaxMana = cfg.StartMana,
            Slots = slots,

            HideAccessories = player._initHideAccessories,
            HideMisc = player._initHideMisc,
            HairDye = player._initHairDye,
            Hair = player._initHair,

            Colors = player._initColors,

            Info1 = player._initInfo1,
            Info2 = player._initInfo2,
            Info3 = player._initInfo3,
        };
        model.Save();

        return model;
    }

    public ICharacterWrapper CreateServersideWrapper(NetPlayer player) => new ServerCharacterWrapper(player, GetModelByPlayer(player));

    public class SSCConfiguration
    {
        public List<NetItem> StartItems { get; set; } =
        [
            new NetItem(ItemID.IronShortsword, 1, 0),
            new NetItem(ItemID.IronPickaxe, 1, 0),
            new NetItem(ItemID.IronAxe, 1, 0)
        ];

        public int StartLife { get; set; } = 100;
        public int StartMana { get; set; } = 20;
    }
}
