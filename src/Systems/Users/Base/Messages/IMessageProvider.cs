using Amethyst.Text;

namespace Amethyst.Systems.Users.Base.Messages;

public interface IMessageProvider
{
    string Culture { get; set; }

    void ReplySuccess(string text, params object[] args);
    void ReplyInfo(string text, params object[] args);
    void ReplyError(string text, params object[] args);
    void ReplyWarning(string text, params object[] args);
    void ReplyPage(PagesCollection pages, string? header, string? footer, object[]? footerArgs, bool showPageName, int page = 0);
}
