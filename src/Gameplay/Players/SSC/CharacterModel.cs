using Amethyst.Network;
using Amethyst.Players.SSC.Enums;
using Amethyst.Storages.Mongo;
using MongoDB.Bson.Serialization.Attributes;

namespace Amethyst.Players.SSC;

[BsonIgnoreExtraElements]
public sealed class CharacterModel(string name) : DataModel(name)
{
    public NetItem[] Slots { get; set; } = new NetItem[350];
    public int MaxLife { get; set; } = 500;
    public int MaxMana { get; set; } = 200;
    public PlayerInfo1 Info1 { get; set; }
    public PlayerInfo2 Info2 { get; set; }
    public PlayerInfo3 Info3 { get; set; }
    public byte SkinVariant { get; set; }
    public byte Hair { get; set; }
    public byte HairDye { get; set; }
    public bool[] HideAccessories { get; set; } = new bool[10];
    public byte HideMisc { get; set; }
    public NetColor[] Colors { get; set; } = new NetColor[7];
    public int QuestsCompleted { get; set; }

    public override void Save() => PlayerManager.Characters.Save(this);

    public override void Remove() => PlayerManager.Characters.Remove(Name);
}
