using System.Globalization;
using Amethyst.Network;
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

    private readonly Color _replyErrorColor = new(255, 0, 0);
    private readonly Color _replyInfoColor = new(0, 255, 0);
    private readonly Color _replySuccessColor = new(0, 0, 255);
    private readonly Color _replyWarningColor = new(255, 255, 0);

    public PlayerUser User { get; }

    public string Language { get; set; }

    public void SendMessage(string text) => SendMessage(text, Color.White);

    public void SendMessage(string text, Color color)
    {
        byte[] packet = new PacketWriter().SetType(82)
            .PackUInt16(1) // text id
            .PackByte(255)
            .PackByte(0)
            .PackString(text)
            .PackColor(color)
            .BuildPacket();

        User.Player.SendPacketBytes(packet);
    }

    public void ReplyMessage(string text) => ReplyMessage(text, Color.White);

    public void ReplyMessage(string text, Color color)
        => SendMessage($"[c/303030:{Localization.Get("amethyst.serverPrefix", Language)}:] {text}", color);

    public void ReplyError(string text, params object[] args)
        => ReplyMessage(string.Format(CultureInfo.InvariantCulture, Localization.Get(text, Language), args), _replyErrorColor);

    public void ReplyInfo(string text, params object[] args)
        => ReplyMessage(string.Format(CultureInfo.InvariantCulture, Localization.Get(text, Language), args), _replyInfoColor);

    public void ReplySuccess(string text, params object[] args)
        => ReplyMessage(string.Format(CultureInfo.InvariantCulture, Localization.Get(text, Language), args), _replySuccessColor);

    public void ReplyWarning(string text, params object[] args)
        => ReplyMessage(string.Format(CultureInfo.InvariantCulture, Localization.Get(text, Language), args), _replyWarningColor);

    public void ReplyPage(PagesCollection pages, string? header, string? footer, object[]? footerArgs, bool showPageName, int page = 0)
        => pages.SendPage(User, this, header, footer, footerArgs, showPageName, page);
}
