using Amethyst.Systems.Users.Base.Permissions;
using Amethyst.Text;

namespace Amethyst.Systems.Commands;

public interface ICommandSender : IPermissionProvider
{
    public SenderType Type { get; }

    public string Language { get; set; }

    public void ReplySuccess(string text, params object[] args);
    public void ReplyInfo(string text, params object[] args);
    public void ReplyError(string text, params object[] args);
    public void ReplyWarning(string text, params object[] args);
    public void ReplyPage(PagesCollection pages, string? header, string? footer, object[]? footerArgs, bool showPageName, int page = 0);
}
