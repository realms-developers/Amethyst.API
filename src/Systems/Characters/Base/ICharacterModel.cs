using Amethyst.Server.Network.Structures;
using Amethyst.Systems.Characters.Base.Enums;

namespace Amethyst.Systems.Characters.Base;

public interface ICharacterModel
{
    string Name { get; }

    NetItem[] Slots { get; set; }
    int MaxLife { get; set; }
    int MaxMana { get; set; }
    PlayerInfo1 Info1 { get; set; }
    PlayerInfo2 Info2 { get; set; }
    PlayerInfo3 Info3 { get; set; }
    byte SkinVariant { get; set; }
    byte Hair { get; set; }
    byte HairDye { get; set; }
    bool[] HideAccessories { get; set; }
    byte HideMisc { get; set; }
    NetColor[] Colors { get; set; }
    int QuestsCompleted { get; set; }

    void Save();
    void Remove();
}
