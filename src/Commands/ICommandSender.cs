using Amethyst.Permissions;
using Amethyst.Localization;
using Amethyst.Text;

namespace Amethyst.Commands;

public interface ICommandSender : IPermissionable
{
    public SenderType Type { get; }
    public Language Language { get; }

    public void ReplySuccess(string text, params object[] args);
    public void ReplyInfo(string text, params object[] args);
    public void ReplyError(string text, params object[] args);
    public void ReplyWarning(string text, params object[] args);
    public void ReplyPage(PagesCollection pages, string? header, string? footer, object[]? footerArgs, bool showPageName, int page = 0);
}
