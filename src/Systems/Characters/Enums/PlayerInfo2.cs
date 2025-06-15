namespace Amethyst.Systems.Characters.Enums;

[Flags]
public enum PlayerInfo2 : byte
{
   UsingBiomeTorches = 1,
   HappyTorchTime = 2,
   UnlockedBiomeTorches = 4,
   UnlockedSuperCart = 8,
   EnabledSuperCart = 16,
}
