using Amethyst.Server.Entities.Players;
using Amethyst.Storages;
using Amethyst.Systems.Characters.Base;
using Amethyst.Systems.Characters.Base.Enums;
using Amethyst.Systems.Characters.Base.Factories;
using Amethyst.Systems.Characters.Utilities;

namespace Amethyst.Systems.Characters.Serverside.Factories;

public sealed class ConfigModelFactory : IDefaultModelFactory
{
    public void Initialize() { }

    public ICharacterModel CreateModel(PlayerEntity player)
    {
        EmptyCharacterModel model = new EmptyCharacterModel();
        model.Name = player.Name;

        CopyPlayerInfo(player, ref model);
        CopyFromConfig(ref model);

        return model;
    }

    private void CopyPlayerInfo(PlayerEntity player, ref EmptyCharacterModel model)
    {
        model.Hair = player.TempPlayerInfo.HairID;
        model.HairDye = player.TempPlayerInfo.HairDyeID;
        model.SkinVariant = player.TempPlayerInfo.SkinVariant;

        model.Colors[(byte)PlayerColorType.SkinColor] = player.TempPlayerInfo.SkinColor;
        model.Colors[(byte)PlayerColorType.HairColor] = player.TempPlayerInfo.HairColor;
        model.Colors[(byte)PlayerColorType.ShirtColor] = player.TempPlayerInfo.ShirtColor;
        model.Colors[(byte)PlayerColorType.UnderShirtColor] = player.TempPlayerInfo.UnderShirtColor;
        model.Colors[(byte)PlayerColorType.PantsColor] = player.TempPlayerInfo.PantsColor;
        model.Colors[(byte)PlayerColorType.ShoesColor] = player.TempPlayerInfo.ShoeColor;
        model.Colors[(byte)PlayerColorType.EyesColor] = player.TempPlayerInfo.EyeColor;
    }

    private void CopyFromConfig(ref EmptyCharacterModel model)
    {
        for (int i = 0; i < CharactersConfiguration.Instance.DefaultItems.Count; i++)
        {
            model.Slots[i] = CharactersConfiguration.Instance.DefaultItems[i];
        }

        model.MaxLife = CharactersConfiguration.Instance.DefaultLife;
        model.MaxMana = CharactersConfiguration.Instance.DefaultMana;
    }
}
