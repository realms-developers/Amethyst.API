using Microsoft.Xna.Framework;

namespace Amethyst.Network;

public static class NetExtensions
{
    public static Color ReadColor(this BinaryReader reader)
    {
        return new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
    }
    public static NetColor ReadNetColor(this BinaryReader reader)
    {
        return new NetColor(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
    }
    public static Vector2 ReadVector2(this BinaryReader reader)
    {
        return new Vector2(reader.ReadSingle(), reader.ReadSingle());
    }

    public static T Read<T>(this BinaryReader reader) where T : Enum
    {
        return (T)(object)reader.ReadByte();
    }

    public static NetColor ToNet(this Color color) => new NetColor(color.R, color.G, color.B);
}