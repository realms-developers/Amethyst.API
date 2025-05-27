namespace Amethyst.Server.Network.Enums;

public enum TileInteractType : byte
{
    // main
    KillTile = 0, KillTileNoItem = 4, KillTileV3 = 20,
    PlaceTile = 1,

    KillWall = 2, PlaceWall = 3,

    ReplaceTile = 21, ReplaceWall = 22,

    // wiring
    PlaceWire = 5, PlaceWire2 = 10, PlaceWire3 = 12, PlaceWire4 = 16,
    KillWire = 6, KillWire2 = 11, KillWire3 = 13, KillWire4 = 17,
    PlaceActuator = 8, KillActuator = 9,

    Actuate = 19,

    // other
    PoundTile = 7, SlopeTile = 14, SlopeAndPound = 23,

    PokeLogicGate = 18,

    FrameTrack = 15
}
