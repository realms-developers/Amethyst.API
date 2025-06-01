using Amethyst.Server.Network.Structures;
using Amethyst.Storages.Mongo;
using Amethyst.Systems.Characters.Base;
using Amethyst.Systems.Characters.Base.Enums;

namespace Amethyst.Systems.Characters.Storages.MongoDB;

public sealed class MongoCharacterModel : DataModel, ICharacterModel
{
    public MongoCharacterModel(string name) : base(name)
    {
        Slots = new NetItem[350];
        Colors = new NetColor[8];
        HideAccessories = new bool[10];
    }

    public NetItem[] Slots { get; set; }

    public int MaxLife { get; set; }

    public int MaxMana { get; set; }

    public PlayerInfo1 Info1 { get; set; }

    public PlayerInfo2 Info2 { get; set; }

    public PlayerInfo3 Info3 { get; set; }

    public byte SkinVariant { get; set; }

    public byte Hair { get; set; }

    public byte HairDye { get; set; }

    public bool[] HideAccessories { get; set; }

    public byte HideMisc { get; set; }

    public NetColor[] Colors { get; set; }

    public int QuestsCompleted { get; set; }

    public override void Save()
    {
    }

    public override void Remove()
    {
    }
}
