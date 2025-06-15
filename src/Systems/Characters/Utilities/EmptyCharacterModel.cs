using Amethyst.Network.Structures;
using Amethyst.Systems.Characters.Base;
using Amethyst.Systems.Characters.Enums;
using Amethyst.Systems.Characters.Storages.MongoDB;

namespace Amethyst.Systems.Characters.Utilities;

public sealed class EmptyCharacterModel : ICharacterModel
{
    public string Name { get; set; } = string.Empty;

    public NetItem[] Slots { get; set; } = new NetItem[350];

    public int MaxLife { get; set; } = 100;

    public int MaxMana { get; set; } = 20;

    public PlayerInfo1 Info1 { get; set; }

    public PlayerInfo2 Info2 { get; set; }

    public PlayerInfo3 Info3 { get; set; }

    public byte SkinVariant { get; set; }

    public byte Hair { get; set; }

    public byte HairDye { get; set; }

    public bool[] HideAccessories { get; set; } = new bool[10];

    public byte HideMisc { get; set; }

    public NetColor[] Colors { get; set; } = new NetColor[8];

    public int QuestsCompleted { get; set; }
    public void Remove()
    { }

    public void Save()
    { }

    public MongoCharacterModel ToMongoModel()
    {
        MongoCharacterModel model = new(Name)
        {
            Slots = Slots,
            MaxLife = MaxLife,
            MaxMana = MaxMana,
            Info1 = Info1,
            Info2 = Info2,
            Info3 = Info3,
            SkinVariant = SkinVariant,
            Hair = Hair,
            HairDye = HairDye,
            HideAccessories = HideAccessories,
            HideMisc = HideMisc,
            Colors = Colors,
            QuestsCompleted = QuestsCompleted
        };

        return model;
    }
}
