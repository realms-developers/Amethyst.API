using System.Globalization;
using Amethyst.Kernel;
using Amethyst.Network.Structures;
using Amethyst.Network.Utilities;
using Amethyst.Systems.Users.Base.Messages;
using Amethyst.Text;
using Microsoft.Xna.Framework;

namespace Amethyst.Systems.Users.Players.Messages;

public sealed class PlayerMessageProvider : IMessageProvider
{
    internal PlayerMessageProvider(PlayerUser user, string culture)
    {
        User = user;
        Language = culture;
    }

    public static NetColor ErrorColor { get; set; } = new(255, 0, 0);
    public static NetColor InfoColor { get; set; } = new(255, 255, 0);
    public static NetColor SuccessColor { get; set; } = new(0, 255, 0);
    public static NetColor WarningColor { get; set; } = new(0, 0, 255);

    public PlayerUser User { get; }

    public string Language { get; set; }

    public void SendMessage(string text) => SendMessage(text, new NetColor(255, 255, 255));

    public void SendMessage(string text, NetColor color)
    {
        var writer = new FastPacketWriter(82, 512);

        writer.WriteUInt16(1);
        writer.WriteByte(255);
        writer.WriteByte(0);
        writer.WriteString(text);
        writer.WriteNetColor(color);

        User.Player.SendPacketBytes(writer.Build());
    }

    public void ReplyMessage(string text) => ReplyMessage(text, Color.White);

    public void ReplyMessage(string text, Color color)
        => SendMessage($"[c/303030:{Localization.Get("amethyst.serverPrefix", Language)}:] {text}", color);

    public void ReplyError(string text, params object[] args)
        => ReplyMessage(string.Format(CultureInfo.InvariantCulture, Localization.Get(text, Language), args), ErrorColor);

    public void ReplyInfo(string text, params object[] args)
        => ReplyMessage(string.Format(CultureInfo.InvariantCulture, Localization.Get(text, Language), args), InfoColor);

    public void ReplySuccess(string text, params object[] args)
        => ReplyMessage(string.Format(CultureInfo.InvariantCulture, Localization.Get(text, Language), args), SuccessColor);

    public void ReplyWarning(string text, params object[] args)
        => ReplyMessage(string.Format(CultureInfo.InvariantCulture, Localization.Get(text, Language), args), WarningColor);

    public void ReplyPage(PagesCollection pages, string? header, string? footer, object[]? footerArgs, bool showPageName, int page = 0)
    {
        if (pages.Pages.Count > 0)
        {
            User.Commands.ActivePage = pages;
        }
        pages.SendPage(User, this, header, footer, footerArgs, showPageName, page);
    }
}
