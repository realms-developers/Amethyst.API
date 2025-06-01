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
        model.Hair = (byte)player.TPlayer.hair;
        model.HairDye = player.TPlayer.hairDye;

        model.Colors[(byte)PlayerColorType.SkinColor] = new(player.TPlayer.skinColor.R, player.TPlayer.skinColor.G, player.TPlayer.skinColor.B);
        model.Colors[(byte)PlayerColorType.HairColor] = new(player.TPlayer.hairColor.R, player.TPlayer.hairColor.G, player.TPlayer.hairColor.B);
        model.Colors[(byte)PlayerColorType.ShirtColor] = new(player.TPlayer.shirtColor.R, player.TPlayer.shirtColor.G, player.TPlayer.shirtColor.B);
        model.Colors[(byte)PlayerColorType.UnderShirtColor] = new(player.TPlayer.underShirtColor.R, player.TPlayer.underShirtColor.G, player.TPlayer.underShirtColor.B);
        model.Colors[(byte)PlayerColorType.PantsColor] = new(player.TPlayer.pantsColor.R, player.TPlayer.pantsColor.G, player.TPlayer.pantsColor.B);
        model.Colors[(byte)PlayerColorType.ShoesColor] = new(player.TPlayer.shoeColor.R, player.TPlayer.shoeColor.G, player.TPlayer.shoeColor.B);
        model.Colors[(byte)PlayerColorType.EyesColor] = new(player.TPlayer.eyeColor.R, player.TPlayer.eyeColor.G, player.TPlayer.eyeColor.B);
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
