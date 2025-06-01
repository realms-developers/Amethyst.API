#pragma warning disable CA1051

using Terraria.Localization;

namespace Amethyst.Server.Network.Structures;

public struct NetText
{
    public NetText()
    {
        Mode = 0;
        Text = string.Empty;
        Substitutions = null;
    }
    public NetText(byte textMode, string text, NetText[]? substitutions = null)
    {
        Mode = textMode;
        Text = text;
        Substitutions = substitutions;
    }

    public static implicit operator NetworkText(NetText netText)
    {
        return new NetworkText(netText.Text, (NetworkText.Mode)netText.Mode)
        {
            _substitutions = netText.Substitutions != null ? netText.Substitutions.Select(s => (NetworkText)s).ToArray() : Array.Empty<NetworkText>()
        };
    }

    public static implicit operator NetText(NetworkText networkText)
    {
        return new NetText
        {
            Mode = (byte)networkText._mode,
            Text = networkText._text,
            Substitutions = networkText._substitutions?.Select(s => (NetText)s).ToArray()
        };
    }

    public byte Mode;
    public string Text;
    public NetText[]? Substitutions;
}
