using Amethyst.Network;
using Amethyst.Systems.Characters.Base;
using Amethyst.Systems.Characters.Base.Enums;

namespace Amethyst.Systems.Characters.Utilities;

public sealed class EmptyCharacterModel : ICharacterModel
{
    public string Name { get; set; } = string.Empty;

    public NetItem[] Slots { get; set; } = new NetItem[450];

    public int MaxLife { get; set; }

    public int MaxMana { get; set; }

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
    public void Remove()
    {}

    public void Save()
    {}
}
