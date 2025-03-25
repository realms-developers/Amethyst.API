using Amethyst.Network;
using Amethyst.Players.SSC.Enums;
using Amethyst.Storages.Mongo;
using MongoDB.Bson.Serialization.Attributes;

namespace Amethyst.Players.SSC;

[BsonIgnoreExtraElements]
public sealed class CharacterModel : DataModel
{
    public CharacterModel(string name) : base(name)
    {
        Slots = new NetItem[350];
        MaxLife = 100;
        MaxMana = 200;
        HideAccessories = new bool[10];
        Colors = new NetColor[7];
    }

    public NetItem[] Slots;
    public int MaxLife;
    public int MaxMana;
    public PlayerInfo1  Info1;
    public PlayerInfo2 Info2;
    public PlayerInfo3 Info3;
    public byte SkinVariant;
    public byte Hair;
    public byte HairDye;
    public bool[] HideAccessories;
    public byte HideMisc;
    public NetColor[] Colors;
    public int QuestsCompleted;

    public override void Save()
    {
        PlayerManager.Characters.Save(this);
    }

    public override void Remove()
    {
        PlayerManager.Characters.Remove(Name);
    }
}
